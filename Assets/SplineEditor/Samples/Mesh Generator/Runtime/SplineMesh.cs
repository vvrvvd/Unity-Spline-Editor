// <copyright file="SplineMesh.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SplineEditor.MeshGenerator
{
	/// <summary>
	/// Component for generating flat mesh based on a bezier spline.
	/// </summary>
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BezierSpline))]
	[CanEditMultipleObjects]
	[ExecuteAlways]
	public class SplineMesh : MonoBehaviour
	{
		private const float Precision = 0.0001f;
		private const float MinSpacingValue = 0.1f;
		private const float MinWidthValue = 0.001f;

#if UNITY_EDITOR
		private const int EditorLateUpdateFramesDelay = 5;
#endif

		[SerializeField]
		private float width = 5f;
		[SerializeField]
		private float spacing = 1f;
		[SerializeField]
		private bool useAsymetricWidthCurve = false;
		[SerializeField]
		private bool usePointsScale = true;
		[SerializeField]
		private UVMode uvMode = UVMode.Linear;
		[SerializeField]
		private bool mirrorUV = false;
		[SerializeReference]
		private CustomAnimationCurve rightSideCurve = CustomAnimationCurve.Constant(0f, 1f, 1f);
		[SerializeReference]
		private CustomAnimationCurve leftSideCurve = CustomAnimationCurve.Constant(0f, 1f, 1f);
		[SerializeField]
		private MeshFilter meshFilter;
		[SerializeField]
		private MeshRenderer meshRenderer;
		[SerializeField]
		private BezierSpline bezierSpline;
		[SerializeReference]
		private SplinePath splinePath;

		private Mesh cachedMesh;

		private bool updateMesh = false;

		/// <summary>
		/// UV generation mode type.
		/// Determines how the UV is generated.
		/// </summary>
		public enum UVMode
		{
			/// <summary>
			/// UV is simply generated from [0..1].
			/// </summary>
			Linear,

			/// <summary>
			/// UV is generated to the half of the texture and then is mirrored [0..1..0].
			/// </summary>
			PingPong,
		}

		/// <summary>
		/// Gets or sets points on generated on the spline to create the mesh.
		/// </summary>
		public Vector3[] Points
		{
			get => splinePath.Points;
			set => splinePath.Points = value;
		}

		/// <summary>
		/// Gets or sets normals corresponding to the points on mesh.
		/// Generated based on Tangents.
		/// </summary>
		public Vector3[] Normals
		{
			get => splinePath.Normals;
			set => splinePath.Normals = value;
		}

		/// <summary>
		/// Gets or sets tangents of the points on the mesh based on the spline.
		/// </summary>
		public Vector3[] Tangents
		{
			get => splinePath.Tangents;
			set => splinePath.Tangents = value;
		}

		/// <summary>
		/// Gets or sets scales of the points on the mesh.
		/// </summary>
		public Vector3[] Scale
		{
			get => splinePath.Scales;
			set => splinePath.Scales = value;
		}

		/// <summary>
		/// Gets or sets value of parameter T on the points on mesh regarding to spline.
		/// </summary>
		public float[] ParametersT
		{
			get => splinePath.ParametersT;
			set => splinePath.ParametersT = value;
		}

		/// <summary>
		/// Gets or sets width of the generated mesh.
		/// </summary>
		public float Width
		{
			get => width;
			set
			{
				var newValue = Mathf.Max(MinWidthValue, value);
				width = newValue;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets or sets distance between evenly spaced points on the curve.
		/// </summary>
		public float Spacing
		{
			get => spacing;
			set
			{
				var newValue = Mathf.Max(MinSpacingValue, value);
				spacing = newValue;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether RightSideCurve is used for sampling width on both sides of the mesh.
		/// If false RightSideCurve is used for sampling width on right side of the mesh and LeftSideCurve for left side of the mesh.
		/// </summary>
		public bool UseAsymetricWidthCurve
		{
			get => useAsymetricWidthCurve;
			set
			{
				useAsymetricWidthCurve = value;
				updateMesh = true;

			}
		}

		/// <summary>
		/// Gets or sets a curve used for sampling width of the right side of the mesh.
		/// If UseAsymetricWidthCurve is set to false then it's also used for sampling width of the left side as well.
		/// </summary>
		public CustomAnimationCurve RightSideCurve
		{
			get => rightSideCurve;
			set
			{
				rightSideCurve = value;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets or sets a curve used for sampling width of the left side of the mesh.
		/// Used only if UseAsymetricWidthCurve is set to true.
		/// </summary>
		public CustomAnimationCurve LeftSideCurve
		{
			get => leftSideCurve;
			set
			{
				leftSideCurve = value;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether points scales from BezierCurve component are used as multiplier for sampling width curve of the mesh.
		/// </summary>
		public bool UsePointsScale
		{
			get => usePointsScale;
			set
			{
				usePointsScale = value;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets or sets mode used for generating UV on the mesh.
		/// </summary>
		public UVMode UvMode
		{
			get => uvMode; set
			{
				uvMode = value;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether UV is being mirrored on both axis.
		/// For mirroring UV the original value is being substracted from one (1 - UV).
		/// </summary>
		public bool MirrorUV
		{
			get => mirrorUV; set
			{
				mirrorUV = value;
				updateMesh = true;
			}
		}

		/// <summary>
		/// Gets MeshFilter attached to the SplineMesh component.
		/// </summary>
		public MeshFilter MeshFilter => meshFilter;

		/// <summary>
		/// Gets MeshRenderer attached to the SplineMesh component.
		/// </summary>
		public MeshRenderer MeshRenderer => meshRenderer;

		/// <summary>
		/// Gets BezierSpline attached to the SplineMesh component.
		/// </summary>
		public BezierSpline BezierSpline => bezierSpline;

		/// <summary>
		/// Constructs and assigned mesh based on the spline.
		/// </summary>
		[ContextMenu("Generate Mesh")]
		public void GenerateMesh()
		{
			if (BezierSpline == null || MeshFilter == null)
			{
				return;
			}

			if(cachedMesh == null) 
			{
				cachedMesh = new Mesh();
			}

			if (splinePath == null) 
			{
				splinePath = new SplinePath();
			}

			bezierSpline.GetEvenlySpacedPoints(Spacing, splinePath, 0.0001f, false);

			TestJobs.GenerateMesh(this, splinePath, cachedMesh);
			meshFilter.sharedMesh = cachedMesh;
		}

		/// <summary>
		/// Constructs mesh based on the spline.
		/// </summary>
		/// <returns>Constructed mesh based on BezierSpline component.</returns>
		public Mesh ConstructMesh()
		{
			int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

			if (splinePath == null)
			{
				splinePath = new SplinePath();
			}

			BezierSpline.RecalculateNormals();
			BezierSpline.GetEvenlySpacedPoints(Spacing, splinePath, Precision, false);

			var isLoop = BezierSpline.IsLoop;
			var verts = new Vector3[Points.Length * 2];
			var normals = new Vector3[Points.Length * 2];
			var uvs = new Vector2[verts.Length];
			var numTris = (2 * (Points.Length - 1)) + (isLoop ? 2 : 1);
			var tris = new int[numTris * 3];
			var vertIndex = 0;
			var triIndex = 0;

			for (int i = 0; i < Points.Length; i++)
			{
				var normalVector = Normals[i];
				var right = Vector3.Cross(normalVector, Tangents[i]).normalized;
				var rightScaledWidth = Width * (UsePointsScale ? Scale[i].x : 1f) * RightSideCurve.Evaluate(ParametersT[i]);
				var leftScaledWidth = Width * (UsePointsScale ? Scale[i].x : 1f) * LeftSideCurve.Evaluate(ParametersT[i]);

				verts[vertIndex] = Points[i] - (right * (UseAsymetricWidthCurve ? leftScaledWidth : rightScaledWidth));
				verts[vertIndex + 1] = Points[i] + (right * rightScaledWidth);

				normals[vertIndex] = normalVector;
				normals[vertIndex + 1] = normalVector;

				var v = GetUV(i);
				uvs[vertIndex] = new Vector2(0, v);
				uvs[vertIndex + 1] = new Vector2(1, v);

				if (i < Points.Length - 1 || isLoop)
				{
					for (int j = 0; j < triangleMap.Length; j++)
					{
						tris[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
					}
				}

				vertIndex += 2;
				triIndex += 6;
			}

			if (cachedMesh == null)
			{
				cachedMesh = new Mesh();
			}

			cachedMesh.Clear();
			cachedMesh.vertices = verts;
			cachedMesh.normals = normals;
			cachedMesh.triangles = tris;
			cachedMesh.uv = uvs;

			return cachedMesh;
		}

		private float GetUV(int pointIndex)
		{
			var uv = pointIndex / (float)(Points.Length - 1);
			switch (UvMode)
			{
				case UVMode.PingPong:
					uv = 1 - Mathf.Abs((2 * uv) - 1);
					break;
				case UVMode.Linear:
				default:
					break;
			}

			return MirrorUV ? 1 - uv : uv;
		}

		private void OnValidate()
		{
			updateMesh = true;

			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}

			if (meshRenderer == null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
			}

			if (bezierSpline == null)
			{
				bezierSpline = GetComponent<BezierSpline>();
			}
		}

		private void Awake()
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}

			if (meshRenderer == null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
			}

			if (bezierSpline == null)
			{
				bezierSpline = GetComponent<BezierSpline>();
			}

			Assert.IsNotNull(meshFilter);
			Assert.IsNotNull(meshRenderer);
			Assert.IsNotNull(bezierSpline);
		}

		private void OnEnable()
		{
			bezierSpline.OnSplineChanged += GenerateMesh;
		}

		private void OnDisable()
		{
			bezierSpline.OnSplineChanged -= GenerateMesh;
		}

		private void LateUpdate()
		{
			if (!updateMesh)
			{
				return;
			}

			GenerateMesh();
			updateMesh = false;
		}
	}
}
