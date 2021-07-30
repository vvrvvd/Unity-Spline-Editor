using SplineEditor;
using SplineEditor.MeshGenerator;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

public class GenerateMeshJobExecutor
{

	private const int JobBatchSize = 64;
	private const float SplinePathPrecision = 0.001f;

	public bool isJobScheduled = false;
	private bool scheduleNextJob = false;

#if UNITY_EDITOR
	private EditorCoroutine generateMeshCoroutine;
#else
	private Coroutine generateMeshCoroutine;
	private MonoBehaviour coroutineContext;
#endif

	private SplineMesh splineMesh;
	private SplinePath splinePath;

	public GenerateMeshJobExecutor(SplineMesh splineMesh, SplinePath splinePath) 
	{
		this.splineMesh = splineMesh;
		this.splinePath = splinePath;
	}

	public void GenerateMesh(Mesh mesh, Action<Mesh> onMeshGenerated, bool immediate)
	{
		if (isJobScheduled)
		{
			scheduleNextJob = true;
			return;
		}

		isJobScheduled = true;
		scheduleNextJob = false;
		StopJobCoroutine();
		var generateMeshJob = PrepareGenerateMeshJob();
		StartJob(generateMeshJob, splineMesh, mesh, onMeshGenerated, immediate);
	}

	private void StopJobCoroutine() {
		if (generateMeshCoroutine == null) 
		{
			return;
		}

#if UNITY_EDITOR
			EditorCoroutineUtility.StopCoroutine(generateMeshCoroutine);
#else
			coroutineContext.StopCoroutine(generateMeshCoroutine);
#endif
	}

	private GenerateMeshJob PrepareGenerateMeshJob()
	{
		splineMesh.BezierSpline.GetEvenlySpacedPoints(splineMesh.Spacing, splinePath, SplinePathPrecision, false);

		var bezierSpline = splineMesh.BezierSpline;

		var leftCurveScales = new float[splinePath.Points.Count];
		var rightCurveScales = new float[splinePath.Points.Count];
		for (var i = 0; i < leftCurveScales.Length; i++)
		{
			var t = splinePath.ParametersT[i];
			leftCurveScales[i] = splineMesh.LeftSideCurve.Evaluate(t);
			rightCurveScales[i] = splineMesh.RightSideCurve.Evaluate(t);
		}

		var width = splineMesh.Width;
		var scales = new NativeArray<Vector3>(splinePath.Scales.ToArray(), Allocator.TempJob);
		var normals = new NativeArray<Vector3>(splinePath.Normals.ToArray(), Allocator.TempJob);
		var tangents = new NativeArray<Vector3>(splinePath.Tangents.ToArray(), Allocator.TempJob);
		var positions = new NativeArray<Vector3>(splinePath.Points.ToArray(), Allocator.TempJob);
		var leftScale = new NativeArray<float>(leftCurveScales, Allocator.TempJob);
		var rightScale = new NativeArray<float>(rightCurveScales, Allocator.TempJob);

		var numTris = (2 * (positions.Length - 1)) + (bezierSpline.IsLoop ? 2 : 1);
		var indicesResult = new NativeArray<int>(numTris * 3, Allocator.TempJob);
		var uvsResult = new NativeArray<Vector2>(positions.Length * 2, Allocator.TempJob);
		var vertsResult = new NativeArray<Vector3>(positions.Length * 2, Allocator.TempJob);
		var normalsResult = new NativeArray<Vector3>(positions.Length * 2, Allocator.TempJob);

		GenerateMeshJob generateMeshJob = new GenerateMeshJob()
		{
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
			leftScales = leftScale,
			rightScales = rightScale,

			//Output data
			indicesResult = indicesResult,
			uvsResult = uvsResult,
			vertsResult = vertsResult,
			normalsResult = normalsResult,
		};

		return generateMeshJob;
	}

	private void StartJob(GenerateMeshJob generateMeshJob, SplineMesh splineMesh, Mesh mesh, Action<Mesh> onMeshGenerated, bool immediate) 
	{
		if (immediate) {
			generateMeshJob.ScheduleAndComplete(generateMeshJob.positions.Length, JobBatchSize,
				(generateMeshJob) =>
				{
					OnJobCompleted(ref generateMeshJob, mesh);
					onMeshGenerated?.Invoke(mesh);
					isJobScheduled = false;
				});
		} else {
			StartJobCoroutine(generateMeshJob, splineMesh, mesh, onMeshGenerated);
		}
	}

