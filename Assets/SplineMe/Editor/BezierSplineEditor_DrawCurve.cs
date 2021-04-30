using UnityEditor;
using UnityEngine;
using static SplineMe.BezierSpline;

namespace SplineMe.Editor
{
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Private Fields

		private bool isCurveDrawerMode;
		private bool isDraggingNewCurve;
		private bool firstControlPointSet;
		private bool secondControlPointSet;

		private Vector3 curveDrawerPosition = Vector3.zero;
		private Vector3[] newCurvePoints = new Vector3[4];

		#endregion

		#region Initialize DrawCurveMode

		private void InitializeDrawCurveMode()
		{
			if (!isCurveDrawerMode)
			{
				return;
			}

			ToggleDrawCurveMode(true);
		}

		private void ToggleDrawCurveMode(bool state)
		{
			if (state && CurrentSpline.IsLoop)
			{
				return;
			}

			if (isCurveDrawerMode != state)
			{
				Undo.RecordObject(CurrentSpline, "Toggle Draw Curve Mode");
			}

			isCurveDrawerMode = state;

			if (state)
			{
				var lastPoint = CurrentSpline.Points[CurrentSpline.PointsCount - 1];
				StartDrawCurveMode(lastPoint.position);
				SelectIndex(-1);
			}
		}

		private void StartDrawCurveMode(Vector3 startPoint)
		{
			for (var i = 0; i < newCurvePoints.Length; i++)
			{
				newCurvePoints[i] = startPoint;
			}

			curveDrawerPosition = startPoint;
			firstControlPointSet = false;
			secondControlPointSet = false;
		}


		#endregion

		#region Draw Scene GUI

