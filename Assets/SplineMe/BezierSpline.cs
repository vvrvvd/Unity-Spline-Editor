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

		/// <summary>
		/// Number of curves in the splines.
		/// </summary>
		public int CurvesCount => Mathf.Max(0 ,(PointsCount - 1) / 3);

		/// <summary>
		/// Number of points in the spline.
		/// </summary>
		public int PointsCount => points.Count;

		/// <summary>
		/// List of all the spline points.
		/// </summary>
		public List<SplinePoint> Points => points;

		/// <summary>
		/// Returns if the spline is looped.
		/// If true then the first and the last point are considered the same point.
		/// </summary>
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

		/// <summary>
		/// Returns the entire spline length using quadratic curve approximation for every cubic spline.
		/// </summary>
		public float Length
		{
			get
			{
				var lengthSum = 0f;
				var curveCount = CurvesCount;
				for (var i = 0; i < curveCount; i++)
				{
					lengthSum += BezierUtils.GetCubicLength(points[i * 3].position, points[i * 3 + 1].position, points[i * 3 + 2].position, points[i * 3 + 3].position);
				}

				return lengthSum;
			}
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

		#region Getters & Setters

		/// <summary>
		/// Calculates the spline point position at given t.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="useWorldSpace">Transform point from local space to world space.</param>
		/// <returns></returns>
		public Vector3 GetPoint(float t, bool useWorldSpace = true)
		{
			int i;
			if (t >= 1f)
			{
				t = 1f;
				i = PointsCount - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurvesCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			var localSpacePosition = BezierUtils.GetPoint(Points[i].position, Points[i + 1].position, Points[i + 2].position, Points[i + 3].position, t);
			return useWorldSpace ? transform.TransformPoint(localSpacePosition) : localSpacePosition;
		}

		/// <summary>
		/// Calculates the spline velocity point (the first derivative) at given t.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="useWorldSpace">Transform point from local space to world space.</param>
		/// <returns></returns>
		public Vector3 GetVelocity(float t, bool useWorldSpace = true)
		{
			int i;
			if (t >= 1f)
			{
				t = 1f;
				i = PointsCount - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurvesCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			var localSpacePosition = BezierUtils.GetTheFirstDerivative(Points[i].position, Points[i + 1].position, Points[i + 2].position, Points[i + 3].position, t);
			return useWorldSpace ? transform.TransformPoint(localSpacePosition) - transform.position : localSpacePosition;
		}

		/// <summary>
		/// Calculates the spline direction (normalized velocity) at given t.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="useWorldSpace">Transform point from local space to world space.</param>
		/// <returns></returns>
		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t, false).normalized;
		}

		/// <summary>
		/// Returns control point mode for given point index.
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <returns></returns>
		public BezierControlPointMode GetControlPointMode(int pointIndex)
		{
			return modes[(pointIndex + 1) / 3];
		}

		/// <summary>
		/// Sets control point mode for given point index.
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <param name="mode"></param>
		public void SetControlPointMode(int pointIndex, BezierControlPointMode mode)
		{
			var modeIndex = (pointIndex + 1) / 3;
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

			ApplyContraints(pointIndex);
		}

		#endregion

		#region Tools

		/// <summary>
		/// Update point position at given index.
		/// </summary>
		/// <param name="pointIndex">Point index.</param>
		/// <param name="position">New local point position.</param>
		/// <param name="applyConstraints">Updates connected control points based on BezierControlPointMode for point at given index.</param>
		/// <param name="updateAttachedSidePoints">If the control point at pointIndex is starting or ending curve point (pointIndex % 3 == 0) and this value is set to true then control points attached to this point will also updated.</param>
		public void UpdatePoint(int pointIndex, Vector3 position, bool applyConstraints = true, bool updateAttachedSidePoints = true)
		{
			if (updateAttachedSidePoints && pointIndex % 3 == 0)
			{
				var delta = position - Points[pointIndex].position;
				if (IsLoop)
				{
					if (pointIndex == 0)
					{
						Points[1].position += delta;
						Points[PointsCount - 2].position += delta;
						Points[PointsCount - 1].position = position;
					}
					else if (pointIndex == PointsCount - 1)
					{
						Points[0].position = position;
						Points[1].position += delta;
						Points[pointIndex - 1].position += delta;
					}
					else
					{
						Points[pointIndex - 1].position += delta;
						Points[pointIndex + 1].position += delta;
					}
				}
				else
				{
					if (pointIndex > 0)
					{
						Points[pointIndex - 1].position += delta;
					}
					if (pointIndex + 1 < PointsCount)
					{
						Points[pointIndex + 1].position += delta;
					}
				}
			}

			Points[pointIndex].position = position;

			if (applyConstraints)
			{
				ApplyContraints(pointIndex);
			}
		}

		/// <summary>
		/// Appends new curve with given control points and control point mode.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="mode"></param
		/// <param name="addAtBeginning">Should new curve be added at the beginning of spline.</param>
		public void AppendCurve(Vector3 p1, Vector3 p2, Vector3 p3, BezierControlPointMode mode, bool addAtBeginning = true)
		{
			if(!addAtBeginning || IsLoop)
			{
				//Add at the end of the spline
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
			else
			{
				//Add at the beginning of the spline
				AddPoint(p1, 0);
				AddPoint(p2, 0);
				AddPoint(p3, 0);

				modes.Insert(0, mode);
				ApplyContraints(3);
			}

		}

		/// <summary>
		/// Removes curve at given index.
		/// </summary>
		/// <param name="curveIndex"></param>
		public void RemoveCurve(int curveIndex)
		{
			var isLastCurve = (isLoop && curveIndex == CurvesCount) || (!isLoop && curveIndex == CurvesCount - 1);
			var isStartCurve = curveIndex == 0;
			var isMidCurve = IsLoop && curveIndex == 1 && CurvesCount == 2;
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

			if (!isLastCurve || !IsLoop)
			{
				RemovePoint(startCurveIndex - 1);
				var modeIndex = (beginCurveIndex + 2) / 3;
				modes.RemoveAt(modeIndex);
			}

			var nextPointIndex = (isLastCurve || startCurveIndex >= PointsCount) ? PointsCount - 1 : startCurveIndex;

			if (IsLoop && CurvesCount == 1)
			{
				IsLoop = false;
			}

			if (IsLoop)
			{
				UpdatePoint(0, points[0].position);
			}

			UpdatePoint(nextPointIndex, Points[nextPointIndex].position);
		}

		/// <summary>
		/// Factors the spline by adding mid curve points for every curve.
		/// </summary>
		public void FactorSpline()
		{
			for (var i = 0; i < CurvesCount; i += 2)
			{
				InsertCurve(i, 0.5f);
			}
		}

		/// <summary>
		/// Simplifies the spline by removing every second curve.
		/// </summary>
		public void SimplifySpline()
		{
			for (var i = 1; i < CurvesCount; i++)
			{
				RemoveCurveAndRecalculateControlPoints(i);
				if (i == CurvesCount - 1)
				{
					return;
				}
			}
		}

		/// <summary>
		/// Inserts a new curve by adding it at t point of curve at given index.
		/// </summary>
		/// <param name="curveIndex"></param>
		public void InsertCurve(int curveIndex, float t)
		{
			t = Mathf.Clamp01(t);
			if(t == 0f || t == 1f)
			{
				return;
			}

			var startPointIndex = curveIndex * 3;

			SetControlPointMode(startPointIndex, BezierControlPointMode.Free);
			SetControlPointMode(startPointIndex + 3, BezierControlPointMode.Free);

			var p0 = points[startPointIndex].position;

			var newT = (curveIndex + t) / CurvesCount;
			var newPoint = GetPoint(newT, false);

			newT = (curveIndex + 0.4f * t) / CurvesCount;
			var pointOnCurve1 = GetPoint(newT, false);

			newT = (curveIndex + 0.8f * t) / CurvesCount;
			var pointOnCurve2 = GetPoint(newT, false);

			//Left control point
			BezierUtils.GetInverseControlPoints(p0, newPoint, pointOnCurve1, pointOnCurve2, 0.4f, 0.8f, out var p1, out var p2);
			var leftControlPoint = p2;
			var updatedP1 = p1;

			var p3 = points[startPointIndex + 3].position;

			newT = (curveIndex + t + (1f - t)*0.4f) / CurvesCount;
			pointOnCurve1 = GetPoint(newT, false);

			newT = (curveIndex + t + (1f - t) * 0.8f) / CurvesCount;
			pointOnCurve2 = GetPoint(newT, false);

			//Right control point
			BezierUtils.GetInverseControlPoints(newPoint, p3, pointOnCurve1, pointOnCurve2, 0.4f, 0.8f, out p1, out p2);

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

		#endregion

		#endregion

		#region Private Methods

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

		private void RemovePoint(int pointIndex)
		{
			Points.RemoveAt(pointIndex);
		}

		private void ApplyContraints(int pointIndex)
		{
			var modeIndex = (pointIndex + 1) / 3;
			var mode = modes[modeIndex];
			if (mode == BezierControlPointMode.Free || (!IsLoop && (modeIndex == 0 || modeIndex == modes.Count - 1)))
			{
				return;
			}

			var middleIndex = modeIndex * 3;
			int fixedIndex, enforcedIndex;
			if (pointIndex <= middleIndex)
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

		private void RemoveCurveAndRecalculateControlPoints(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;
			var p0Index = startPointIndex - 3;
			var p3Index = startPointIndex + 3;

			var p0 = points[p0Index].position;
			var p3 = points[p3Index].position;

			var t = (curveIndex - 0.5f) / CurvesCount;
			var pointOnCurve1 = GetPoint(t, false);

			t = (curveIndex + 0.5f) / CurvesCount;
			var pointOnCurve2 = GetPoint(t, false);

			BezierUtils.GetInverseControlPoints(p0, p3, pointOnCurve1, pointOnCurve2, 0.25f, 0.75f, out var p1, out var p2);
			SetControlPointMode(p0Index, BezierControlPointMode.Free);
			SetControlPointMode(p3Index, BezierControlPointMode.Free);
			UpdatePoint(p0Index + 1, p1);
			UpdatePoint(p3Index - 1, p2);

			RemoveCurve(curveIndex);
		}

		#endregion

	}

}