	private void StartJobCoroutine(GenerateMeshJob generateMeshJob, SplineMesh splineMesh, Mesh mesh, Action<Mesh> onMeshGenerated) 
	{
#if UNITY_EDITOR
		generateMeshCoroutine = generateMeshJob.ScheduleAndCompleteEditorAsync(generateMeshJob.positions.Length, JobBatchSize, splineMesh,
			(generateMeshJob) =>
			{
				OnJobCompleted(ref generateMeshJob, mesh);
				onMeshGenerated?.Invoke(mesh);
				isJobScheduled = false;

				if (scheduleNextJob) {
					splineMesh.GenerateMesh();
				}
			});
#else
		generateMeshCoroutine = generateMeshJob.ScheduleAndCompleteAsync(generateMeshJob.positions.Length, JobBatchSize, splineMesh,
			(generateMeshJob) =>
			{
				OnJobCompleted(ref generateMeshJob, mesh);
				onMeshGenerated?.Invoke(mesh);
				isJobScheduled = false;

				if (scheduleNextJob) {
					splineMesh.GenerateMesh();
				}
			});
#endif
	}

	private void OnJobCompleted(ref GenerateMeshJob generateMeshJob, Mesh mesh)
	{
		mesh.Clear();
		mesh.SetVertices(generateMeshJob.vertsResult);
		mesh.SetNormals(generateMeshJob.normalsResult);
		mesh.SetUVs(0, generateMeshJob.uvsResult);
		mesh.SetTriangles(generateMeshJob.indicesResult.ToArray(), 0);
	}

	[BurstCompile(CompileSynchronously = true)]
	public struct GenerateMeshJob : IDisposableJobParallelFor
	{
		private static readonly int[] indicesMap = { 0, 2, 1, 1, 2, 3 };

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
		[ReadOnly]
		public NativeArray<float> leftScales;
		[ReadOnly]
		public NativeArray<float> rightScales;

		[WriteOnly, NativeDisableParallelForRestriction]
		public NativeArray<int> indicesResult;
		[WriteOnly, NativeDisableParallelForRestriction]
		public NativeArray<Vector2> uvsResult;
		[WriteOnly, NativeDisableParallelForRestriction]
		public NativeArray<Vector3> vertsResult;
		[WriteOnly, NativeDisableParallelForRestriction]
		public NativeArray<Vector3> normalsResult;

		public void Dispose()
		{
			scales.Dispose();
			normals.Dispose();
			tangents.Dispose();
			positions.Dispose();
			leftScales.Dispose();
			rightScales.Dispose();

			indicesResult.Dispose();
			uvsResult.Dispose();
			vertsResult.Dispose();
			normalsResult.Dispose();
		}

		public void Execute(int i) 
		{
			var vertIndex = i * 2;
			var trisIndex = i * 6;

			//Normals
			normalsResult[vertIndex] = normals[i];
			normalsResult[vertIndex + 1] = normals[i];

			//Vertices
			var right = Vector3.Cross(normals[i], tangents[i]).normalized;
			var rightScaledWidth = width * (usePointsScale ? scales[i].x : 1) * rightScales[i];
			var leftScaledWidth = width * (usePointsScale ? scales[i].x : 1) * leftScales[i];
			vertsResult[vertIndex] = positions[i] - (right * (useAsymetricWidthCurve ? leftScaledWidth : rightScaledWidth));
			vertsResult[vertIndex + 1] = positions[i] + (right * rightScaledWidth);

			//UV
			var uv = GetUV(i);
			uvsResult[vertIndex] = new Vector2(0, uv);
			uvsResult[vertIndex + 1] = new Vector2(1, uv);

			//Triangles
			if (i < positions.Length - 1 || isLoop) 
			{
				for (int j = 0; j < indicesMap.Length; j++) 
				{
					indicesResult[trisIndex + j] = (vertIndex + indicesMap[j]) % vertsResult.Length;
				}
			}
		}

		private float GetUV(int pointIndex) 
		{
			var uv = pointIndex / (float)(positions.Length - 1);
			switch (uvMode) 
			{
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
}
