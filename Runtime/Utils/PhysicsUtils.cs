// <copyright file="PhysicsUtils.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Utility class used for general physics calculations.
	/// </summary>
	public static class PhysicsUtils
	{
		/// <summary>
		/// Calculates casted point in the direction and returns if the point exists.
		/// </summary>
		/// <param name="point">Raycast origin position.</param>
		/// <param name="direction">Raycast direction position.</param>
		/// <param name="castedPoint">Reference to casted point Vector.</param>
		/// <returns>If a casted point was found or not.</returns>
		public static bool TryCastPoint(Vector3 point, Vector3 direction, out Vector3 castedPoint)
		{
			var isCorrectPosition = Physics.Raycast(point, direction, out var hit, Mathf.Infinity, Physics.AllLayers);

			castedPoint = isCorrectPosition ? hit.point : point;
			return isCorrectPosition;
		}
	}
}
