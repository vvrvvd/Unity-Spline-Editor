using System;
using System.Collections.Generic;
using UnityEngine;

namespace SplineEditor
{

	[DisallowMultipleComponent]
	public class BezierSpline : MonoBehaviour
	{

		#region Enums

		public enum BezierControlPointMode : int
		{
			Free = 0,
			Aligned = 1,
			Mirrored = 2,
			Auto = 3
		}

		#endregion

		#region Events

		public event Action OnSplineChanged;

		#endregion

		#region Public Fields

		[HideInInspector]
		public bool drawPoints = true;
		[HideInInspector]
		public bool drawDirections = false;
		[HideInInspector]
		public bool showTransformHandle = true;
		[HideInInspector]
		public bool alwaysDrawSplineOnScene = true;

		#endregion

		#region Editor Fields

		[SerializeField]
		private bool isLoop = default;

		[SerializeField]
		protected List<SplinePoint> points = default;

		[SerializeField]
		private List<BezierControlPointMode> modes = default;

		#endregion

		#region Private Fields

		private bool invokeEvents = true;

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
		/// Returns the spline length using line iteration approximation.
		/// </summary>
		/// <param name="useWorldScale"></param>
		/// <returns></returns>
		public float GetLinearLength(bool useWorldScale = true)
		{
			var lengthSum = 0f;
			var iterationsCount = 1000;
			var t = 0f;
			var prevPoint = GetPoint(t, useWorldScale);
			for (var i = 1; i < iterationsCount; i++)
			{
				t += (float)i / iterationsCount;
				var nextPoint = GetPoint(t, useWorldScale);
				lengthSum += Vector3.Distance(prevPoint, nextPoint);
				prevPoint = nextPoint;
			}

			return lengthSum;
		}

		/// <summary>
		/// Returns the spline length using quadratic curve approximation for every cubic spline.
		/// </summary>
		/// <param name="useWorldScale"></param>
		/// <returns></returns>
		public float GetQuadraticLength(bool useWorldScale = true)
		{

			var lengthSum = 0f;
			var curveCount = CurvesCount;

			for (var i = 0; i < curveCount; i++)
			{
				var p0 = points[i * 3].position;
				var p1 = points[i * 3 + 1].position;
				var p2 = points[i * 3 + 2].position;
				var p3 = points[i * 3 + 3].position;

				if(useWorldScale)
				{
					p0 = transform.TransformPoint(p0);
					p1 = transform.TransformPoint(p1);
					p2 = transform.TransformPoint(p2);
					p3 = transform.TransformPoint(p3);
				}

				lengthSum += BezierUtils.GetCubicLength(p0, p1, p2, p3);
			}

			return lengthSum;
		}

		/// <summary>
		/// If spline is looping then returns next index taking into account the loop 
		/// e.g. -1 index in looping spline is considered as index at PointsCount - 1 
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <returns></returns>
		public int GetLoopingIndex(int pointIndex)
		{

			if((pointIndex < 0 || pointIndex >= points.Count))
			{
				if(isLoop)
				{
					return pointIndex < 0 ? points.Count - 1 + pointIndex : pointIndex - points.Count - 1;
				}
				else
				{
					return -1;
				}
			}

			return pointIndex;
		}

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
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

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

