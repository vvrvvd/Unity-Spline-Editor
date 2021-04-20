using UnityEngine;
using System.Collections.Generic;

namespace SplineMe
{

	[DisallowMultipleComponent]
	public class BezierSpline : MonoBehaviour
	{

		#region Enums

		public enum BezierControlPointMode : int
		{
			Free = 0,
			Aligned = 1,
			Mirrored = 2
		}

		#endregion

		#region Editor Fields

		[SerializeField]
		private bool isLoop = default;

		[SerializeField]
		protected List<SplinePoint> points = default;

		[SerializeField]
		private List<BezierControlPointMode> modes = default;

		#endregion

		#region Properties

		public int CurveCount => Mathf.Max(0 ,(PointsCount - 1) / 3);
		public int PointsCount => points.Count;
		public List<SplinePoint> Points => points;

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

		public float Length
		{
			get
			{
				var lengthSum = 0f;
				var curveCount = CurveCount;
				for (var i = 0; i < curveCount; i++)
				{
					lengthSum += BezierUtils.GetCubicLength(points[i * 3].position, points[i * 3 + 1].position, points[i * 3 + 2].position, points[i * 3 + 3].position);
				}

				return lengthSum;
			}
		}

		#endregion

		#region Getters & Setters

		public Vector3 GetPoint(float t)
		{
			int i;
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

			return transform.TransformPoint(BezierUtils.GetPoint(Points[i].position, Points[i + 1].position, Points[i + 2].position, Points[i + 3].position, t));
		}

		public Vector3 GetVelocity(float t)
		{
			int i;
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

			return transform.TransformPoint(BezierUtils.GetFirstDerivative(Points[i].position, Points[i + 1].position, Points[i + 2].position, Points[i + 3].position, t)) - transform.position;
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

		#endregion

		#region Initialize

		private void Reset()
		{
			modes = new List<BezierControlPointMode>();
			points = new List<SplinePoint>(1000);

			var p0 = new Vector3(1f, 0f, 0f);
			var p1 = new Vector3(1.5f, 0f, 0f);
			var p2 = new Vector3(3.5f, 0f, 0f);
			var p3 = new Vector3(4f, 0f, 0f);

			AddPoint(p0);
			AddPoint(p1);
			AddPoint(p2);
			AddPoint(p3);

			modes.Add(BezierControlPointMode.Free);
			modes.Add(BezierControlPointMode.Free);
		}

		#endregion

		#region Public Methods

		public void AddCurve(float segmentLength = 1f)
		{
			var deltaDir = (Points[PointsCount - 1].position - Points[PointsCount - 2].position).normalized * segmentLength / 3;
			var p1 = Points[PointsCount - 1].position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = modes[modes.Count - 1];
			AddCurve(p1, p2, p3, prevMode);
		}

		public void AddCurve(Vector3 p1, Vector3 p2, Vector3 p3, BezierControlPointMode mode)
		{
			AddPoint(p1);
			AddPoint(p2);
			AddPoint(p3);

			modes.Add(mode);
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
			RemoveCurve(curveIndex, false);
		}

		public void UpdatePoint(int index, Vector3 position, bool applyConstraints = true, bool applyToSidePoints = true)
		{
			if (applyToSidePoints && index % 3 == 0)
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

			if (applyConstraints)
			{
				ApplyContraints(index);
			}
		}

		public void CastCurve()
		{
			for (var i = 0; i < modes.Count; i++)
			{
				modes[i] = BezierControlPointMode.Free;
			}

			var newPointsPositions = new Vector3[PointsCount];
			for (var i = 0; i < PointsCount; i++)
			{
				TryCastPoint(i, -transform.up, out newPointsPositions[i]);
			}

			for (var i = 0; i < PointsCount; i += 3)
			{
				var prevPoint = i > 0 ? points[i - 1].position : Vector3.zero;
				var nextPoint = i < PointsCount - 1 ? points[i + 1].position : Vector3.zero;

				UpdatePoint(i, newPointsPositions[i], false, true);

				var isPreviousPointCasted = i > 0 && newPointsPositions[i - 1] != prevPoint;
				if (isPreviousPointCasted)
				{
					UpdatePoint(i - 1, newPointsPositions[i - 1], false, false);
				}

				var isNextPointCasted = i < PointsCount - 1 && newPointsPositions[i + 1] != nextPoint;
				if (isNextPointCasted)
				{
					UpdatePoint(i + 1, newPointsPositions[i + 1], false, false);
				}
			}

		}

		public bool TryCastPoint(int index, Vector3 direction, out Vector3 castedPoint)
		{
			var point = Points[index];
			var worldPosition = transform.TransformPoint(point.position);
			var isCorrectPosition = Physics.Raycast(worldPosition, direction, out var hit, Mathf.Infinity, Physics.AllLayers);

			castedPoint = isCorrectPosition ? transform.InverseTransformPoint(hit.point) : point.position;
			return isCorrectPosition;
		}

		public void AddMidCurve(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;

			var p0 = points[startPointIndex].position;
			var p1 = points[startPointIndex + 1].position;

			var t = (curveIndex + 0.5f) / CurveCount;
			var newPoint = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.25f) / CurveCount;
			var pointOnCurve = transform.InverseTransformPoint(GetPoint(t));

			//Left control point
			var leftControlPoint = BezierUtils.GetInverseCubicPointP2(p0, p1, newPoint, pointOnCurve, 0.5f);

			p0 = newPoint;
			var p2 = points[startPointIndex + 2].position;
			var p3 = points[startPointIndex + 3].position;

			t = (curveIndex + 0.75f) / CurveCount;
			pointOnCurve = transform.InverseTransformPoint(GetPoint(t));

			//Right control point
			var rightControlPoint = BezierUtils.GetInverseCubicPointP1(newPoint, p2, p3, pointOnCurve, 0.5f);

			AddPoint(leftControlPoint, startPointIndex + 2);
			AddPoint(newPoint, startPointIndex + 3);
			AddPoint(rightControlPoint, startPointIndex + 4);

			var modeIndex = (startPointIndex + 3) / 3;
			modes.Insert(modeIndex, BezierControlPointMode.Free);
		}

