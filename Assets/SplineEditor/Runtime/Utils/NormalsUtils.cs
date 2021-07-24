// <copyright file="NormalsUtils.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Utility class used for general normal vectors calculations.
	/// </summary>
	public static class NormalsUtils
	{
		/// <summary>
		/// Calculates normal based on rotation axis, two points and two tangents on curve.
		/// </summary>
		/// <param name="rotationAxis">Reference to the rotation axis that is used for normals calculations.</param>
		/// <param name="prevPoint">Previous point position.</param>
		/// <param name="currentPoint">Current point position.</param>
		/// <param name="prevTan">Previous tangent at point.</param>
		/// <param name="currenTan">Current tangent at point.</param>
		/// <returns>Normal vector calculated based on given parameters.</returns>
		public static Vector3 CalculateNormal(ref Vector3 rotationAxis, Vector3 prevPoint, Vector3 currentPoint, Vector3 prevTan, Vector3 currenTan)
		{
			// First reflection
			var offset = currentPoint - prevPoint;
			var sqrDst = offset.sqrMagnitude;
			var rot = rotationAxis - (offset * 2 / sqrDst * Vector3.Dot(offset, rotationAxis));
			var tan = prevTan - (offset * 2 / sqrDst * Vector3.Dot(offset, prevTan));

			// Second reflection
			var v2 = currenTan - tan;
			var c2 = Vector3.Dot(v2, v2);

			var finalRot = rot - (v2 * 2 / c2 * Vector3.Dot(v2, rot));
			var n = Vector3.Cross(finalRot, currenTan).normalized;
			rotationAxis = finalRot;
			return n;
		}
	}
}