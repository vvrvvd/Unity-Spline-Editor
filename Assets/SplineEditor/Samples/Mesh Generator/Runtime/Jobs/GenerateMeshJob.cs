using Unity.Burst;
using Unity.Collections;
using UnityEngine;

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