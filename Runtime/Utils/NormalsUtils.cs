using UnityEngine;

namespace SplineEditor
{

	public static class NormalsUtils {

		/// <summary>
		/// Calculates normal based on rotation axis, two points and two tangents on curve
		/// </summary>
		/// <param name="rotationAxis"></param>
		/// <param name="prevPoint"></param>
		/// <param name="currentPoint"></param>
		/// <param name="prevTan"></param>
		/// <param name="currenTan"></param>
		/// <returns></returns>
		public static Vector3 CalculateNormal(ref Vector3 rotationAxis, Vector3 prevPoint, Vector3 currentPoint, Vector3 prevTan, Vector3 currenTan)
		{
			// First reflection
			var offset = (currentPoint - prevPoint);
			var sqrDst = offset.sqrMagnitude;
			var rot = rotationAxis - offset * 2 / sqrDst * Vector3.Dot(offset, rotationAxis);
			var tan = prevTan - offset * 2 / sqrDst * Vector3.Dot(offset, prevTan);

			// Second reflection
			var v2 = currenTan - tan;
			var c2 = Vector3.Dot(v2, v2);

			var finalRot = rot - v2 * 2 / c2 * Vector3.Dot(v2, rot);
			var n = Vector3.Cross(finalRot, currenTan).normalized;
			rotationAxis = finalRot;
			return n;
		}

	}

}

