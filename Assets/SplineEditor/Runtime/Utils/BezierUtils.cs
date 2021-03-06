// <copyright file="BezierUtils.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Utility class for general Bezier Curve calculations.
	/// </summary>
	public static class BezierUtils
	{
		/// <summary>
		/// Returns coordinates for point on a cubic bezier curve with given control points and t.
		/// </summary>
		/// <param name="p0">The first control point position for cubic bezier curve.</param>
		/// <param name="p1">The second control point position for a cubic bezier curve.</param>
		/// <param name="p2">The third control point position for a cubic bezier curve.</param>
		/// <param name="p3">The fourth control point position for a cubic bezier curve.</param>
		/// <param name="t">Parameter for point on a cubic bezier curve.</param>
		/// <returns>Point position on a cubic bezier curve for given parameters.</returns>
		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			var oneMinusT = 1f - t;
			return
				(oneMinusT * oneMinusT * oneMinusT * p0) +
				(3f * oneMinusT * oneMinusT * t * p1) +
				(3f * oneMinusT * t * t * p2) +
				(t * t * t * p3);
		}

		/// <summary>
		/// Returns the first derivative of a cubic bezier curve for given control points and t.
		/// </summary>
		/// <param name="p0">The first control point position for cubic bezier curve.</param>
		/// <param name="p1">The second control point position for a cubic bezier curve.</param>
		/// <param name="p2">The third control point position for a cubic bezier curve.</param>
		/// <param name="p3">The fourth control point position for a cubic bezier curve.</param>
		/// <param name="t">Parameter for point on a cubic bezier curve.</param>
		/// <returns>The first derivative on a cubic bezier curve for given parameters.</returns>
		public static Vector3 GetTheFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				(3f * oneMinusT * oneMinusT * (p1 - p0)) +
				(6f * oneMinusT * t * (p2 - p1)) +
				(3f * t * t * (p3 - p2));
		}

		/// <summary>
		/// Cubic bezier curve length based on mid point quadratic approximation.
		/// </summary>
		/// <param name="p0">The first control point position for cubic bezier curve.</param>
		/// <param name="p1">The second control point position for a cubic bezier curve.</param>
		/// <param name="p2">The third control point position for a cubic bezier curve.</param>
		/// <param name="p3">The fourth control point position for a cubic bezier curve.</param>
		/// <returns>Total cubic bezier curve length using mid point quadratic approximation.</returns>
		public static float GetCubicLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			var midPoint = GetPoint(p0, p1, p2, p3, 0.5f);
			return GetQuadraticLength(p0, p1, midPoint) + GetQuadraticLength(midPoint, p2, p3);
		}

		/// <summary>
		/// Quadratic Bezier Curve Length.
		/// <remarks>
		/// Integral calculation by Dave Eberly.
		/// See: http://www.gamedev.net/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
		/// </remarks>
		/// </summary>
		/// <param name="p0">The first control point position for a quadratic bezier curve.</param>
		/// <param name="p1">The second control point position for a quadratic bezier curve.</param>
		/// <param name="p2">The third control point position for a quadratic bezier curve.</param>
		/// <returns>Total quadratic bezier curve length using integrals.</returns>
		public static float GetQuadraticLength(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			if (p0 == p2)
			{
				if (p0 == p1)
				{
					return 0.0f;
				}

				return (p0 - p1).magnitude;
			}

			if (p1 == p0 || p1 == p2)
			{
				return (p0 - p2).magnitude;
			}

			var a0 = p1 - p0;
			var a1 = p0 - (2.0f * p1) + p2;
			if (Mathf.Approximately(a1.x, 0.0f) && Mathf.Approximately(a1.y, 0.0f) && Mathf.Approximately(a1.z, 0.0f))
			{
				var c = 4.0f * Vector3.Dot(a1, a1);
				var b = 8.0f * Vector3.Dot(a0, a1);
				var a = 4.0f * Vector3.Dot(a0, a0);
				var q = (4.0f * a * c) - (b * b);
				var twoCpB = (2.0f * c) + b;
				var sumCBA = c + b + a;
				var l0 = 0.25f / c * ((twoCpB * Mathf.Sqrt(sumCBA)) - (b * Mathf.Sqrt(a)));
				if (Mathf.Approximately(q, 0.0f))
				{
					return l0;
				}

				var l1 = q / (8.0f * Mathf.Pow(c, 1.5f)) * (Mathf.Log((2.0f * Mathf.Sqrt(c * sumCBA)) + twoCpB) - Mathf.Log((2.0f * Mathf.Sqrt(c * a)) + b));
				return l0 + l1;
			}
			else
			{
				return 2.0f * a0.magnitude;
			}
		}

		/// <summary>
		/// Returns coordinates of p1 for given control points and point on a cubic curve and t.
		/// </summary>
		/// <param name="p0">The first control point position for a cubic bezier curve.</param>
		/// <param name="p2">The third control point position for a cubic bezier curve.</param>
		/// <param name="p3">The fourth control point position for a cubic bezier curve.</param>
		/// <param name="pointOnCurve">Point on curve used for calculating the second control point on cubic bezier curve.</param>
		/// <param name="t">Parameter for point on a cubic bezier curve.</param>
		/// <returns>The second control point position for a cubic bezier curve.</returns>
		public static Vector3 GetInverseCubicPointP1(Vector3 p0, Vector3 p2, Vector3 p3, Vector3 pointOnCurve, float t)
		{
			t = Mathf.Clamp01(t);
			var oneMinusT = 1f - t;
			return
				(pointOnCurve -
				(oneMinusT * oneMinusT * oneMinusT * p0) -
				(3f * oneMinusT * t * t * p2) -
				(t * t * t * p3)) /
				(3f * oneMinusT * oneMinusT * t);
		}

		/// <summary>
		/// Returns coordinates of p2 for given control points and point on curve and t.
		/// </summary>
		/// <param name="p0">The first control point position for a cubic bezier curve.</param>
		/// <param name="p1">The second control point position for a cubic bezier curve.</param>
		/// <param name="p3">The fourth control point position for a cubic bezier curve.</param>
		/// <param name="pointOnCurve">Point on curve used for calculating the third control point on cubic bezier curve.</param>
		/// <param name="t">Parameter for point on a cubic bezier curve.</param>
		/// <returns>The third control point position for a cubic bezier curve.</returns>
		public static Vector3 GetInverseCubicPointP2(Vector3 p0, Vector3 p1, Vector3 p3, Vector3 pointOnCurve, float t)
		{
			t = Mathf.Clamp01(t);
			var oneMinusT = 1f - t;
			return
				(pointOnCurve -
				(oneMinusT * oneMinusT * oneMinusT * p0) -
				(3f * oneMinusT * oneMinusT * t * p1) -
				(t * t * t * p3)) /
				(3f * oneMinusT * t * t);
		}

		/// <summary>
		/// Calculates controls points p1 and p2 for given p0, p3 and two points on curve with given u, v for them.
		/// </summary>
		/// <remarks>
		/// See: https://www.ijser.org/researchpaper/INVERSE-POINT-SOLUTION-OF-BEZIER-CURVE.pdf.
		/// </remarks>
		/// <param name="p0">The first control point for a cubic curve.</param>
		/// <param name="p3">The fourth control point for a cubic curve.</param>
		/// <param name="f">Point on the cubic curve for given u value.</param>
		/// <param name="g">Point on the cubic curve for given v value.</param>
		/// <param name="u">value of t parameter for given point f on the cubic curve.</param>
		/// <param name="v">value of t parameter for given point g on the cubic curve.</param>
		/// <param name="p1">The second control point for a cubic bezier curve.</param>
		/// <param name="p2">The third control point for a cubic bezier curve.</param>
		public static void GetInverseControlPoints(Vector3 p0, Vector3 p3, Vector3 f, Vector3 g, float u, float v, out Vector3 p1, out Vector3 p2)
		{
			p1 = Vector3.zero;
			p2 = Vector3.zero;

			var oneMinusU = 1f - u;
			var c =
				f -
				(oneMinusU * oneMinusU * oneMinusU * p0) -
				(u * u * u * p3);

			var oneMinusV = 1f - v;
			var d =
				g -
				(oneMinusV * oneMinusV * oneMinusV * p0) -
				(v * v * v * p3);

			var det =
					(3f * oneMinusU * oneMinusU * u * 3f * oneMinusV * v * v) -
					(3f * oneMinusU * u * u * 3f * oneMinusV * oneMinusV * v);

			var m0 = (3f * oneMinusV * v * v) / det;
			var m1 = (-3f * oneMinusU * u * u) / det;
			var m2 = (-3f * oneMinusV * oneMinusV * v) / det;
			var m3 = (3f * oneMinusU * oneMinusU * u) / det;

			var a = new float[,]
			{
				{ m0, m1 },
				{ m2, m3 }

				// | m0 m1 |
				// | m2 m3 |
			};

			var b = new float[,]
			{
				{ c.x, c.y, c.z },
				{ d.x, d.y, d.z }

				// | c.x d.x |
				// | c.y d.y |
				// | c.z d.z |
			};

			var solution = MultiplyMatrices(a, b);
			p1.x = solution[0, 0];
			p1.y = solution[0, 1];
			p1.z = solution[0, 2];

			p2.x = solution[1, 0];
			p2.y = solution[1, 1];
			p2.z = solution[1, 2];
		}

		private static float[,] MultiplyMatrices(float[,] a, float[,] b)
		{
			int m = a.GetLength(0);
			int n = a.GetLength(1);
			int p = b.GetLength(0);
			int q = b.GetLength(1);

			if (n != p)
			{
				Debug.LogError($"Matrix multiplication not possible due to sizes not matching {n} != {p}");
			}

			float[,] c = new float[m, q];

			for (var i = 0; i < m; i++)
			{
				for (var j = 0; j < q; j++)
				{
					c[i, j] = 0;
					for (int k = 0; k < n; k++)
					{
						c[i, j] += a[i, k] * b[k, j];
					}
				}
			}

			return c;
		}
	}
}
