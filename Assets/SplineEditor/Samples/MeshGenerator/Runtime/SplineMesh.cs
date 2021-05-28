using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace SplineEditor.MeshGenerator
{

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BezierSpline))]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class SplineMesh : MonoBehaviour
	{

		#region Public Fields

		public float spacing = 1f;
		public float precision = 0.01f;
		public float width = 5f;
		public bool flipNormals = false;

		#endregion

		#region Private Fields

		[SerializeField, HideInInspector]
		private MeshFilter meshFilter;
		[SerializeField, HideInInspector]
		private MeshRenderer meshRenderer;
		[SerializeField, HideInInspector]
		private BezierSpline bezierSpline;

		private Mesh mesh;

		private Vector3[] uvs;
		private Vector3[] normals;
		private Vector3[] vertices;

		private SplinePath splinePath;

		#endregion

		#region Properties

		public Vector3[] Points
		{
			get => splinePath.points;
			set => splinePath.points = value;
		}

		public Vector3[] Normals
		{
			get => normals;
			set => normals = value;
		}

		public Vector3[] Tangents
		{
			get => splinePath.tangents;
			set => splinePath.tangents = value;
		}

		public MeshFilter MeshFilter => meshFilter;
		public MeshRenderer MeshRenderer => meshRenderer;
		public BezierSpline BezierSpline => bezierSpline;
		public SplinePath SplinePath => splinePath;

		#endregion

		#region Initialize

		private void OnValidate()
		{
			precision = Mathf.Max(precision, 0.001f);
			spacing = Mathf.Max(spacing, 0.1f);

			GenerateMesh();

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

			bezierSpline.OnSplineChanged -= GenerateMesh;
			bezierSpline.OnSplineChanged += GenerateMesh;
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

			bezierSpline.OnSplineChanged += GenerateMesh;
		}

		#endregion

		#region Public Methods

		public void GenerateMesh()
		{
			if (BezierSpline == null || MeshFilter == null)
			{
				return;
			}

			mesh = ConstructMesh();
			meshFilter.sharedMesh = mesh;
		}

		public Mesh ConstructMesh()
		{
			int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

			if(splinePath == null)
			{
				splinePath = new SplinePath();
			}

			BezierSpline.GetEvenlySpacedPoints(spacing, splinePath, precision, false);
			RecalculateNormals();

			var isLoop = BezierSpline.IsLoop;
			var verts = new Vector3[Points.Length * 2];
			var uvs = new Vector2[verts.Length];
			var numTris = 2 * (Points.Length - 1) + (isLoop ? 2 : 1);
			var tris = new int[numTris * 3];
			var vertIndex = 0;
			var triIndex = 0;

			for (int i = 0; i < Points.Length; i++)
			{

				var up = Vector3.Cross(Tangents[i], Normals[i]);
				var right = Vector3.Cross(Normals[i], Tangents[i]);

				verts[vertIndex] = Points[i] - right * width * .5f;
				verts[vertIndex + 1] = Points[i] + right * width * .5f;

				float completionPercent = i / (float)(Points.Length - 1);
				float v = 1 - Mathf.Abs(2 * completionPercent - 1);
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

			var mesh = new Mesh();
			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.uv = uvs;

			return mesh;
		}

		public void RecalculateNormals()
		{
			var isLoop = bezierSpline.IsLoop;

			if(Normals == null)
			{
				Normals = new Vector3[Points.Length];
			}
			else if (Normals.Length != Points.Length)
			{
				Array.Resize(ref normals, Points.Length);
			}

			var lastRotationAxis = (flipNormals ? 1 : -1) * Vector3.forward;
			for (var i = 0; i < Points.Length; i++)
			{
				if (i == 0)
				{
					Normals[0] = Vector3.Cross(lastRotationAxis, Tangents[0]).normalized;
				}
				else
				{
					// First reflection
					Vector3 offset = (Points[i] - Points[i - 1]);
					float sqrDst = offset.sqrMagnitude;
					Vector3 r = lastRotationAxis - offset * 2 / sqrDst * Vector3.Dot(offset, lastRotationAxis);
					Vector3 t = Tangents[i - 1] - offset * 2 / sqrDst * Vector3.Dot(offset, Tangents[i - 1]);

					// Second reflection
					Vector3 v2 = Tangents[i] - t;
					float c2 = Vector3.Dot(v2, v2);

					Vector3 finalRot = r - v2 * 2 / c2 * Vector3.Dot(v2, r);
					Vector3 n = Vector3.Cross(finalRot, Tangents[i]).normalized;
					Normals[i] = n;
					lastRotationAxis = finalRot;
				}
			}

			if (isLoop && Normals.Length > 1)
			{
				// Get angle between first and last normal (if zero, they're already lined up, otherwise we need to correct)
				float normalsAngleErrorAcrossJoin = Vector3.SignedAngle(Normals[Normals.Length - 1], Normals[0], Tangents[0]);
				// Gradually rotate the normals along the path to ensure start and end normals line up correctly
				if (Mathf.Abs(normalsAngleErrorAcrossJoin) > 0.1f) // don't bother correcting if very nearly correct
				{
					for (int i = 1; i < Normals.Length; i++)
					{
						float t = (i / (Normals.Length - 1f));
						float angle = normalsAngleErrorAcrossJoin * t;
						Quaternion rot = Quaternion.AngleAxis(angle, Tangents[i]);

						Normals[i] = rot * Normals[i];
					}
				}

			}
		}

		#endregion

		#region Private Methods

		#endregion
	}

}
