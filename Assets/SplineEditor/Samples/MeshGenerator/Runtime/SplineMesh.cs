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
		private Vector3[] vertices;
		private BezierSpline.BezierPath bezierPath;

		#endregion

		#region Properties

		public int SegmentCounts
		{
			//TODO: Cache
			get
			{
				var count = (int)(BezierSpline.GetLinearLength(false) / spacing);

				if (!BezierSpline.IsLoop)
				{
					count += 1;
				}
				return count;
			}
		}

		public MeshFilter MeshFilter => meshFilter;
		public MeshRenderer MeshRenderer => meshRenderer;
		public BezierSpline BezierSpline => bezierSpline;
		public BezierSpline.BezierPath BezierPath => bezierPath;

		#endregion

		#region Initialize

		private void OnValidate()
		{
			precision = Mathf.Max(precision, 0.001f);
			spacing = Mathf.Max(spacing, 0.1f);

			ResizePointsArray();
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

		//TODO: Move it

		public Mesh ConstructMesh()
		{
			int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

			ResizePointsArray();
			BezierSpline.GetEvenlySpacedPoints(spacing, bezierPath, precision, false);
			bezierPath.RecalculateNormals(bezierSpline.IsLoop);

			var isLoop = BezierSpline.IsLoop;
			var verts = new Vector3[bezierPath.points.Length * 2];
			var uvs = new Vector2[verts.Length];
			var numTris = 2 * (bezierPath.points.Length - 1) + (isLoop ? 2 : 1);
			var tris = new int[numTris * 3];
			var vertIndex = 0;
			var triIndex = 0;

			//TODO: Add auto normals
			var usePathNormals = true;

			for (int i = 0; i < bezierPath.points.Length; i++)
			{

				var up = usePathNormals ? Vector3.Cross(bezierPath.tangents[i], bezierPath.normals[i]) : Vector3.up;
				var right = usePathNormals ? bezierPath.normals[i] : Vector3.Cross(up, bezierPath.tangents[i]);

				verts[vertIndex] = bezierPath.points[i] - right * width * .5f;
				verts[vertIndex + 1] = bezierPath.points[i] + right * width * .5f;

				float completionPercent = i / (float)(bezierPath.points.Length - 1);
				float v = 1 - Mathf.Abs(2 * completionPercent - 1);
				uvs[vertIndex] = new Vector2(0, v);
				uvs[vertIndex + 1] = new Vector2(1, v);

				if (i < bezierPath.points.Length - 1 || isLoop)
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

		#endregion

		#region Private Methods

		private void ResizePointsArray()
		{
			if (BezierSpline == null)
			{
				return;
			}

			var segmentsCount = SegmentCounts;

			if (bezierPath == null)
			{
				bezierPath = new BezierSpline.BezierPath(segmentsCount);
			}

			if (bezierPath.points.Length != spacing)
			{
				var newArraySize = segmentsCount;
				Array.Resize(ref bezierPath.points, newArraySize);
				Array.Resize(ref bezierPath.normals, newArraySize);
				Array.Resize(ref bezierPath.tangents, newArraySize);
			}
		}

		#endregion

	}

}
