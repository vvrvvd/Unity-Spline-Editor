using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Private Fields

		private bool isRotating;
		private bool isSnapping;
		private bool isDraggingPoint;
		private Quaternion lastRotation;

		#endregion

		#region Initialize Scene GUI

		private void InitializeSceneGUI()
		{
			isRotating = false;
			isDraggingPoint = false;
			lastRotation = Quaternion.identity;
		}

		#endregion

		#region Draw Scene GUI

		private void DrawSceneGUI()
		{
			if (Event.current.type == EventType.Repaint)
			{
				DrawSpline(currentSpline, SelectedCurveIndex);

				if (showDirectionsLines)
				{
					DrawSplineDirections(currentSpline);
				}

				if (showSegmentsPoints)
				{
					DrawSplineSegments(currentSpline);
				}

			}

			if (isCurveDrawerMode)
			{
				DrawCurveModeSceneGUI();
			}

			if (showPointsHandles)
			{
				DrawPoints();
			}

		}

		private void DrawPoints()
		{
			for (var i = 0; i < currentSpline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				var p0 = DrawPoint(curveStartIndex);
				var p1 = DrawPoint(curveStartIndex + 1);
				var p2 = DrawPoint(curveStartIndex + 2);
				var p3 = handleTransform.TransformPoint(currentSpline.Points[curveStartIndex + 3].position);

				if (!isCurveDrawerMode || i < currentSpline.CurvesCount - 1)
				{
					p3 = DrawPoint(curveStartIndex + 3);
				}

				DrawLine(p0, p1, BezierSplineEditor_Consts.TangentLineColor);
				DrawLine(p3, p2, BezierSplineEditor_Consts.TangentLineColor);
			}
		}

		private static void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index)
		{
			var mode = currentSpline.GetControlPointMode(index);
			var pointColor = index % 3 == 0 ? BezierSplineEditor_Consts.CurvePointColor : BezierSplineEditor_Consts.ModeColors[(int)mode];

			return DrawPoint(index, pointColor);
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(currentSpline.Points[index].position);
			var size = HandleUtility.GetHandleSize(point);

			if (index != 0 && index != currentSpline.PointsCount - 1)
			{
				size *= 0.5f;
			}
			else
			{
				var nextEndPointIndex = index == 0 ? currentSpline.PointsCount - 1 : 0;
				var nextEndPoint = handleTransform.TransformPoint(currentSpline.Points[nextEndPointIndex].position);
				var pointsDistance = Vector3.Distance(point, nextEndPoint);
				isSnapping = (currentEditor.selectedPointIndex == 0 || currentEditor.selectedPointIndex == currentSpline.PointsCount - 1) && snapEndPointsFlag && !currentSpline.IsLoop && pointsDistance <= size * BezierSplineEditor_Consts.SnapSplineEndPointsMinDistance;
				if (isSnapping)
				{
					Handles.color = BezierSplineEditor_Consts.SnapEndPointsLineColor;
					Handles.DrawDottedLine(point, nextEndPoint, 5f);
				}
			}

			Handles.color = pointColor;

			if (Handles.Button(point, handleRotation, size * BezierSplineEditor_Consts.HandlePointSize, size * BezierSplineEditor_Consts.PickPointSize, Handles.DotHandleCap))
			{
				SelectIndex(index);
				Repaint();
			}

			if (selectedPointIndex == index)
			{
				if (savedTool == Tool.Rotate && index % 3 == 0)
				{
					RotateLocal(index, point);
				}
				else
				{
					MovePoint(index, point);
				}
			}

			return point;
		}

		private void MovePoint(int index, Vector3 point)
		{
			if (castSelectedPointFlag)
			{
				var castedPosition = Vector3.zero;
				if (TryCastMousePoint(out castedPosition))
				{
					Undo.RecordObject(currentSpline, "Cast Line Point To Mouse");
					point = castedPosition;
					currentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					Undo.RecordObject(currentSpline, "Move Line Point");
					isDraggingPoint = true;
					currentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
				}
				else if ((isDraggingPoint && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					Undo.RecordObject(currentSpline, "Move Line Point");
					currentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					isDraggingPoint = false;
					castSelectedPointFlag = false;
				}
			}
		}

		private void RotateLocal(int index, Vector3 point)
		{
			EditorGUI.BeginChangeCheck();
			var rotation = Handles.DoRotationHandle(handleRotation, point);
			if (EditorGUI.EndChangeCheck())
			{
				if (!isRotating)
				{
					lastRotation = rotation;
					isRotating = true;
				}

				var rotationDiff = rotation * Quaternion.Inverse(lastRotation);

				Undo.RecordObject(currentSpline, "Rotate Line Point");
				var point1Index = index == currentSpline.PointsCount - 1 && currentSpline.IsLoop ? 1 : index + 1;
				var point2Index = index == 0 && currentSpline.IsLoop ? currentSpline.PointsCount - 2 : index - 1;

				if (point1Index >= 0 && point1Index < currentSpline.PointsCount)
				{
					var point1 = handleTransform.TransformPoint(currentSpline.Points[point1Index].position);
					var rotatedPoint1 = Vector3Utils.RotateAround(point1, point, rotationDiff);
					currentSpline.UpdatePoint(point1Index, handleTransform.InverseTransformPoint(rotatedPoint1));
				}

				if (point2Index >= 0 && point2Index < currentSpline.PointsCount)
				{
					var point2 = handleTransform.TransformPoint(currentSpline.Points[point2Index].position);
					var rotatedPoint2 = Vector3Utils.RotateAround(point2, point, rotationDiff);
					currentSpline.UpdatePoint(point2Index, handleTransform.InverseTransformPoint(rotatedPoint2));
				}

				lastRotation = rotation;
			}
			else if (isRotating && currentEvent.type == EventType.MouseUp)
			{
				lastRotation = handleRotation;
				isRotating = false;
			}
		}

		#endregion

		#region Static Methods

		private static void DrawSpline(BezierSpline spline, int selectedSplineIndex = -1)
		{
			var transformHandle = spline.transform;
			for (var i = 0; i < spline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				var p0 = transformHandle.TransformPoint(spline.Points[curveStartIndex].position);
				var p1 = transformHandle.TransformPoint(spline.Points[curveStartIndex + 1].position);
				var p2 = transformHandle.TransformPoint(spline.Points[curveStartIndex + 2].position);
				var p3 = transformHandle.TransformPoint(spline.Points[curveStartIndex + 3].position);

				var splineColor = !snapEndPointsFlag && i == selectedSplineIndex ? BezierSplineEditor_Consts.SelectedLineColor : BezierSplineEditor_Consts.LineColor;
				Handles.DrawBezier(p0, p3, p1, p2, splineColor, null, BezierSplineEditor_Consts.LineWidth * 1.5f);
			}
		}

		private static void DrawSplineDirections(BezierSpline spline)
		{
			var point = spline.GetPoint(1f);
			Handles.DrawLine(point, point - spline.GetDirection(1f) * BezierSplineEditor_Consts.DirectionScale);

			var curveSteps = BezierSplineEditor_Consts.CurveStepsCount * spline.CurvesCount;
			for (int i = curveSteps - 1; i >= 0; i--)
			{
				point = spline.GetPoint(i / (float)curveSteps);
				Handles.color = BezierSplineEditor_Consts.DirectionLineColor;
				Handles.DrawLine(point, point - spline.GetDirection(i / (float)curveSteps) * BezierSplineEditor_Consts.DirectionScale);
			}
		}

		private static void DrawSplineSegments(BezierSpline spline)
		{
			var point = spline.GetPoint(1f);
			Handles.color = BezierSplineEditor_Consts.SegmentsColor;
			var curveSteps = BezierSplineEditor_Consts.CurveStepsCount * spline.CurvesCount;
			for (int i = curveSteps - 1; i >= 0; i--)
			{
				var size = HandleUtility.GetHandleSize(point);
				point = spline.GetPoint(i / (float)curveSteps);
				Handles.Button(point, Quaternion.identity, size * BezierSplineEditor_Consts.HandleSegmentSize, size * BezierSplineEditor_Consts.HandleSegmentSize, Handles.DotHandleCap);
			}
		}

		#endregion

	}

}
