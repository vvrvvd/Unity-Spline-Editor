using UnityEngine;

namespace SplineEditor
{

	public static class PhysicsUtils
	{

		/// <summary>
		/// Calculates casted point in the direction and returns if the point exists.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="direction"></param>
		/// <param name="castedPoint"></param>
		/// <returns></returns>
		public static bool TryCastPoint(Vector3 point, Vector3 direction, out Vector3 castedPoint)
		{
			var isCorrectPosition = Physics.Raycast(point, direction, out var hit, Mathf.Infinity, Physics.AllLayers);

			castedPoint = isCorrectPosition ? hit.point : point;
			return isCorrectPosition;
		}

	}

}

