// <copyright file="VectorUtils.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// 
	/// </summary>
	public static class VectorUtils
	{

		/// <summary>
		/// Rotates target point arount pivot point by given rotation.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="pivotPoint"></param>
		/// <param name="rot"></param>
		/// <returns></returns>
		public static Vector3 RotateAround(Vector3 target, Vector3 pivotPoint, Quaternion rot)
		{
			return (rot * (target - pivotPoint)) + pivotPoint;
		}

		/// <summary>
		/// Returns inversed lerp value t for given start, end and lerped point positions.
		/// </summary>
		/// <param name="startPoint"></param>
		/// <param name="endPoint"></param>
		/// <param name="lerpedPoint"></param>
		/// <returns></returns>
		public static float InverseLerp(Vector3 startPoint, Vector3 endPoint, Vector3 lerpedPoint)
		{
			Vector3 aB = endPoint - startPoint;
			Vector3 aV = lerpedPoint - startPoint;
			return Vector3.Dot(aV, aB) / Vector3.Dot(aB, aB);
		}
	}
}