			invokeEvents = prevInvokeEvents;
			if(invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		public Vector3[] GetEvenlySpacedPoints(int segmentsCount, float precision = 0.001f, bool useWorldSpace = true)
		{
			var results = new Vector3[segmentsCount];
			GetEvenlySpacedPointsNonAlloc(segmentsCount, results, precision, useWorldSpace);

			return results;
		}

		public void GetEvenlySpacedPointsNonAlloc(int segmentsCount, Vector3[] results, float precision = 0.001f, bool useWorldSpace = true)
		{
			if(segmentsCount == 0 || results == null || results.Length==0)
			{
				return;
			}

			var splineLength = GetLinearLength(false);
			var segmentLength = splineLength / segmentsCount;

			var t = precision;
			var prevPoint = points[0].position;
			results[0] = prevPoint;
			var pointsProcessed = 1;
			for(var i=1; i< segmentsCount && i<results.Length; i++)
			{

				var nextPoint = GetPoint(t, false);
				var distance = Vector3.Distance(prevPoint, nextPoint);
				while (distance < segmentLength && t < 1f)
				{
					t += precision;
					prevPoint = nextPoint;
					nextPoint = GetPoint(t, false);
					distance += Vector3.Distance(prevPoint, nextPoint);
				}

				var alpha = segmentLength / distance;
				nextPoint = Vector3.Lerp(prevPoint, nextPoint, alpha);

				if (t >= 1f)
				{
					t -= precision;
				}

				results[i] = nextPoint;
				prevPoint = nextPoint;
				pointsProcessed++;
			}


			if (!IsLoop && results.Length > segmentsCount)
			{
				results[segmentsCount] = GetPoint(1f, false);
				pointsProcessed++;
			}

			if (useWorldSpace)
			{

				for (var i = 0; i < pointsProcessed; i++)
				{
					results[i] = transform.TransformPoint(results[i]);
				}
			}
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
			var pointMode = GetControlPointMode(pointIndex);
			if (pointIndex % 3 != 0 && pointMode == BezierControlPointMode.Auto)
			{
				return;
			}

			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

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

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
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
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			if (!addAtBeginning || IsLoop)
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

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Removes curve at given index.
		/// </summary>
		/// <param name="curveIndex"></param>
		public void RemoveCurve(int curveIndex, bool removeFirstPoints)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			var isLastCurve = !removeFirstPoints && ((isLoop && curveIndex == CurvesCount) || (!isLoop && curveIndex == CurvesCount - 1));
			var isStartCurve = removeFirstPoints && curveIndex == 0;
			var isMidCurve = (IsLoop && curveIndex == 1 && CurvesCount == 2);
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
			} else if(!removeFirstPoints)
			{
				startCurveIndex += 3;
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

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Loops spline by adding closing path curve.
		/// If beginning and ending points are in the same position then path isn't created and spline is simply looped.
		/// </summary>
		public void ToggleCloseLoop()
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;
			if (IsLoop)
			{
				IsLoop = false;
				RemoveCurve(CurvesCount - 1, false);
			}
			else { 

				if (points[0].position != points[PointsCount-1].position)
				{
					var p0 = points[PointsCount - 1].position;
					var p1 = p0 - (points[PointsCount - 2].position - p0);
					var p3 = points[0].position;
					var p2 = p3 - (points[1].position - p3);

					AppendCurve(p1, p2, p3, modes[0], false);
				}

				IsLoop = true;
			}


			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Factors the spline by adding mid curve points for every curve.
		/// </summary>
		public void FactorSpline()
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			for (var i = 0; i < CurvesCount; i += 2)
			{
				InsertCurve(i, 0.5f);
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Simplifies the spline by removing every second curve.
		/// </summary>
		public void SimplifySpline()
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			for (var i = 1; i < CurvesCount; i++)
			{
				RemoveCurveAndRecalculateControlPoints(i);
				if (i >= CurvesCount - 1)
				{
					i = CurvesCount;
				}
			}

			ApplyAutoSetToAllControlPoints();

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
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

			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			var startPointIndex = curveIndex * 3;

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
			var prevMode = modes[modeIndex-1];
			modes.Insert(modeIndex, prevMode);

			ApplyAutoSetToAllControlPoints();

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Sets all control points on spline to selected mode
		/// </summary>
		/// <param name="mode"></param>
		public void SetAllControlPointsMode(BezierControlPointMode mode)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			for (var i = 0; i < PointsCount; i+=3)
			{
				SetControlPointMode(i, mode);
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
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
			var curveIndex = (pointIndex + 1) / 3;
			var mode = modes[curveIndex];

			if ((mode == BezierControlPointMode.Free || mode == BezierControlPointMode.Auto) || (!IsLoop && (curveIndex == 0 || curveIndex == modes.Count - 1)))
			{
				ApplyAffectedAutoSetControlPoints(pointIndex);
				return;
			}

			var middleIndex = curveIndex * 3;
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
			ApplyAffectedAutoSetControlPoints(pointIndex);
		}

		private void ApplyAutoSetToAllControlPoints()
		{
			for(var i=0; i < PointsCount; i+=3)
			{
				ApplyAffectedAutoSetControlPoints(i);
			}
		}

		private void ApplyAffectedAutoSetControlPoints(int pointIndex)
		{
			var curveIndex = (pointIndex + 1) / 3;
			var mode = modes[curveIndex];
			var curvesCount = CurvesCount;

			var applyAutoConstraintsToNextNeighbour = curveIndex < curvesCount - 1 && modes[curveIndex + 1] == BezierControlPointMode.Auto;
			var applyAutoConstraintsToPreviousNeighbour = curveIndex > 0 && modes[curveIndex - 1] == BezierControlPointMode.Auto;

			if (applyAutoConstraintsToNextNeighbour)
			{
				ApplyAutoSetControlPoints(curveIndex + 1);
			}

			if (applyAutoConstraintsToPreviousNeighbour)
			{
				ApplyAutoSetControlPoints(curveIndex - 1);
			}

			if (mode == BezierControlPointMode.Auto)
			{
				ApplyAutoSetControlPoints(curveIndex);
			}

			ApplyAutoSetControlPointsToEdgePoints();
		}

		private void ApplyAutoSetControlPoints(int curveIndex)
		{
			if(modes[curveIndex]!=BezierControlPointMode.Auto)
			{
				return;
			}

			var startPointIndex = curveIndex * 3;
			var startPointPosition = Points[startPointIndex].position;
			var previousPointPosition = startPointPosition;
			var nextPointPosition = startPointPosition;

			if(curveIndex > 0 || isLoop)
			{
				var loopedIndex = GetLoopingIndex(startPointIndex - 3);
				previousPointPosition = Points[loopedIndex].position;
			} 

			if(curveIndex < CurvesCount || isLoop)
			{
				var loopedIndex = GetLoopingIndex(startPointIndex + 3);
				nextPointPosition = Points[loopedIndex].position;
			}

			var prevDistance = (previousPointPosition - startPointPosition);
			var nextDistance = (nextPointPosition - startPointPosition);
			var dir = (prevDistance.normalized - nextDistance.normalized).normalized;

			var prevControlPointIndex = GetLoopingIndex(startPointIndex - 1);
			var nextControlPointIndex = GetLoopingIndex(startPointIndex + 1);

			if(prevControlPointIndex != -1)
			{
				var updatedPosition = Points[startPointIndex].position + dir * prevDistance.magnitude * 0.5f;
				Points[prevControlPointIndex].position = updatedPosition;
			}

			if(nextControlPointIndex != -1)
			{
				var updatedPosition = Points[startPointIndex].position - dir * nextDistance.magnitude * 0.5f;
				Points[nextControlPointIndex].position = updatedPosition;
			}

		}

		private void ApplyAutoSetControlPointsToEdgePoints()
		{
			if (isLoop)
			{
				if (CurvesCount == 2 && modes[0] == BezierControlPointMode.Auto && modes[1] == BezierControlPointMode.Auto)
				{
					Vector3 dirAnchorAToB = (points[3].position - points[0].position).normalized;
					float dstBetweenAnchors = (points[0].position - points[3].position).magnitude;
					Vector3 perp = Vector3.Cross(dirAnchorAToB, Vector3.up);
					points[1].position = points[0].position + perp * dstBetweenAnchors / 2f;
					points[5].position = points[0].position - perp * dstBetweenAnchors / 2f;
					points[2].position = points[3].position + perp * dstBetweenAnchors / 2f;
					points[4].position = points[3].position - perp * dstBetweenAnchors / 2f;
				}
				else
				{
					ApplyAutoSetControlPoints(0);
					ApplyAutoSetControlPoints(CurvesCount-1);
				}
			}
			else
			{
				if(modes[0] == BezierControlPointMode.Auto)
				{
					points[1].position = (points[0].position + points[2].position) * 0.5f;
				}

				if(modes[modes.Count-1] == BezierControlPointMode.Auto)
				{
					points[points.Count - 2].position = (points[points.Count - 1].position + points[points.Count - 3].position) * 0.5f;
				}
			}
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
			UpdatePoint(p0Index + 1, p1);
			UpdatePoint(p3Index - 1, p2);

			RemoveCurve(curveIndex, true);
		}

		#endregion

	}

}