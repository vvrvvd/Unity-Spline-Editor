// <copyright file="SplineMesh.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
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
		private const float MinSpacingValue = 0.1f;
		private const float MinWidthValue = 0.001f;

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
		private MeshJobExecutor generateMeshJobExecutor;

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
		public List<Vector3> Points
		{
			get => splinePath.Points;
			set => splinePath.Points = value;
		}

		/// <summary>
		/// Gets or sets normals corresponding to the points on mesh.
		/// Generated based on Tangents.
		/// </summary>
		public List<Vector3> Normals
		{
			get => splinePath.Normals;
			set => splinePath.Normals = value;
		}

		/// <summary>
		/// Gets or sets tangents of the points on the mesh based on the spline.
		/// </summary>
		public List<Vector3> Tangents
		{
			get => splinePath.Tangents;
			set => splinePath.Tangents = value;
		}

		/// <summary>
		/// Gets or sets scales of the points on the mesh.
		/// </summary>
		public List<Vector3> Scale
		{
			get => splinePath.Scales;
			set => splinePath.Scales = value;
		}

		/// <summary>
		/// Gets or sets value of parameter T on the points on mesh regarding to spline.
		/// </summary>
		public List<float> ParametersT
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
				GenerateMesh();
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
				GenerateMesh();
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
				GenerateMesh();
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
				GenerateMesh();
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
				GenerateMesh();
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
				GenerateMesh();
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
				GenerateMesh();
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
				GenerateMesh();
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
		/// Constructs and updates spline mesh using Unity Jobs.
		/// </summary>
		[ContextMenu("Generate Mesh")]
		public void GenerateMesh()
		{
			var config = SplineMeshConfiguration.Instance;
			GenerateMesh(!config.UseJobsWithCorotuines);
		}

		/// <summary>
		/// Constructs and assignes mesh based on the spline using Unity Jobs system.
		/// </summary>
		/// <param name="immediate">Should mesh be generated in the same frame.</param>
		/// <param name="onMeshGenerated">Action invoked when mesh is generated.</param>
		public void GenerateMesh(bool immediate = false, Action<Mesh> onMeshGenerated = null)
		{
			if (BezierSpline == null || MeshFilter == null)
			{
				return;
			}

			if (cachedMesh == null)
			{
				cachedMesh = new Mesh();
			}

			if (splinePath == null)
			{
				splinePath = new SplinePath();
			}

			if (generateMeshJobExecutor == null)
			{
				generateMeshJobExecutor = new MeshJobExecutor(this, splinePath);
			}

			generateMeshJobExecutor.GenerateMesh(cachedMesh, onMeshGenerated, immediate);
			meshFilter.sharedMesh = cachedMesh;
		}

		private void OnValidate()
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

			if (splinePath == null)
			{
				splinePath = new SplinePath();
			}

#if UNITY_EDITOR
			bezierSpline.OnSplineChanged -= GenerateMesh;
			bezierSpline.OnSplineChanged += GenerateMesh;
#endif
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

			if (splinePath == null)
			{
				splinePath = new SplinePath();
			}

			Assert.IsNotNull(meshFilter);
			Assert.IsNotNull(meshRenderer);
			Assert.IsNotNull(bezierSpline);

#if UNITY_EDITOR
			bezierSpline.OnSplineChanged -= GenerateMesh;
			bezierSpline.OnSplineChanged += GenerateMesh;
#endif
		}

		private void OnEnable()
		{
			if (bezierSpline == null)
			{
				return;
			}

			bezierSpline.OnSplineChanged += GenerateMesh;
			GenerateMesh();
		}

		private void OnDisable()
		{
			if (bezierSpline == null)
			{
				return;
			}

			bezierSpline.OnSplineChanged -= GenerateMesh;
		}
	}
}