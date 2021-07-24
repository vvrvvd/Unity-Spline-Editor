// <copyright file="QuaternionUtils.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Utility class used for general Quaternion calculations.
	/// </summary>
	public static class QuaternionUtils
	{
		/// <summary>
		/// Returns signed angle between two quaterion on given axis.
		/// </summary>
		/// <remarks>
		/// See: https://answers.unity.com/questions/599393/angles-from-quaternionvector-problem.html.
		/// </remarks>
		/// <param name="a">The first quaterion.</param>
		/// <param name="b">The second quaternion.</param>
		/// <param name="axis">Rotation axis between quaterions used for signed angle calculation.</param>
		/// <returns>Signed angle between given quaternions on the given axis.</returns>
		public static float GetSignedAngle(Quaternion a, Quaternion b, Vector3 axis)
		{
			var angle = 0f;
			var angleAxis = Vector3.zero;
			(b * Quaternion.Inverse(a)).ToAngleAxis(out angle, out angleAxis);
			if (Vector3.Angle(axis, angleAxis) > 90f)
			{
				angle = -angle;
			}

			return Mathf.DeltaAngle(0f, angle);
		}
	}
}