		private void DrawCurveModeSceneGUI()
		{
			var curveDrawerPointLocal = curveDrawerPosition;
			var curveDrawerPointWorld = CurrentSpline.transform.TransformPoint(curveDrawerPointLocal);
			var size = HandleUtility.GetHandleSize(curveDrawerPointWorld);

			if (isDraggingNewCurve)
			{
				VisualizeDrawCurveModeCurve();
			}

			Handles.color = Color.green;
			Handles.Button(curveDrawerPointWorld, handleRotation, size * BezierSplineEditor_Consts.DrawCurveSphereSize, size * BezierSplineEditor_Consts.DrawCurveSphereSize, Handles.SphereHandleCap);

			if (castSelectedPointFlag)
			{
				isDraggingNewCurve = true;
				if (TryCastMousePoint(out var castedPosition))
				{
					Undo.RecordObject(CurrentSpline, "Cast Drawer Point");
					curveDrawerPosition = CurrentSpline.transform.InverseTransformPoint(castedPosition);
					UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				var newEndPositionGlobal = Handles.DoPositionHandle(curveDrawerPointWorld, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					isDraggingNewCurve = true;
					Undo.RecordObject(CurrentSpline, "Move Drawer Point");
					curveDrawerPosition = CurrentSpline.transform.InverseTransformPoint(newEndPositionGlobal);
					UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
				}
				else if ((isDraggingNewCurve && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					if (firstControlPointSet && secondControlPointSet)
					{
						SpawnDrawCurveModeCurve(drawCurveSmoothAcuteAngles);
					}

					var defaultDrawerPosition = CurrentSpline.Points[CurrentSpline.PointsCount - 1].position;
					StartDrawCurveMode(defaultDrawerPosition);
					isDraggingNewCurve = false;
					castSelectedPointFlag = false;
				}
			}
		}

		private void VisualizeDrawCurveModeCurve()
		{
			var p0 = newCurvePoints[0];
			var p3 = newCurvePoints[3];

			if (Vector3.Distance(p0, p3) < BezierSplineEditor_Consts.DrawCurveMinLengthToVisualize)
			{
				return;
			}

			BezierUtils.GetInverseControlPoints(p0, p3, newCurvePoints[1], newCurvePoints[2], BezierSplineEditor_Consts.DrawCurveFirstControlPointT, BezierSplineEditor_Consts.DrawCurveSecondControlPointT, out var p1, out var p2);

			p0 = handleTransform.TransformPoint(p0);
			p1 = handleTransform.TransformPoint(p1);
			p2 = handleTransform.TransformPoint(p2);
			p3 = handleTransform.TransformPoint(p3);

			Handles.DrawBezier(p0, p3, p1, p2, BezierSplineEditor_Consts.LineColor, null, BezierSplineEditor_Consts.LineWidth * 1.5f);

			if (showPointsHandles)
			{
				if (firstControlPointSet)
				{
					var f = handleTransform.TransformPoint(newCurvePoints[1]);
					var size = HandleUtility.GetHandleSize(f);
					Handles.color = BezierSplineEditor_Consts.DrawCurvePointColor;
					Handles.CubeHandleCap(0, f, Quaternion.identity, size * 0.1f, EventType.Repaint);
				}

				if (secondControlPointSet)
				{
					var g = handleTransform.TransformPoint(newCurvePoints[2]);
					var size = HandleUtility.GetHandleSize(g);
					Handles.color = BezierSplineEditor_Consts.DrawCurvePointColor;
					Handles.CubeHandleCap(0, g, Quaternion.identity, size * 0.1f, EventType.Repaint);
				}

			}

		}

		private void UpdateNewDrawCurvePainterPosition(Vector3 newEndPosition)
		{
			float distance;
			if (secondControlPointSet)
			{
				distance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) + Vector3.Distance(newCurvePoints[1], newCurvePoints[2]) + Vector3.Distance(newCurvePoints[2], newEndPosition);
			}
			else if (firstControlPointSet)
			{
				distance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) + Vector3.Distance(newCurvePoints[1], newEndPosition);
			}
			else
			{
				distance = Vector3.Distance(newCurvePoints[0], newEndPosition);
			}

			var dir = (newEndPosition - newCurvePoints[0]).normalized;
			var firstPointDistance = BezierSplineEditor_Consts.DrawCurveFirstControlPointT * BezierSplineEditor_Consts.DrawCurveSegmentLength;
			var secondPointDistance = BezierSplineEditor_Consts.DrawCurveSecondControlPointT * BezierSplineEditor_Consts.DrawCurveSegmentLength;

			if (!firstControlPointSet)
			{
				if (distance < firstPointDistance)
				{
					newCurvePoints[1] = newEndPosition;
				}
				else if (distance >= secondPointDistance)
				{
					newCurvePoints[1] = newCurvePoints[0] + dir * firstPointDistance;
					firstControlPointSet = true;
				}
			}

			if (!secondControlPointSet)
			{
				if (distance < secondPointDistance)
				{
					newCurvePoints[2] = newEndPosition;
				}
				else if (distance >= BezierSplineEditor_Consts.DrawCurveSegmentLength)
				{
					newCurvePoints[2] = newCurvePoints[0] + dir * secondPointDistance;
					secondControlPointSet = true;
				}
			}

			if (distance < secondPointDistance && distance >= firstPointDistance && !firstControlPointSet)
			{
				var prevFirstPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) / BezierSplineEditor_Consts.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / BezierSplineEditor_Consts.DrawCurveSegmentLength;
				var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevFirstPointNormalizedDistance);
				newCurvePoints[1] = Vector3.Lerp(newCurvePoints[1], newEndPosition, Mathf.Abs(BezierSplineEditor_Consts.DrawCurveFirstControlPointT - prevFirstPointNormalizedDistance) / normalizedDistancesDiff);
				firstControlPointSet = true;
			}

			if (distance < BezierSplineEditor_Consts.DrawCurveSegmentLength)
			{
				newCurvePoints[3] = newEndPosition;

				if (distance >= secondPointDistance && !secondControlPointSet)
				{
					var prevSecondPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[2]) / BezierSplineEditor_Consts.DrawCurveSegmentLength;
					var normalizedCurrentDistance = distance / BezierSplineEditor_Consts.DrawCurveSegmentLength;
					var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevSecondPointNormalizedDistance);
					newCurvePoints[2] = Vector3.Lerp(newCurvePoints[2], newEndPosition, Mathf.Abs(BezierSplineEditor_Consts.DrawCurveSecondControlPointT - prevSecondPointNormalizedDistance) / normalizedDistancesDiff);
					secondControlPointSet = true;
				}
			}
			else
			{
				var prevLastPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[3]) / BezierSplineEditor_Consts.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / BezierSplineEditor_Consts.DrawCurveSegmentLength;
				var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevLastPointNormalizedDistance);
				newCurvePoints[3] = Vector3.Lerp(newCurvePoints[3], newEndPosition, Mathf.Abs(1f - prevLastPointNormalizedDistance) / normalizedDistancesDiff);
				SpawnDrawCurveModeCurve(drawCurveSmoothAcuteAngles);
				StartDrawCurveMode(newCurvePoints[3]);
				UpdateNewDrawCurvePainterPosition(newEndPosition);
			}

		}

		private void SpawnDrawCurveModeCurve(bool smoothAcuteAngles)
		{
			var p0 = newCurvePoints[0];
			var p3 = newCurvePoints[3];
			BezierUtils.GetInverseControlPoints(p0, p3, newCurvePoints[1], newCurvePoints[2], BezierSplineEditor_Consts.DrawCurveFirstControlPointT, BezierSplineEditor_Consts.DrawCurveSecondControlPointT, out var p1, out var p2);

			if (smoothAcuteAngles)
			{
				CurrentSpline.SetControlPointMode(CurrentSpline.PointsCount - 1, BezierControlPointMode.Aligned);
			}

			CurrentSpline.AppendCurve(p1, p2, p3, BezierControlPointMode.Free, false);
		}

		#endregion

	}

}
