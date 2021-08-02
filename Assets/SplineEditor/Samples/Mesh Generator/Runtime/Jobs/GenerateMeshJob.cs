// <copyright file="GenerateMeshJob.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Unity.Burst;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Job for generating flat mesh from given spline path settings.
/// </summary>
[BurstCompile(CompileSynchronously = true)]
public struct GenerateMeshJob : IDisposableJobParallelFor
{
	/// <summary>
	/// Width of the generated mesh.
	/// </summary>
	[ReadOnly]
	public float Width;

	/// <summary>
	/// Should individual points scales be applied to mesh width.
	/// </summary>
	[ReadOnly]
	public bool UsePointsScale;

	/// <summary>
	/// Should both width curves (left and right) be used for mesh curve width.
	/// If false then the right side curve is applied to both sides.
	/// </summary>
	[ReadOnly]
	public bool UseAsymetricWidthCurve;

	/// <summary>
	/// How UV should be generated.
	/// 0 - Linear, 1 - PingPong.
	/// </summary>
	[ReadOnly]
	public int UvMode; // 0 - Linear, 1 - PingPong

	/// <summary>
	/// Should UV be mirrored.
	/// </summary>
	[ReadOnly]
	public bool MirrorUv;

	/// <summary>
	/// Is mesh looped (the first and the last points have the same position).
	/// </summary>
	[ReadOnly]
	public bool IsLoop;

	/// <summary>
	/// Source scales for spline path points.
	/// </summary>
	[ReadOnly]
	public NativeArray<Vector3> Scales;

	/// <summary>
	/// Source normals for spline path points.
	/// </summary>
	[ReadOnly]
	public NativeArray<Vector3> Normals;

	/// <summary>
	/// Source tangents for spline path points.
	/// </summary>
	[ReadOnly]
	public NativeArray<Vector3> Tangents;

	/// <summary>
	/// Source positions for spline path points.
	/// </summary>
	[ReadOnly]
	public NativeArray<Vector3> Positions;

	/// <summary>
	/// Source left side curve scales for spline path points.
	/// </summary>
	[ReadOnly]
	public NativeArray<float> LeftScales;

	/// <summary>
	/// Source right side curve scales for spline path points.
	/// </summary>
	[ReadOnly]
	public NativeArray<float> RightScales;

	/// <summary>
	/// Calculated indices for the generated mesh.
	/// </summary>
	[WriteOnly, NativeDisableParallelForRestriction]
	public NativeArray<int> IndicesResult;

	/// <summary>
	/// Calculated UVs for the generated mesh.
	/// </summary>
	[WriteOnly, NativeDisableParallelForRestriction]
	public NativeArray<Vector2> UvsResult;

	/// <summary>
	/// Calculated vertices for the generated mesh.
	/// </summary>
	[WriteOnly, NativeDisableParallelForRestriction]
	public NativeArray<Vector3> VertsResult;

	/// <summary>
	/// Calculated normals for the generated mesh.
	/// </summary>
	[WriteOnly, NativeDisableParallelForRestriction]
	public NativeArray<Vector3> NormalsResult;

	private static readonly int[] IndicesMap = { 0, 2, 1, 1, 2, 3 };

	/// <summary>
	/// Executes mesh generation for given index.
	/// </summary>
	/// <param name="i">Index of the point to generate mesh from.</param>
	public void Execute(int i)
	{
		var vertIndex = i * 2;
		var trisIndex = i * 6;

		// Normals
		NormalsResult[vertIndex] = Normals[i];
		NormalsResult[vertIndex + 1] = Normals[i];

		// Vertices
		var right = Vector3.Cross(Normals[i], Tangents[i]).normalized;
		var rightScaledWidth = Width * (UsePointsScale ? Scales[i].x : 1) * RightScales[i];
		var leftScaledWidth = Width * (UsePointsScale ? Scales[i].x : 1) * LeftScales[i];
		VertsResult[vertIndex] = Positions[i] - (right * (UseAsymetricWidthCurve ? leftScaledWidth : rightScaledWidth));
		VertsResult[vertIndex + 1] = Positions[i] + (right * rightScaledWidth);

		// UV
		var uv = GetUV(i);
		UvsResult[vertIndex] = new Vector2(0, uv);
		UvsResult[vertIndex + 1] = new Vector2(1, uv);

		// Triangles
		if (i < Positions.Length - 1 || IsLoop)
		{
			for (int j = 0; j < IndicesMap.Length; j++)
			{
				IndicesResult[trisIndex + j] = (vertIndex + IndicesMap[j]) % VertsResult.Length;
			}
		}
	}

	/// <summary>
	/// Disposes of all the NativeArray allocated for this job.
	/// </summary>
	public void Dispose()
	{
		Scales.Dispose();
		Normals.Dispose();
		Tangents.Dispose();
		Positions.Dispose();
		LeftScales.Dispose();
		RightScales.Dispose();

		IndicesResult.Dispose();
		UvsResult.Dispose();
		VertsResult.Dispose();
		NormalsResult.Dispose();
	}

	private float GetUV(int pointIndex)
	{
		var uv = pointIndex / (float)(Positions.Length - 1);
		switch (UvMode)
		{
			case 1: // PingPong
				uv = 1 - Mathf.Abs((2 * uv) - 1);
				break;
			case 0: // Linear
			default:
				break;
		}

		return MirrorUv ? 1 - uv : uv;
	}
}