// <copyright file="VectorUtils.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Utility class used for general Vector calculations.
	/// </summary>
	public static class VectorUtils
	{
		/// <summary>
		/// Rotates target point arount pivot point by given rotation.
		/// </summary>
		/// <param name="targetPoint">Position to rotated.</param>
		/// <param name="pivotPoint">Pivot position to be rotated around.</param>
		/// <param name="rotation">Quaternion to rotate target point around pivot point.</param>
		/// <returns>Target point position rotated around pivot point position by given quaternion rotation.</returns>
		public static Vector3 RotateAround(Vector3 targetPoint, Vector3 pivotPoint, Quaternion rotation)
		{
			return (rotation * (targetPoint - pivotPoint)) + pivotPoint;
		}

		/// <summary>
		/// Returns inversed lerp value t for given start, end and lerped point positions.
		/// </summary>
		/// <param name="startPoint">Start position.</param>
		/// <param name="endPoint">End position.</param>
		/// <param name="lerpedPoint">Lerped position between start and end positions.</param>
		/// <returns>Value of parameter t that'd give lerped point position between start point and end point positions (inversed lerp).</returns>
		public static float InverseLerp(Vector3 startPoint, Vector3 endPoint, Vector3 lerpedPoint)
		{
			Vector3 aB = endPoint - startPoint;
			Vector3 aV = lerpedPoint - startPoint;
			return Vector3.Dot(aV, aB) / Vector3.Dot(aB, aB);
		}
	}
}
