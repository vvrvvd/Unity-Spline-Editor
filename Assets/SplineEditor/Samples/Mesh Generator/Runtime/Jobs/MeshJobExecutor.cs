using SplineEditor;
using SplineEditor.MeshGenerator;
using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

public class MeshJobExecutor
{

	private const int JobBatchSize = 64;
	private const float SplinePathPrecision = 0.001f;

	public bool isJobScheduled = false;
	private bool scheduleNextJob = false;

	private Coroutine generateMeshCoroutine;
	private MonoBehaviour coroutineContext;

	private SplineMesh splineMesh;
	private SplinePath splinePath;

	public MeshJobExecutor(SplineMesh splineMesh, SplinePath splinePath) 
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
		if (coroutineContext==null || generateMeshCoroutine == null) 
		{
			return;
		}

		coroutineContext.StopCoroutine(generateMeshCoroutine);
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
	}

	private void OnJobCompleted(ref GenerateMeshJob generateMeshJob, Mesh mesh)
	{
		mesh.Clear();
		mesh.SetVertices(generateMeshJob.vertsResult);
		mesh.SetNormals(generateMeshJob.normalsResult);
		mesh.SetUVs(0, generateMeshJob.uvsResult);
		mesh.SetTriangles(generateMeshJob.indicesResult.ToArray(), 0);
	}
}