		public void FactorCurve()
		{
			for (var i = 0; i < CurveCount; i += 2)
			{
				AddMidCurveAndApplyConstraints(i);
			}
		}

		public void SimplifyCurve()
		{
			for (var i = 1; i < CurveCount; i++)
			{
				RemoveCurveAndApplyConstraints(i);
				if (i == CurveCount - 1)
				{
					return;
				}
			}
		}

		public void AddMidCurveAndApplyConstraints(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;

			SetControlPointMode(startPointIndex, BezierControlPointMode.Free);
			SetControlPointMode(startPointIndex + 3, BezierControlPointMode.Free);

			var p0 = points[startPointIndex].position;

			var t = (curveIndex + 0.5f) / CurveCount;
			var newPoint = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.16f) / CurveCount;
			var pointOnCurve1 = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.33f) / CurveCount;
			var pointOnCurve2 = transform.InverseTransformPoint(GetPoint(t));

			//Left control point
			BezierUtils.GetInverseControlPoints(p0, newPoint, pointOnCurve1, pointOnCurve2, 0.32f, 0.66f, out var p1, out var p2);
			var leftControlPoint = p2;
			var updatedP1 = p1;

			var p3 = points[startPointIndex + 3].position;

			t = (curveIndex + 0.66f) / CurveCount;
			pointOnCurve1 = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.83f) / CurveCount;
			pointOnCurve2 = transform.InverseTransformPoint(GetPoint(t));

			//Right control point
			BezierUtils.GetInverseControlPoints(newPoint, p3, pointOnCurve1, pointOnCurve2, 0.32f, 0.66f, out p1, out p2);

			var rightControlPoint = p1;
			var updatedP2 = p2;

			UpdatePoint(startPointIndex + 2, updatedP2);
			UpdatePoint(startPointIndex + 1, updatedP1);

			AddPoint(leftControlPoint, startPointIndex + 2);
			AddPoint(newPoint, startPointIndex + 3);
			AddPoint(rightControlPoint, startPointIndex + 4);

			var modeIndex = (startPointIndex + 3) / 3;
			modes.Insert(modeIndex, BezierControlPointMode.Free);
		}

		public void RemoveCurveAndApplyConstraints(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;
			var p0Index = startPointIndex - 3;
			var p3Index = startPointIndex + 3;

			var p0 = points[p0Index].position;
			var p3 = points[p3Index].position;

			var t = (curveIndex - 0.5f) / CurveCount;
			var pointOnCurve1 = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.5f) / CurveCount;
			var pointOnCurve2 = transform.InverseTransformPoint(GetPoint(t));

			BezierUtils.GetInverseControlPoints(p0, p3, pointOnCurve1, pointOnCurve2, 0.25f, 0.75f, out var p1, out var p2);
			SetControlPointMode(p0Index, BezierControlPointMode.Free);
			SetControlPointMode(p3Index, BezierControlPointMode.Free);
			UpdatePoint(p0Index + 1, p1);
			UpdatePoint(p3Index - 1, p2);

			RemoveCurve(curveIndex);
		}

		#endregion

		#region Private Methods

		private void RemoveCurve(int curveIndex, bool isRecursiveCall)
		{
			var wasRemovingCurve = isRecursiveCall;
			var isLastCurve = curveIndex == CurveCount;
			var isStartCurve = curveIndex == 0;
			var isMidCurve = IsLoop && curveIndex == 1 && CurveCount == 2;

			if (!wasRemovingCurve && IsLoop && isStartCurve)
			{
				RemoveCurve(CurveCount, true);
			}

			var beginCurveIndex = curveIndex * 3;
			var startCurveIndex = beginCurveIndex;
			if (isStartCurve)
			{
				startCurveIndex += 1;
			}
			else if (isLastCurve)
			{
				startCurveIndex = PointsCount - 2;
			}
			else if (isMidCurve)
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
				RemoveCurve(0, true);
			}

			if (wasRemovingCurve)
			{
				return;
			}

			var nextPointIndex = (isLastCurve || startCurveIndex >= PointsCount) ? PointsCount - 1 : startCurveIndex;

			if (IsLoop && CurveCount == 1)
			{
				IsLoop = false;
			}

			if (IsLoop)
			{
				UpdatePoint(0, Points[0].position);
			}

			UpdatePoint(nextPointIndex, Points[nextPointIndex].position);
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

			if (enforcedIndex == PointsCount || fixedIndex == PointsCount)
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

		private void AddPoint(Vector3 point)
		{
			var nextIndex = PointsCount > 0 ? PointsCount : 0;
			AddPoint(point, nextIndex);
		}

		private void AddPoint(Vector3 point, int index)
		{
			var linePoint = new SplinePoint(point);
			if (index != PointsCount)
			{
				Points.Insert(index, linePoint);
			}
			else
			{
				Points.Add(linePoint);
			}
		}

		private void RemovePoint(int index)
		{
			Points.RemoveAt(index);
		}

		#endregion

	}

}