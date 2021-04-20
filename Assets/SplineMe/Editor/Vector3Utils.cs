using UnityEngine;

public static class Vector3Utils
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
		return rot * (target - pivotPoint) + pivotPoint;
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
		Vector3 AB = endPoint - startPoint;
		Vector3 AV = lerpedPoint - startPoint;
		return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
	}

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

