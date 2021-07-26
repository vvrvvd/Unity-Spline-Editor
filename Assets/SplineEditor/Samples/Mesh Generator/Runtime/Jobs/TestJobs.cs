using SplineEditor;
using SplineEditor.MeshGenerator;
using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

public static class TestJobs
{

	public struct GenerateMeshJob : IJobParallelFor 
	{
		private static readonly int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

		[ReadOnly]
		public float width;
		[ReadOnly]
		public bool useAsymetricWidthCurve;
		[ReadOnly]
		public int uvMode; // 0 - Linear, 1 - PingPong
		[ReadOnly]
		public bool mirrorUv;
		[ReadOnly]
		public bool isLoop;
		[ReadOnly]
		public NativeArray<Vector3> scales;
		[ReadOnly]
		public NativeArray<Vector3> normals;
		[ReadOnly]
		public NativeArray<Vector3> tangents;
		[ReadOnly]
		public NativeArray<Vector3> positions;

		[NativeDisableParallelForRestriction]
		public NativeArray<int> trisResult;
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector2> uvsResult;
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> vertsResult;
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> normalsResult;

		public void Execute(int i) {
			var vertIndex = i * 2;
			var trisIndex = i * 6;

			//Normals
			normalsResult[vertIndex] = normals[i];
			normalsResult[vertIndex + 1] = normals[i];

			//Vertices
			var right = Vector3.Cross(normals[i], tangents[i]).normalized;
			var rightScaledWidth = width * scales[i].x;
			var leftScaledWidth = width * scales[i].x;
			vertsResult[vertIndex] = positions[i] - (right * (useAsymetricWidthCurve ? leftScaledWidth : rightScaledWidth));
			vertsResult[vertIndex + 1] = positions[i] + (right * rightScaledWidth);

			//UV
			var uv = GetUV(i);
			uvsResult[vertIndex] = new Vector2(0, uv);
			uvsResult[vertIndex + 1] = new Vector2(1, uv);

			//Triangles
			if (i < positions.Length - 1 || isLoop) {
				for (int j = 0; j < triangleMap.Length; j++) {
					trisResult[trisIndex + j] = (vertIndex + triangleMap[j]) % vertsResult.Length;
				}
			}

		}

		private float GetUV(int pointIndex) {
			var uv = pointIndex / (float)(positions.Length - 1);
			switch (uvMode) {
				case 1: //PingPong
					uv = 1 - Mathf.Abs((2 * uv) - 1);
					break;
				case 0: //Linear
				default:
					break;
			}

			return mirrorUv ? 1 - uv : uv;
		}

	}

	[MenuItem("Jobs/Test Job")]
	private static void TestJob() {
		var splineMesh = UnityEngine.Object.FindObjectOfType<SplineMesh>();
		var splinePath = new SplinePath();
		var bezierSpline = splineMesh.BezierSpline;

		bezierSpline.RecalculateNormals();
		bezierSpline.GetEvenlySpacedPoints(splineMesh.Spacing, splinePath, 0.0001f, false);

		var width = splineMesh.Width;
		var scales = new NativeArray<Vector3>(splinePath.Scales, Allocator.TempJob);
		var normals = new NativeArray<Vector3>(splinePath.Normals, Allocator.TempJob);
		var tangents = new NativeArray<Vector3>(splinePath.Tangents, Allocator.TempJob);
		var positions = new NativeArray<Vector3>(splinePath.Points, Allocator.TempJob);

		var numTris = (2 * (positions.Length - 1)) + (bezierSpline.IsLoop ? 2 : 1);
		var trisResult = new NativeArray<int>(numTris * 3, Allocator.TempJob);
		var uvsResult = new NativeArray<Vector2>(positions.Length * 2, Allocator.TempJob);
		var vertsResult = new NativeArray<Vector3>(positions.Length * 2, Allocator.TempJob);
		var normalsResult = new NativeArray<Vector3>(positions.Length * 2, Allocator.TempJob);

		GenerateMeshJob generateMeshJob = new GenerateMeshJob();
		
		//Input data
		generateMeshJob.width = width;
		generateMeshJob.useAsymetricWidthCurve = splineMesh.UseAsymetricWidthCurve;
		generateMeshJob.uvMode = (int)splineMesh.UvMode;
		generateMeshJob.mirrorUv = splineMesh.MirrorUV;
		generateMeshJob.isLoop = bezierSpline.IsLoop;
		generateMeshJob.scales = scales;
		generateMeshJob.normals = normals;
		generateMeshJob.tangents = tangents;
		generateMeshJob.positions = positions;

		//Output data
		generateMeshJob.trisResult = trisResult;
		generateMeshJob.uvsResult = uvsResult;
		generateMeshJob.vertsResult = vertsResult;
		generateMeshJob.normalsResult = normalsResult;

		// Schedule the job with one Execute per index in the results array and only 1 item per processing batch
		JobHandle generateMeshJobHandle = generateMeshJob.Schedule(positions.Length, 32);

		var sharedMesh = splineMesh.MeshFilter.sharedMesh;

		if (sharedMesh == null) {
			sharedMesh = new Mesh();
		}

		sharedMesh.Clear();
		splineMesh.MeshFilter.sharedMesh = sharedMesh;

		// Wait for the job to complete
		generateMeshJobHandle.Complete();

		splineMesh.MeshFilter.sharedMesh.vertices = generateMeshJob.vertsResult.ToArray();
		splineMesh.MeshFilter.sharedMesh.normals = generateMeshJob.normalsResult.ToArray();
		splineMesh.MeshFilter.sharedMesh.triangles = generateMeshJob.trisResult.ToArray();
		splineMesh.MeshFilter.sharedMesh.uv = generateMeshJob.uvsResult.ToArray();

		// Free the memory allocated by the arrays
		scales.Dispose();
		normals.Dispose();
		tangents.Dispose();
		positions.Dispose();

		trisResult.Dispose();
		uvsResult.Dispose();
		vertsResult.Dispose();
		normalsResult.Dispose();
	}

	private static IEnumerator WaitForJobToFinish(JobHandle jobHandle, Action onJobFinished) {
		var startTime = Time.realtimeSinceStartup;
		yield return new WaitWhile(() => !jobHandle.IsCompleted);

		jobHandle.Complete();
		onJobFinished?.Invoke();
		var deltaTime = Time.realtimeSinceStartup - startTime;
		Debug.Log("Finished in: " + deltaTime);
	}

}
