// <copyright file="MeshJobExecutor.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using SplineEditor;
using SplineEditor.MeshGenerator;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Class for scheduling and executing mesh generation using Jobs system.
/// </summary>
public class MeshJobExecutor
{
	private const int JobBatchSize = 64;
	private const float SplinePathPrecision = 0.001f;

	private bool isJobScheduled = false;
	private bool scheduleNextJob = false;

	private Coroutine generateMeshCoroutine;
	private MonoBehaviour coroutineContext;

	private SplineMesh splineMesh;
	private SplinePath splinePath;

	/// <summary>
	/// Initializes a new instance of the <see cref="MeshJobExecutor"/> class.
	/// </summary>
	/// <param name="splineMesh">SplineMesh component for getting parameters for mesh generation.</param>
	/// <param name="splinePath">SplinePath object for keeping parameters for mesh generation.</param>
	public MeshJobExecutor(SplineMesh splineMesh, SplinePath splinePath)
	{
		this.splineMesh = splineMesh;
		this.splinePath = splinePath;
	}

	/// <summary>
	/// Prepares and generates new mesh using Jobs system.
	/// On mesh generation completion it is stored in the given mesh and onMeshGenerated action is invoked.
	/// </summary>
	/// <param name="mesh">Mesh to be generated.</param>
	/// <param name="onMeshGenerated">Action invoked on mesh generation completion.</param>
	/// <param name="immediate">Should mesh generation be forced to be executed in the same frame (on the main thread).</param>
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

	private void StopJobCoroutine()
	{
		if (coroutineContext == null || generateMeshCoroutine == null)
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
			// Input data
			Width = width,
			UsePointsScale = splineMesh.UsePointsScale,
			UseAsymetricWidthCurve = splineMesh.UseAsymetricWidthCurve,
			UvMode = (int)splineMesh.UvMode,
			MirrorUv = splineMesh.MirrorUV,
			IsLoop = bezierSpline.IsLoop,
			Scales = scales,
			Normals = normals,
			Tangents = tangents,
			Positions = positions,
			LeftScales = leftScale,
			RightScales = rightScale,

			// Output data
			IndicesResult = indicesResult,
			UvsResult = uvsResult,
			VertsResult = vertsResult,
			NormalsResult = normalsResult,
		};

		return generateMeshJob;
	}

	private void StartJob(GenerateMeshJob generateMeshJob, SplineMesh splineMesh, Mesh mesh, Action<Mesh> onMeshGenerated, bool immediate)
	{
		if (immediate)
		{
			generateMeshJob.ScheduleAndComplete(generateMeshJob.Positions.Length, JobBatchSize,
				(generateMeshJob) =>
				{
					OnJobCompleted(ref generateMeshJob, mesh);
					onMeshGenerated?.Invoke(mesh);
					isJobScheduled = false;
				});
		}
		else
		{
			StartJobCoroutine(generateMeshJob, splineMesh, mesh, onMeshGenerated);
		}
	}

	private void StartJobCoroutine(GenerateMeshJob generateMeshJob, SplineMesh splineMesh, Mesh mesh, Action<Mesh> onMeshGenerated)
	{
		generateMeshCoroutine = generateMeshJob.ScheduleAndCompleteAsync(generateMeshJob.Positions.Length, JobBatchSize, splineMesh,
			(generateMeshJob) =>
			{
				OnJobCompleted(ref generateMeshJob, mesh);
				onMeshGenerated?.Invoke(mesh);
				isJobScheduled = false;

				if (scheduleNextJob)
				{
					splineMesh.GenerateMesh();
				}
			});
	}

	private void OnJobCompleted(ref GenerateMeshJob generateMeshJob, Mesh mesh)
	{
		mesh.Clear();
		mesh.SetVertices(generateMeshJob.VertsResult);
		mesh.SetNormals(generateMeshJob.NormalsResult);
		mesh.SetUVs(0, generateMeshJob.UvsResult);
		mesh.SetTriangles(generateMeshJob.IndicesResult.ToArray(), 0);
	}
}
