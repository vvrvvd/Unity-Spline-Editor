using SplineEditor;
using SplineEditor.MeshGenerator;
using System;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.EditorCoroutines.Editor;
using Unity.Jobs;
using UnityEngine;

public static class TestJobs
{

	[BurstCompile]
	public struct GenerateMeshJob : IJobParallelFor
	{
		private static readonly int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

		[ReadOnly]
		public float width;
		[ReadOnly]
		public bool usePointsScale;
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
			var rightScaledWidth = width * (usePointsScale ? scales[i].x : 1);
			var leftScaledWidth = width * (usePointsScale ? scales[i].x : 1);
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

	private static bool isJobScheduled = false;
	private static bool scheduleNextJob = false;
	
	public static void GenerateMesh(SplineMesh splineMesh, SplinePath splinePath, Mesh mesh) {
		if (isJobScheduled) {
			scheduleNextJob = true;
			return;
		}

		isJobScheduled = true;
		var generateMeshJob = PrepareGenerateMeshJob(splineMesh, splinePath);
		var jobHandle = generateMeshJob.Schedule(generateMeshJob.positions.Length, 64);

		EditorCoroutineUtility.StartCoroutine(WaitForJobToFinish(jobHandle, splineMesh, () => OnJobCompleted(ref generateMeshJob, mesh)), splineMesh);
	}

	private static GenerateMeshJob PrepareGenerateMeshJob(SplineMesh splineMesh, SplinePath splinePath) {
		var bezierSpline = splineMesh.BezierSpline;

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

		GenerateMeshJob generateMeshJob = new GenerateMeshJob() {
			//Input data
			width = width,
			usePointsScale = splineMesh.UsePointsScale,
			useAsymetricWidthCurve = splineMesh.UseAsymetricWidthCurve,
			uvMode = (int)splineMesh.UvMode,
			mirrorUv = splineMesh.MirrorUV,
			isLoop = bezierSpline.IsLoop,
			scales = scales,
			normals = normals,
			tangents = tangents,
			positions = positions,

			//Output data
			trisResult = trisResult,
			uvsResult = uvsResult,
			vertsResult = vertsResult,
			normalsResult = normalsResult,
		};

		return generateMeshJob;
	}

	private static IEnumerator WaitForJobToFinish(JobHandle jobHandle, SplineMesh splineMesh, Action onJobFinished) {
		var framesCounter = 0;
		while (!jobHandle.IsCompleted) {
			yield return null;
			framesCounter++;
		}

		jobHandle.Complete();
		isJobScheduled = false;
		onJobFinished?.Invoke();

		if (scheduleNextJob) {
			splineMesh.GenerateMesh();
			scheduleNextJob = false;
		}
	}

	private static void OnJobCompleted(ref GenerateMeshJob generateMeshJob, Mesh mesh) {
		mesh.Clear();
		mesh.vertices = generateMeshJob.vertsResult.ToArray();
		mesh.normals = generateMeshJob.normalsResult.ToArray();
		mesh.triangles = generateMeshJob.trisResult.ToArray();
		mesh.uv = generateMeshJob.uvsResult.ToArray();

		CleanUp(ref generateMeshJob);
	}

	private static void CleanUp(ref GenerateMeshJob generateMeshJob) {

		// Free the memory allocated by the arrays
		generateMeshJob.scales.Dispose();
		generateMeshJob.normals.Dispose();
		generateMeshJob.tangents.Dispose();
		generateMeshJob.positions.Dispose();

		generateMeshJob.trisResult.Dispose();
		generateMeshJob.uvsResult.Dispose();
		generateMeshJob.vertsResult.Dispose();
		generateMeshJob.normalsResult.Dispose();
	}

}
