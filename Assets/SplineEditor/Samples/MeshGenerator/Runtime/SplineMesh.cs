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

		public int segmentsCount = 10;
		public float precision = 0.01f;
		public float width = 5f;

		#endregion

		#region Private Fields


		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;
		private BezierSpline bezierSpline;

		private Mesh mesh;

		private Vector3[] uvs;
		private Vector3[] normals;
		private Vector3[] vertices;
		private Vector3[] segmentPoints;

		#endregion

		#region Properties

		public MeshFilter MeshFilter => meshFilter;
		public MeshRenderer MeshRenderer => meshRenderer;
		public BezierSpline BezierSpline => bezierSpline;

		#endregion

		#region Initialize

		private void OnValidate()
		{
			precision = Mathf.Max(precision, 0.001f);
			segmentsCount = Mathf.Max(segmentsCount, 1);

			ResizePointsArray();
			GenerateMesh();

			bezierSpline.OnSplineChanged -= GenerateMesh;
			bezierSpline.OnSplineChanged += GenerateMesh;
		}

		private void Awake()
		{
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			bezierSpline = GetComponent<BezierSpline>();

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
			GenerateEvenlySpacedPoints();

			var isLoop = BezierSpline.IsLoop;
			var verts = new Vector3[segmentPoints.Length * 2];
			var uvs = new Vector2[verts.Length];
			var numTris = 2 * (segmentPoints.Length - 1) + (isLoop ? 2 : 0);
			var tris = new int[numTris * 3];
			var vertIndex = 0;
			var triIndex = 0;

			for (int i = 0; i < segmentPoints.Length; i++)
			{
				var forward = Vector3.zero;
				if (i < segmentPoints.Length - 1 || isLoop)
				{
					forward += segmentPoints[(i + 1) % segmentPoints.Length] - segmentPoints[i];
				}
				if (i > 0 || isLoop)
				{
					forward += segmentPoints[i] - segmentPoints[(i - 1 + segmentPoints.Length) % segmentPoints.Length];
				}

				forward.Normalize();
				var left = new Vector3(-forward.y, forward.x);

				verts[vertIndex] = segmentPoints[i] + left * width * .5f;
				verts[vertIndex + 1] = segmentPoints[i] - left * width * .5f;

				float completionPercent = i / (float)(segmentPoints.Length - 1);
				float v = 1 - Mathf.Abs(2 * completionPercent - 1);
				uvs[vertIndex] = new Vector2(0, v);
				uvs[vertIndex + 1] = new Vector2(1, v);

				if (i < segmentPoints.Length - 1 || isLoop)
				{
					tris[triIndex] = vertIndex;
					tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
					tris[triIndex + 2] = vertIndex + 1;

					tris[triIndex + 3] = vertIndex + 1;
					tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
					tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
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



		private void GenerateEvenlySpacedPoints()
		{
			ResizePointsArray();
			BezierSpline.GetEvenlySpacedPointsNonAlloc(segmentsCount, segmentPoints, precision, false);
		}

		private void ResizePointsArray()
		{
			if (BezierSpline == null)
			{
				return;
			}

			if (segmentPoints == null)
			{
				segmentPoints = new Vector3[segmentsCount];
			}

			if ((BezierSpline.IsLoop && segmentPoints.Length != segmentsCount) || (!BezierSpline.IsLoop && segmentPoints.Length != segmentsCount + 1))
			{
				var newArraySize = BezierSpline.IsLoop ? segmentsCount : segmentsCount + 1;
				Array.Resize(ref segmentPoints, newArraySize);
			}
		}

		#endregion

	}

}
