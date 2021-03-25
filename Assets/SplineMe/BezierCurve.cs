using System.Collections.Generic;
using UnityEngine;

namespace SplineMe
{

    public class BezierCurve : Polyline
    {
		protected override void Reset()
		{
			reversedPoints = new List<LinePoint>();
			inspectorPoints = new List<LinePoint>();

			var p0 = new Vector3(1f, 0f, 0f);
			var p1 = new Vector3(2f, 0f, 0f);
			var p2 = new Vector3(3f, 0f, 0f);
			var p3 = new Vector3(4f, 0f, 0f);

			AddPoint(p0);
			AddPoint(p1);
			AddPoint(p2);
			AddPoint(p3);
		}

		public Vector3 GetPoint(float t)
		{
			return transform.TransformPoint(GetPoint(Points[0].position, Points[1].position, Points[2].position, Points[3].position, t));
		}

		public Vector3 GetVelocity(float t)
		{
			return transform.TransformPoint(GetFirstDerivative(Points[0].position, Points[1].position, Points[2].position, Points[3].position, t)) -
				transform.position;
		}
		
		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}

		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * oneMinusT * p0 +
				3f * oneMinusT * oneMinusT * t * p1 +
				3f * oneMinusT * t * t * p2 +
				t * t * t * p3;
		}

		public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				3f * oneMinusT * oneMinusT * (p1 - p0) +
				6f * oneMinusT * t * (p2 - p1) +
				3f * t * t * (p3 - p2);
		}

	}

}