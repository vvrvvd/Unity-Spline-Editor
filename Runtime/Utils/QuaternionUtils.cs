using UnityEngine;

namespace SplineEditor
{

	public static class QuaternionUtils
	{

		/// <summary>
		/// Returns signed angle between two quaterion on given axis.
		/// </summary>
		/// <remarks>
		/// See: https://answers.unity.com/questions/599393/angles-from-quaternionvector-problem.html
		/// </remarks>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="axis"></param>
		/// <returns></returns>
		public static float GetSignedAngle(Quaternion A, Quaternion B, Vector3 axis)
		{
			var angle = 0f;
			var angleAxis = Vector3.zero;
			(B * Quaternion.Inverse(A)).ToAngleAxis(out angle, out angleAxis);
			if (Vector3.Angle(axis, angleAxis) > 90f)
			{
				angle = -angle;
			}
			return Mathf.DeltaAngle(0f, angle);
		}

	}

}

