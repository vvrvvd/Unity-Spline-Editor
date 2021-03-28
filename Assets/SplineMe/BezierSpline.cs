using System.Collections.Generic;
using UnityEngine;

namespace SplineMe
{

    public class BezierSpline : Polyline
    {

		public int CurveCount => (PointsCount - 1) / 3;

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
			var i = 0;
			if (t >= 1f)
			{
				t = 1f;
				i = PointsCount - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return transform.TransformPoint(GetPoint(Points[i].position, Points[i+1].position, Points[i+2].position, Points[i+3].position, t));
		}

		public Vector3 GetVelocity(float t)
		{
			var i = 0;
			if (t >= 1f)
			{
				t = 1f;
				i = PointsCount - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return transform.TransformPoint(GetFirstDerivative(Points[i].position, Points[i+1].position, Points[i+2].position, Points[i+3].position, t)) - transform.position;
		}
		
		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}
		public void AddCurve()
		{
			var point = Points[PointsCount - 1].position;
			point.x += 1f;
			AddPoint(point);
			point.x += 1f;
			AddPoint(point);
			point.x += 1f;
			AddPoint(point);
		}

		public void RemoveCurve(int curveIndex)
		{
			var startCurveIndex = curveIndex * 3;
			RemovePoint(startCurveIndex + 3);
			RemovePoint(startCurveIndex + 2);
			RemovePoint(startCurveIndex + 1);
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