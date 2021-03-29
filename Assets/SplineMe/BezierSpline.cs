using System;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMe
{

	public enum BezierControlPointMode : int
	{
		Free = 0,
		Aligned = 1,
		Mirrored = 2
	}

	[Serializable]
	public class SplinePoint
	{
		public Vector3 position;

		public SplinePoint(Vector3 position)
		{
			this.position = position;
		}
	}

	public class BezierSpline : MonoBehaviour
    {

		public int PointsCount => points.Count;
		public List<SplinePoint> Points => points;

		[SerializeField]
		protected List<SplinePoint> points;

		[SerializeField]
		private bool isLoop;

		[SerializeField]
		private List<BezierControlPointMode> modes;

		public int CurveCount => (PointsCount - 1) / 3;

		private bool isRemovingCurve = false;

		public bool IsLoop
		{
			get
			{
				return isLoop;
			}
			set
			{
				isLoop = value;
				if (value == true)
				{
					modes[modes.Count - 1] = modes[0];
					UpdatePoint(0, points[0].position);
				}
			}
		}

		#region Getters & Setters

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

		public BezierControlPointMode GetControlPointMode(int index)
		{
			return modes[(index + 1) / 3];
		}

		public void SetControlPointMode(int index, BezierControlPointMode mode)
		{
			var modeIndex = (index + 1) / 3;
			modes[modeIndex] = mode;

			if (isLoop)
			{
				if (modeIndex == 0)
				{
					modes[modes.Count - 1] = mode;
				}
				else if (modeIndex == modes.Count - 1)
				{
					modes[0] = mode;
				}
			}

			ApplyContraints(index);
		}

		private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * oneMinusT * p0 +
				3f * oneMinusT * oneMinusT * t * p1 +
				3f * oneMinusT * t * t * p2 +
				t * t * t * p3;
		}

		private static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				3f * oneMinusT * oneMinusT * (p1 - p0) +
				6f * oneMinusT * t * (p2 - p1) +
				3f * t * t * (p3 - p2);
		}

		#endregion

		private void Reset()
		{
			modes = new List<BezierControlPointMode>();
			points = new List<SplinePoint>();

			var p0 = new Vector3(1f, 0f, 0f);
			var p1 = new Vector3(2f, 0f, 0f);
			var p2 = new Vector3(3f, 0f, 0f);
			var p3 = new Vector3(4f, 0f, 0f);

			AddPoint(p0);
			AddPoint(p1);
			AddPoint(p2);
			AddPoint(p3);

			modes.Add(BezierControlPointMode.Free);
			modes.Add(BezierControlPointMode.Free);
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

			var prevMode = modes[modes.Count - 1];
			modes.Add(prevMode);
			ApplyContraints(PointsCount - 4);

			if (IsLoop)
			{
				Points[PointsCount - 1].position = Points[0].position;
				modes[modes.Count - 1] = modes[0];
				ApplyContraints(0);
			}
		}

		public void RemoveCurve(int curveIndex)
		{
			var wasRemovingCurve = isRemovingCurve;
			isRemovingCurve = true;
			var isLastCurve = curveIndex == CurveCount;
			var isStartCurve = curveIndex == 0;
			var isMidCurve = IsLoop && curveIndex == 1 && CurveCount == 2;

			if (!wasRemovingCurve && IsLoop && isStartCurve)
			{
				RemoveCurve(CurveCount);
			}

			var beginCurveIndex = curveIndex * 3;
			var startCurveIndex = beginCurveIndex;
			if (isStartCurve)
			{
				startCurveIndex += 1;
			} else if(isLastCurve)
			{
				startCurveIndex -= 1;
			} else if(isMidCurve)
			{
				startCurveIndex += 2;
			}

			RemovePoint(startCurveIndex + 1);
			RemovePoint(startCurveIndex);
			RemovePoint(startCurveIndex - 1);

			var modeIndex = (beginCurveIndex + 2) / 3;
			modes.RemoveAt(modeIndex);

			if (!wasRemovingCurve && IsLoop && isLastCurve)
			{
				RemoveCurve(0);
			}

			if (wasRemovingCurve)
			{
				return;
			}

			var nextPointIndex = isLastCurve ? PointsCount - 1 : startCurveIndex;

			var wasLoop = IsLoop;
			if (IsLoop && CurveCount==1)
			{
				IsLoop = false;
			}

			UpdatePoint(nextPointIndex, Points[nextPointIndex].position);
			isRemovingCurve = false;
		}

		public void UpdatePoint(int index, Vector3 position)
		{
			if (index % 3 == 0)
			{
				var delta = position - Points[index].position;
				if (IsLoop)
				{
					if (index == 0)
					{
						Points[1].position += delta;
						Points[PointsCount - 2].position += delta;
						Points[PointsCount - 1].position = position;
					}
					else if (index == PointsCount - 1)
					{
						Points[0].position = position;
						Points[1].position += delta;
						Points[index - 1].position += delta;
					}
					else
					{
						Points[index - 1].position += delta;
						Points[index + 1].position += delta;
					}
				}
				else
				{
					if (index > 0)
					{
						Points[index - 1].position += delta;
					}
					if (index + 1 < PointsCount)
					{
						Points[index + 1].position += delta;
					}
				}
			}

			Points[index].position = position;
			
			ApplyContraints(index);
		}

		private void ApplyContraints(int index)
		{
			var modeIndex = (index + 1) / 3;
			var mode = modes[modeIndex];
			if (mode == BezierControlPointMode.Free || (!IsLoop && (modeIndex == 0 || modeIndex == modes.Count - 1)))
			{
				return;
			}

			var middleIndex = modeIndex * 3;
			int fixedIndex, enforcedIndex;
			if (index <= middleIndex)
			{
				fixedIndex = middleIndex - 1;
				if (fixedIndex < 0)
				{
					fixedIndex = PointsCount - 2;
				}
				enforcedIndex = middleIndex + 1;
				if (enforcedIndex >= PointsCount)
				{
					enforcedIndex = 1;
				}
			}
			else
			{
				fixedIndex = middleIndex + 1;
				if (fixedIndex >= PointsCount)
				{
					fixedIndex = 1;
				}
				enforcedIndex = middleIndex - 1;
				if (enforcedIndex < 0)
				{
					enforcedIndex = PointsCount - 2;
				}
			}

			if (enforcedIndex==PointsCount || fixedIndex==PointsCount)
			{
				return;
			}

			var middle = Points[middleIndex].position;
			var enforcedTangent = middle - Points[fixedIndex].position;

			if (mode == BezierControlPointMode.Aligned)
			{
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, Points[enforcedIndex].position);
			}

			Points[enforcedIndex].position = middle + enforcedTangent;
		}

		public void AddPoint(Vector3 point)
		{
			var nextIndex = PointsCount > 0 ? PointsCount : 0;
			AddPoint(point, nextIndex);
		}

		public void AddPoint(Vector3 point, int index)
		{
			var linePoint = new SplinePoint(point);
			Points.Insert(index, linePoint);
		}

		public void RemovePoint(int index)
		{
			Points.RemoveAt(index);
		}

	}

}