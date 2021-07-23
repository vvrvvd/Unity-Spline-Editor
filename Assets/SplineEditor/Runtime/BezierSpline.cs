// <copyright file="BezierSpline.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Component containing bezier spline definition.
	/// </summary>
	[DisallowMultipleComponent]
	public class BezierSpline : MonoBehaviour
	{
		private const float MinNormalsAnglesDifference = 0.1f;
		private const float InsertCurveSecondPointT = 0.8f;
		private const float InsertCurveFirstPointT = 0.4f;
		private const float AutoSetControlPointInterpolationValue = 0.5f;

		[SerializeField]
		private bool isLoop = default;

		[SerializeField]
		private bool flipNormals = default;

		[SerializeField]
		private float globalNormalsRotation = default;

		[SerializeField]
		private List<SplinePoint> points = default;

		[SerializeField]
		private List<Vector3> normals = default;

		[SerializeField]
		private List<Vector3> tangents = default;

		[SerializeField]
		private List<Vector3> pointsScales = default;

		[SerializeField]
		private List<BezierControlPointMode> modes = default;

		[SerializeField]
		private List<float> normalsAngularOffsets = default;

		private bool invokeEvents = true;
		private List<float> normalsOffsetCopyList = new List<float>();

		/// <summary>
		/// Event invoked when spline properties were modified in any way.
		/// </summary>
		public event Action OnSplineChanged;

		/// <summary>
		/// Bezier curve control point mode.
		/// Describes how a control point affects neighbour control points.
		/// </summary>
		public enum BezierControlPointMode : int
		{
			/// <summary>
			/// The point doesn't affect neighbour points.
			/// </summary>
			Free = 0,

			/// <summary>
			/// Neighbour point is being kept on parallel to main point but on the other side.
			/// </summary>
			Aligned = 1,

			/// <summary>
			/// Neighbour point is being kept on parallel to main point and at the same distance from main point.
			/// </summary>
			Mirrored = 2,

			/// <summary>
			/// Neighbour point is being kept based on neighbour main points.
			/// </summary>
			Auto = 3
		}

		/// <summary>
		/// Gets number of curves in the splines.
		/// </summary>
		public int CurvesCount => Mathf.Max(0, (PointsCount - 1) / 3);

		/// <summary>
		/// Gets number of points in the spline.
		/// </summary>
		public int PointsCount => points.Count;

		/// <summary>
		/// Gets list of all the spline points.
		/// </summary>
		public List<SplinePoint> Points => points;

		/// <summary>
		/// Gets calculated normals for Points.
		/// </summary>
		public List<Vector3> Normals => normals;

		/// <summary>
		/// Gets calculated tangents for Points.
		/// </summary>
		public List<Vector3> Tangents => tangents;

		/// <summary>
		/// Gets scales at points. It doesn't affect curve itself but may be used e.g. meshes generation.
		/// </summary>
		public List<Vector3> PointsScales => pointsScales;

		/// <summary>
		/// Gets manually set angular offsets for Points.
		/// </summary>
		public List<float> NormalsAngularOffsets => normalsAngularOffsets;

		/// <summary>
		/// Gets or sets a value indicating whether the spline is looped.
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
				if (value && modes.Count > 0 && pointsScales.Count > 0 && normalsAngularOffsets.Count > 0)
				{
					var prevInvokeEvents = invokeEvents;
					invokeEvents = false;

					modes[modes.Count - 1] = modes[0];
					pointsScales[pointsScales.Count - 1] = pointsScales[0];
					normalsAngularOffsets[normalsAngularOffsets.Count - 1] = normalsAngularOffsets[0];
					SetPoint(0, points[0].Position);

					if (prevInvokeEvents)
					{
						OnSplineChanged?.Invoke();
					}

					invokeEvents = prevInvokeEvents;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether normals are flipped to opposite direction.
		/// </summary>
		public bool FlipNormals
		{
			get
			{
				return flipNormals;
			}

			set
			{
				flipNormals = value;
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Gets or sets offset rotation of every normal along the spline direction axis at given point.
		/// </summary>
		public float GlobalNormalsRotation
		{
			get
			{
				return globalNormalsRotation;
			}

			set
			{
				globalNormalsRotation = value;
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// If spline is looping then returns next index taking into account the loop.
		/// e.g. -1 index in looping spline is considered as index at PointsCount - 1.
		/// </summary>
		/// <param name="pointIndex">Reference index.</param>
		/// <returns>Looped point index.</returns>
		public int GetLoopingIndex(int pointIndex)
		{
			if (pointIndex < 0 || pointIndex >= points.Count)
			{
				if (isLoop)
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
		/// Returns the spline length using line iteration approximation.
		/// </summary>
		/// <param name="targetT">Up to which t should length be calculated.</param>
		/// <param name="precision">Determines length approximation calculation accuracy.</param>
		/// <param name="useWorldScale">Should world object scale be applied.</param>
		/// <returns>Spline length up to the target t point.</returns>
		public float GetLinearLength(float targetT = 1f, float precision = 0.001f, bool useWorldScale = true)
		{
			var lengthSum = 0f;
			var iterationsCount = (int)(1f / precision);
			var t = 0f;
			var prevPoint = GetPoint(t, useWorldScale);
			for (var i = 1; i < iterationsCount; i++)
			{
				t += (float)i / iterationsCount;
				var nextPoint = GetPoint(t, useWorldScale);
				lengthSum += Vector3.Distance(prevPoint, nextPoint);
				prevPoint = nextPoint;

				if (t >= targetT)
				{
					i = iterationsCount;
				}
			}

			return lengthSum;
		}

		/// <summary>
		/// Returns the spline length using quadratic curve approximation for every cubic spline.
		/// </summary>
		/// <param name="useWorldScale">Should world object scale be applied.</param>
		/// <returns>Entire spline length.</returns>
		public float GetQuadraticLength(bool useWorldScale = true)
		{
			var lengthSum = 0f;
			var curveCount = CurvesCount;

			for (var i = 0; i < curveCount; i++)
			{
				var p0 = points[i * 3].Position;
				var p1 = points[(i * 3) + 1].Position;
				var p2 = points[(i * 3) + 2].Position;
				var p3 = points[(i * 3) + 3].Position;

				if (useWorldScale)
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
		/// Calculates the spline point position at given t.
		/// </summary>
		/// <param name="t">Target t parameter for bezier spline.</param>
		/// <param name="useWorldSpace">Transform point from local space to world space.</param>
		/// <returns>Bezier spline point position at given spline point t.</returns>
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

			var localSpacePosition = BezierUtils.GetPoint(Points[i].Position, Points[i + 1].Position, Points[i + 2].Position, Points[i + 3].Position, t);
			return useWorldSpace ? transform.TransformPoint(localSpacePosition) : localSpacePosition;
		}

		/// <summary>
		/// Updates point position at given index.
		/// </summary>
		/// <param name="pointIndex">Control point index.</param>
		/// <param name="position">New local point position.</param>
		/// <param name="applyConstraints">Updates connected control points based on BezierControlPointMode for point at given index.</param>
		/// <param name="updateAttachedSidePoints">If the control point at pointIndex is starting or ending curve point (pointIndex % 3 == 0) and this value is set to true then control points attached to this point will also updated.</param>
		public void SetPoint(int pointIndex, Vector3 position, bool applyConstraints = true, bool updateAttachedSidePoints = true)
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
				var delta = position - Points[pointIndex].Position;
				if (IsLoop)
				{
					if (pointIndex == 0)
					{
						Points[1].Position += delta;
						Points[PointsCount - 2].Position += delta;
						Points[PointsCount - 1].Position = position;
					}
					else if (pointIndex == PointsCount - 1)
					{
						Points[0].Position = position;
						Points[1].Position += delta;
						Points[pointIndex - 1].Position += delta;
					}
					else
					{
						Points[pointIndex - 1].Position += delta;
						Points[pointIndex + 1].Position += delta;
					}
				}
				else
				{
					if (pointIndex > 0)
					{
						Points[pointIndex - 1].Position += delta;
					}

					if (pointIndex + 1 < PointsCount)
					{
						Points[pointIndex + 1].Position += delta;
					}
				}
			}

			Points[pointIndex].Position = position;

			if (applyConstraints)
			{
				ApplyContraints(pointIndex);
			}

			invokeEvents = prevInvokeEvents;

			RecalculateNormals();

			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Calculates the spline velocity point (the first derivative) at given t.
		/// </summary>
		/// <param name="t">Target t parameter for bezier spline.</param>
		/// <param name="useWorldSpace">Transform point from local space to world space.</param>
		/// <returns>The first derivative at given spline point t.</returns>
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

			var localSpacePosition = BezierUtils.GetTheFirstDerivative(Points[i].Position, Points[i + 1].Position, Points[i + 2].Position, Points[i + 3].Position, t);
			return useWorldSpace ? transform.TransformPoint(localSpacePosition) - transform.position : localSpacePosition;
		}

		/// <summary>
		/// Calculates the spline direction (normalized velocity) at given t.
		/// </summary>
		/// <param name="t">Target t parameter for bezier spline.</param>
		/// <returns>Normalized first derivative at given spline point t.</returns>
		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t, false).normalized;
		}

		/// <summary>
		/// Returns interpolated points scale at point t of the spline.
		/// </summary>
		/// <param name="t">Target t parameter for bezier spline.</param>
		/// <returns>Interpolated point scale at given spline point t.</returns>
		public Vector3 GetPointScale(float t)
		{
			var curveSegmentSizeT = 1f / CurvesCount;
			var curveIndex = 0;
			while (t > curveIndex * curveSegmentSizeT)
			{
				curveIndex++;
			}

			curveIndex = Mathf.Clamp(curveIndex - 1, 0, normalsAngularOffsets.Count - 2);

			var prevPointT = curveIndex * curveSegmentSizeT;
			var nextPointT = (curveIndex + 1) * curveSegmentSizeT;
			var alpha = Mathf.InverseLerp(prevPointT, nextPointT, t);
			var interpolatedPointScale = Vector3.Lerp(pointsScales[curveIndex], pointsScales[curveIndex + 1], alpha);

			return interpolatedPointScale;
		}

		/// <summary>
		/// Updates point scale for the point at given index.
		/// </summary>
		/// <param name="mainControlPointIndex">Main control point index.</param>
		/// <param name="scale">Scale vector to be set at given index.</param>
		public void SetPointsScale(int mainControlPointIndex, Vector3 scale)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			pointsScales[mainControlPointIndex] = scale;

			if (IsLoop)
			{
				if (mainControlPointIndex == 0)
				{
					pointsScales[Normals.Count - 1] = scale;
				}
				else if (mainControlPointIndex == Normals.Count - 1)
				{
					pointsScales[0] = scale;
				}
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Returns normals vector for curveIndex point based on Normals and their angular offsets.
		/// </summary>
		/// <param name="controlPointIndex">Spline point index.</param>
		/// <returns>Normals vector with global and local normal rotation at given point index.</returns>
		public Vector3 GetNormal(int controlPointIndex)
		{
			var globalAngleOffset = GlobalNormalsRotation;
			var normalAngularOffset = normalsAngularOffsets[controlPointIndex];
			var globalRotation = Quaternion.AngleAxis(globalAngleOffset, Tangents[controlPointIndex]);
			var normalRotation = Quaternion.AngleAxis(normalAngularOffset, Tangents[controlPointIndex]);
			var normalVector = globalRotation * normalRotation * normals[controlPointIndex];

			return normalVector;
		}

		/// <summary>
		/// Rotates normal vector based on interpolated angular offset at point t of the spline.
		/// </summary>
		/// <param name="normalVector">Normal vector to rotate.</param>
		/// <param name="t">Target t parameter for bezier spline.</param>
		/// <returns>Interpolated normal vector at given point.</returns>
		public Vector3 GetRotatedNormal(Vector3 normalVector, float t)
		{
			var curveSegmentSizeT = 1f / CurvesCount;
			var curveIndex = 0;
			while (t > curveIndex * curveSegmentSizeT)
			{
				curveIndex++;
			}

			curveIndex = Mathf.Clamp(curveIndex - 1, 0, normalsAngularOffsets.Count - 2);

			var tangent = GetDirection(t);
			var prevPointT = curveIndex * curveSegmentSizeT;
			var nextPointT = (curveIndex + 1) * curveSegmentSizeT;
			var alpha = Mathf.InverseLerp(prevPointT, nextPointT, t);
			var globalAngleOffset = GlobalNormalsRotation;
			var normalAngularOffset = Mathf.Lerp(normalsAngularOffsets[curveIndex], normalsAngularOffsets[curveIndex + 1], alpha);
			var globalRotation = Quaternion.AngleAxis(globalAngleOffset, tangent);
			var normalRotation = Quaternion.AngleAxis(normalAngularOffset, tangent);
			var rotatedNormalVector = globalRotation * normalRotation * normalVector;

			return rotatedNormalVector;
		}

		/// <summary>
		/// Updates normal angular offset for a point at given index.
		/// </summary>
		/// <param name="mainControlPointIndex">Main control point index.</param>
		/// <param name="angle">Offset angle to be set at given index.</param>
		public void SetNormalAngularOffset(int mainControlPointIndex, float angle)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			normalsAngularOffsets[mainControlPointIndex] = angle;

			if (IsLoop)
			{
				if (mainControlPointIndex == 0)
				{
					normalsAngularOffsets[Normals.Count - 1] = angle;
				}
				else if (mainControlPointIndex == Normals.Count - 1)
				{
					normalsAngularOffsets[0] = angle;
				}
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Returns control point mode for given point index.
		/// </summary>
		/// <param name="pointIndex">Control point index.</param>
		/// <returns>Control point mode at given index.</returns>
		public BezierControlPointMode GetControlPointMode(int pointIndex)
		{
			return modes[(pointIndex + 1) / 3];
		}

		/// <summary>
		/// Sets control point mode for given point index.
		/// </summary>
		/// <param name="pointIndex">Control point index.</param>
		/// <param name="mode">Control point mode to be set at given index.</param>
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
			if (invokeEvents)
			{
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Sets all control points on spline to selected mode.
		/// </summary>
		/// <param name="mode">Control point mode to be set for all the points.</param>
		public void SetAllControlPointsMode(BezierControlPointMode mode)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			for (var i = 0; i < PointsCount; i += 3)
			{
				SetControlPointMode(i, mode);
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Invokes OnSplineChanged event.
		/// </summary>
		public void UpdateSpline()
		{
			if (OnSplineChanged == null)
			{
				return;
			}

			OnSplineChanged.Invoke();
		}

		/// <summary>
		/// Recalculate normals for control points.
		/// </summary>
		public void RecalculateNormals()
		{
			var curvesCount = CurvesCount;
			if (normals == null)
			{
				normals = new List<Vector3>(curvesCount + 1);
				tangents = new List<Vector3>(normals.Count);
				normalsAngularOffsets = new List<float>(normals.Count);
			}
			else if (normals.Count != curvesCount + 1)
			{
				for (var i = normals.Count; i < curvesCount + 1; i++)
				{
					normals.Add(Vector3.zero);
					tangents.Add(Vector3.zero);
					normalsAngularOffsets.Add(0);
				}
			}

			var precision = 0.0001f;
			var splineLength = GetLinearLength(precision: 0.0001f, useWorldScale: false);
			var spacing = Mathf.Max(splineLength / (curvesCount * 1000f), 0.1f);

			var normalsPath = new SplinePath();
			var globalAngleCopy = globalNormalsRotation;
			globalNormalsRotation = 0f;

			normalsOffsetCopyList.Clear();
			for (var i = 0; i < normalsAngularOffsets.Count; i++)
			{
				normalsOffsetCopyList.Add(normalsAngularOffsets[i]);
				normalsAngularOffsets[i] = 0f;
			}

			GetEvenlySpacedPoints(spacing, normalsPath, precision, false);

			var pointIndex = 1;
			var currentTargetT = pointIndex * (1f / CurvesCount);
			var distance = 0f;
			var targetDistance = GetLinearLength(targetT: currentTargetT, precision: 0.00001f, useWorldScale: false);
			normals[0] = normalsPath.Normals[0];
			Tangents[0] = normalsPath.Tangents[0];
			for (var i = 1; i < normalsPath.Points.Length; i++)
			{
				distance += Vector3.Distance(normalsPath.Points[i - 1], normalsPath.Points[i]);
				if (distance >= targetDistance)
				{
					var alpha = targetDistance / distance;
					normals[pointIndex] = Vector3.Lerp(normalsPath.Normals[i - 1], normalsPath.Normals[i], alpha);
					Tangents[pointIndex] = Vector3.Lerp(normalsPath.Tangents[i - 1], normalsPath.Tangents[i], alpha);

					pointIndex += 1;
					currentTargetT = pointIndex * (1f / CurvesCount);
					targetDistance = GetLinearLength(targetT: currentTargetT, precision: 0.0001f, useWorldScale: false);
				}

				if (currentTargetT == 1)
				{
					break;
				}
			}

			if (isLoop)
			{
				normals[normals.Count - 1] = normalsPath.Normals[0];
				Tangents[Tangents.Count - 1] = normalsPath.Tangents[0];
			}
			else
			{
				normals[normals.Count - 1] = normalsPath.Normals[normalsPath.Normals.Length - 1];
				Tangents[Tangents.Count - 1] = normalsPath.Tangents[normalsPath.Tangents.Length - 1];
			}

			globalNormalsRotation = globalAngleCopy;
			for (var i = 0; i < normalsAngularOffsets.Count; i++)
			{
				normalsAngularOffsets[i] = normalsOffsetCopyList[i];
			}
		}

		/// <summary>
		/// Calculates evenly spaced points and their respective directions (tangents) across the spline with given parameters.
		/// Points are being saved in SplinePath object.
		/// </summary>
		/// <param name="spacing">Distance between points.</param>
		/// <param name="bezierPath">Object for keeping generated points parameters.</param>
		/// <param name="precision">Precision of spline approximation used for calculating points (the lower value, the higher precision).</param>
		/// <param name="useWorldSpace">Transform points from local space to world space.</param>
		public void GetEvenlySpacedPoints(float spacing, SplinePath bezierPath, float precision = 0.001f, bool useWorldSpace = true)
		{
			var scales = new List<Vector3>();
			var normals = new List<Vector3>();
			var tangents = new List<Vector3>();
			var parametersT = new List<float>();
			var spacedPoints = new List<Vector3>();

			var splineLength = GetLinearLength(precision: 0.0001f, useWorldScale: false);
			var segmentsCount = Mathf.RoundToInt(splineLength / spacing) + 1;

			var t = precision;
			var prevPoint = points[0].Position;
			spacedPoints.Add(prevPoint);
			tangents.Add(GetDirection(0f));
			scales.Add(GetPointScale(0f));
			parametersT.Add(0f);

			var lastRotationAxis = (FlipNormals ? -1 : 1) * Vector3.forward;
			var normalVector = Vector3.Cross(lastRotationAxis, tangents[0]).normalized;
			var rotatedNormalVector = GetRotatedNormal(normalVector, 0f);
			normals.Add(rotatedNormalVector);
			for (var i = 1; i < segmentsCount || t < 1; i++)
			{
				var currentPoint = GetPoint(t, false);
				var distance = Vector3.Distance(prevPoint, currentPoint);
				t += precision;
				while (distance < spacing && t < 1f)
				{
					t += precision;
					prevPoint = currentPoint;
					currentPoint = GetPoint(t, false);
					distance += Vector3.Distance(prevPoint, currentPoint);
				}

				var alpha = spacing / distance;
				currentPoint = Vector3.Lerp(prevPoint, currentPoint, alpha);
				spacedPoints.Add(currentPoint);

				t = Mathf.Clamp01(t);
				t += (alpha - 1f) * precision;
				t = Mathf.Clamp01(t);

				scales.Add(GetPointScale(t));
				tangents.Add(GetDirection(t));
				parametersT.Add(t);

				var prevTan = tangents[tangents.Count - 2];
				var currentTan = tangents[tangents.Count - 1];
				normalVector = NormalsUtils.CalculateNormal(ref lastRotationAxis, prevPoint, currentPoint, prevTan, currentTan);
				rotatedNormalVector = GetRotatedNormal(normalVector, t);
				normals.Add(rotatedNormalVector);

				prevPoint = currentPoint;

				if (t >= 1f)
				{
					break;
				}
			}

			if (isLoop && normals.Count > 1)
			{
				// Get angle between first and last normal (if zero, they're already lined up, otherwise we need to correct)
				float normalsAngleErrorAcrossJoin = Vector3.SignedAngle(normals[normals.Count - 1], normals[0], tangents[0]);

				// Gradually rotate the normals along the path to ensure start and end normals line up correctly
				if (Mathf.Abs(normalsAngleErrorAcrossJoin) > MinNormalsAnglesDifference) // don't bother correcting if very nearly correct
				{
					for (int i = 1; i < normals.Count; i++)
					{
						float targetT = i / (normals.Count - 1f);
						float angle = normalsAngleErrorAcrossJoin * targetT;
						Quaternion rot = Quaternion.AngleAxis(angle, tangents[i]);

						normals[i] = rot * normals[i];
					}
				}
			}

			bezierPath.Scales = scales.ToArray();
			bezierPath.Points = spacedPoints.ToArray();
			bezierPath.Normals = normals.ToArray();
			bezierPath.Tangents = tangents.ToArray();
			bezierPath.ParametersT = parametersT.ToArray();

			if (useWorldSpace)
			{
				for (var i = 0; i < bezierPath.Points.Length; i++)
				{
					bezierPath.Points[i] = transform.TransformPoint(bezierPath.Points[i]);
				}
			}
		}

		/// <summary>
		/// Appends new curve with given control points and control point mode starting with the last or the first points as p1 point for this curve.
		/// </summary>
		/// <param name="p2">The second control point position.</param>
		/// <param name="p3">The third control point position.</param>
		/// <param name="p4">The fourth control point position.</param>
		/// <param name="mode">Mode to be assigned for given points p3 and p4.</param
		/// <param name="addAtBeginning">Should a new curve be added at the beginning of the spline.</param>
		public void AppendCurve(Vector3 p2, Vector3 p3, Vector3 p4, BezierControlPointMode mode, bool addAtBeginning = true)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			if (!addAtBeginning || IsLoop)
			{
				// Add at the end of the spline
				AddPoint(p2);
				AddPoint(p3);
				AddPoint(p4);

				modes.Add(mode);
				pointsScales.Add(Vector3.one);
				ApplyContraints(PointsCount - 4);

				if (IsLoop)
				{
					modes[modes.Count - 1] = modes[0];
					points[PointsCount - 1].Position = points[0].Position;
					pointsScales[pointsScales.Count - 1] = pointsScales[0];
					ApplyContraints(0);
				}
			}
			else
			{
				// Add at the beginning of the spline
				AddPoint(p2, 0);
				AddPoint(p3, 0);
				AddPoint(p4, 0);

				modes.Insert(0, mode);
				pointsScales.Insert(0, Vector3.one);
				ApplyContraints(3);
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Removes curve at given index.
		/// </summary>
		/// <param name="curveIndex">Curve index.</param>
		public void RemoveCurve(int curveIndex)
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

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
				pointsScales.RemoveAt(modeIndex);
			}

			var nextPointIndex = (isLastCurve || startCurveIndex >= PointsCount) ? PointsCount - 1 : startCurveIndex;

			if (IsLoop && CurvesCount == 1)
			{
				IsLoop = false;
			}

			if (IsLoop)
			{
				SetPoint(0, points[0].Position);
			}

			SetPoint(nextPointIndex, Points[nextPointIndex].Position);

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Toggles spline loop state by adding/removing closing path curve.
		/// If beginning and ending points are in the same position then a path isn't created and spline is simply looped.
		/// </summary>
		public void ToggleClosingLoopCurve()
		{
			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;
			if (IsLoop)
			{
				IsLoop = false;
				RemoveCurve(CurvesCount - 1);
			}
			else
			{
				if (points[0].Position != points[PointsCount - 1].Position)
				{
					var p0 = points[PointsCount - 1].Position;
					var p1 = p0 - (points[PointsCount - 2].Position - p0);
					var p3 = points[0].Position;
					var p2 = p3 - (points[1].Position - p3);

					AppendCurve(p1, p2, p3, modes[0], false);
				}

				IsLoop = true;
			}

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				RecalculateNormals();
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
				RecalculateNormals();
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
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		/// <summary>
		/// Inserts a new curve by adding it at t point of curve at given index.
		/// </summary>
		/// <param name="curveIndex">Curve index.</param>
		/// <param name="t">Target t parameter for inserting a new curve.</param>
		public void InsertCurve(int curveIndex, float t)
		{
			t = Mathf.Clamp01(t);
			if (t == 0f || t == 1f)
			{
				return;
			}

			var prevInvokeEvents = invokeEvents;
			invokeEvents = false;

			var startPointIndex = curveIndex * 3;

			var p0 = points[startPointIndex].Position;

			var newT = (curveIndex + t) / CurvesCount;
			var newPoint = GetPoint(newT, false);

			newT = (curveIndex + (InsertCurveFirstPointT * t)) / CurvesCount;
			var pointOnCurve1 = GetPoint(newT, false);

			newT = (curveIndex + (InsertCurveSecondPointT * t)) / CurvesCount;
			var pointOnCurve2 = GetPoint(newT, false);

			// Left control point
			BezierUtils.GetInverseControlPoints(p0, newPoint, pointOnCurve1, pointOnCurve2, InsertCurveFirstPointT, InsertCurveSecondPointT, out var p1, out var p2);
			var leftControlPoint = p2;
			var updatedP1 = p1;

			var p3 = points[startPointIndex + 3].Position;

			newT = (curveIndex + t + ((1f - t) * InsertCurveFirstPointT)) / CurvesCount;
			pointOnCurve1 = GetPoint(newT, false);

			newT = (curveIndex + t + ((1f - t) * InsertCurveSecondPointT)) / CurvesCount;
			pointOnCurve2 = GetPoint(newT, false);

			// Right control point
			BezierUtils.GetInverseControlPoints(newPoint, p3, pointOnCurve1, pointOnCurve2, InsertCurveFirstPointT, InsertCurveSecondPointT, out p1, out p2);

			var rightControlPoint = p1;
			var updatedP2 = p2;

			SetPoint(startPointIndex + 2, updatedP2);
			SetPoint(startPointIndex + 1, updatedP1);

			AddPoint(leftControlPoint, startPointIndex + 2);
			AddPoint(newPoint, startPointIndex + 3);
			AddPoint(rightControlPoint, startPointIndex + 4);

			var modeIndex = (startPointIndex + 3) / 3;
			var prevMode = modes[modeIndex - 1];
			modes.Insert(modeIndex, prevMode);

			var pointScaleIndex = (startPointIndex + 3) / 3;
			var prevScale = pointsScales[pointScaleIndex - 1];
			pointsScales.Insert(pointScaleIndex, prevScale);

			ApplyAutoSetToAllControlPoints();

			invokeEvents = prevInvokeEvents;
			if (invokeEvents)
			{
				RecalculateNormals();
				OnSplineChanged?.Invoke();
			}
		}

		private void Reset()
		{
			modes = new List<BezierControlPointMode>(2);
			points = new List<SplinePoint>(4);
			pointsScales = new List<Vector3>(2);

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

			pointsScales.Add(Vector3.one);
			pointsScales.Add(Vector3.one);

			RecalculateNormals();
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

			var middle = Points[middleIndex].Position;
			var enforcedTangent = middle - Points[fixedIndex].Position;

			if (mode == BezierControlPointMode.Aligned)
			{
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, Points[enforcedIndex].Position);
			}

			Points[enforcedIndex].Position = middle + enforcedTangent;
			ApplyAffectedAutoSetControlPoints(pointIndex);
		}

		private void ApplyAutoSetToAllControlPoints()
		{
			for (var i = 0; i < PointsCount; i += 3)
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
			if (modes[curveIndex] != BezierControlPointMode.Auto)
			{
				return;
			}

			var startPointIndex = curveIndex * 3;
			var startPointPosition = Points[startPointIndex].Position;
			var previousPointPosition = startPointPosition;
			var nextPointPosition = startPointPosition;

			if (curveIndex > 0 || isLoop)
			{
				var loopedIndex = GetLoopingIndex(startPointIndex - 3);
				previousPointPosition = Points[loopedIndex].Position;
			}

			if (curveIndex < CurvesCount || isLoop)
			{
				var loopedIndex = GetLoopingIndex(startPointIndex + 3);
				nextPointPosition = Points[loopedIndex].Position;
			}

			if (previousPointPosition == nextPointPosition)
			{
				return;
			}

			var prevDistance = previousPointPosition - startPointPosition;
			var nextDistance = nextPointPosition - startPointPosition;
			var dir = (prevDistance.normalized - nextDistance.normalized).normalized;

			var prevControlPointIndex = GetLoopingIndex(startPointIndex - 1);
			var nextControlPointIndex = GetLoopingIndex(startPointIndex + 1);

			if (prevControlPointIndex != -1)
			{
				var updatedPosition = Points[startPointIndex].Position + (dir * prevDistance.magnitude * AutoSetControlPointInterpolationValue);
				Points[prevControlPointIndex].Position = updatedPosition;
			}

			if (nextControlPointIndex != -1)
			{
				var updatedPosition = Points[startPointIndex].Position - (dir * nextDistance.magnitude * AutoSetControlPointInterpolationValue);
				Points[nextControlPointIndex].Position = updatedPosition;
			}
		}

		private void ApplyAutoSetControlPointsToEdgePoints()
		{
			if (isLoop)
			{
				if (CurvesCount == 2 && modes[0] == BezierControlPointMode.Auto && modes[1] == BezierControlPointMode.Auto)
				{
					var dirAnchorAToB = (points[3].Position - points[0].Position).normalized;
					var dstBetweenAnchors = (points[0].Position - points[3].Position).magnitude;
					var perp = Vector3.Cross(dirAnchorAToB, Vector3.up);
					points[1].Position = points[0].Position + (perp * dstBetweenAnchors * AutoSetControlPointInterpolationValue);
					points[5].Position = points[0].Position - (perp * dstBetweenAnchors * AutoSetControlPointInterpolationValue);
					points[2].Position = points[3].Position + (perp * dstBetweenAnchors * AutoSetControlPointInterpolationValue);
					points[4].Position = points[3].Position - (perp * dstBetweenAnchors * AutoSetControlPointInterpolationValue);
				}
				else
				{
					ApplyAutoSetControlPoints(0);
					ApplyAutoSetControlPoints(CurvesCount - 1);
				}
			}
			else
			{
				if (modes[0] == BezierControlPointMode.Auto)
				{
					points[1].Position = (points[0].Position + points[2].Position) * AutoSetControlPointInterpolationValue;
				}

				if (modes[modes.Count - 1] == BezierControlPointMode.Auto)
				{
					points[points.Count - 2].Position = (points[points.Count - 1].Position + points[points.Count - 3].Position) * AutoSetControlPointInterpolationValue;
				}
			}
		}

		private void RemoveCurveAndRecalculateControlPoints(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;
			var p0Index = startPointIndex - 3;
			var p3Index = startPointIndex + 3;

			var p0 = points[p0Index].Position;
			var p3 = points[p3Index].Position;

			var t = (curveIndex - AutoSetControlPointInterpolationValue) / CurvesCount;
			var pointOnCurve1 = GetPoint(t, false);

			t = (curveIndex + AutoSetControlPointInterpolationValue) / CurvesCount;
			var pointOnCurve2 = GetPoint(t, false);

			BezierUtils.GetInverseControlPoints(p0, p3, pointOnCurve1, pointOnCurve2, AutoSetControlPointInterpolationValue / 2f, AutoSetControlPointInterpolationValue * 3 / 2f, out var p1, out var p2);
			SetPoint(p0Index + 1, p1);
			SetPoint(p3Index - 1, p2);

			RemoveCurve(curveIndex);
		}
	}
}