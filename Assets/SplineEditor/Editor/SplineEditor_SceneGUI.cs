using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
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
				DrawSpline(CurrentSpline, SelectedCurveIndex);

				if (showDirectionsLines)
				{
					DrawSplineDirections(CurrentSpline);
				}

				if (showSegmentsPoints)
				{
					DrawSplineSegments(CurrentSpline);
				}

			}

			if (IsDrawerMode)
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
			for (var i = 0; i < CurrentSpline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				var p0 = DrawPoint(curveStartIndex);
				var p1 = DrawPoint(curveStartIndex + 1);
				var p2 = DrawPoint(curveStartIndex + 2);
				var p3 = handleTransform.TransformPoint(CurrentSpline.Points[curveStartIndex + 3].position);

				if (!IsDrawerMode || i < CurrentSpline.CurvesCount - 1)
				{
					p3 = DrawPoint(curveStartIndex + 3);
				}

				DrawLine(p0, p1, editorSettings.TangentLineColor);
				DrawLine(p3, p2, editorSettings.TangentLineColor);
			}
		}

		private static void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index)
		{
			var mode = CurrentSpline.GetControlPointMode(index);
			var modeIndex = (int)mode;
			var modeColor = modeIndex == 0 ? editorSettings.FreeModeColor : modeIndex == 1 ? editorSettings.AlignedModeColor : editorSettings.MirroredModeColor;
			var pointColor = index % 3 == 0 ? editorSettings.PointColor : modeColor;

			return DrawPoint(index, pointColor);
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(CurrentSpline.Points[index].position);
			var size = editorSettings.ScalePointOnScreen ? HandleUtility.GetHandleSize(point) : 1f;

			if (index == 0 || index == CurrentSpline.PointsCount - 1)
			{
				size *= editorSettings.BeginAndEndPointsScale;
				var nextEndPointIndex = index == 0 ? CurrentSpline.PointsCount - 1 : 0;
				var nextEndPoint = handleTransform.TransformPoint(CurrentSpline.Points[nextEndPointIndex].position);
				var pointsDistance = Vector3.Distance(point, nextEndPoint);
				isSnapping = (SelectedPointIndex == 0 || SelectedPointIndex == CurrentSpline.PointsCount - 1) && snapEndPointsFlag && !CurrentSpline.IsLoop && pointsDistance <= size * editorSettings.SnapSplineEndPointsMinDistance;
				if (isSnapping)
				{
					Handles.color = editorSettings.SnapEndPointsLineColor;
					Handles.DrawDottedLine(point, nextEndPoint, 5f);
				}
			}

			Handles.color = pointColor;
			var handleSize = index % 3 == 0 ? editorSettings.MainPointSize :  editorSettings.TangentPointSize;

			if (Handles.Button(point, handleRotation, size * handleSize, size * handleSize, Handles.DotHandleCap))
			{
				SelectIndex(index);
				Repaint();
				wasSplineModified = true;
			}

			if (SelectedPointIndex == index)
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
				if (TryCastMousePoint(out var castedPosition))
				{
					Undo.RecordObject(CurrentSpline, "Cast Line Point To Mouse");
					point = castedPosition;
					CurrentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					wasSplineModified = true;
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					Undo.RecordObject(CurrentSpline, "Move Line Point");
					isDraggingPoint = true;
					CurrentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					wasSplineModified = true;
				}
				else if ((isDraggingPoint && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					Undo.RecordObject(CurrentSpline, "Move Line Point");
					CurrentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					isDraggingPoint = false;
					castSelectedPointFlag = false;
					wasSplineModified = true;
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

				Undo.RecordObject(CurrentSpline, "Rotate Line Point");
				var point1Index = index == CurrentSpline.PointsCount - 1 && CurrentSpline.IsLoop ? 1 : index + 1;
				var point2Index = index == 0 && CurrentSpline.IsLoop ? CurrentSpline.PointsCount - 2 : index - 1;

				if (point1Index >= 0 && point1Index < CurrentSpline.PointsCount)
				{
					var point1 = handleTransform.TransformPoint(CurrentSpline.Points[point1Index].position);
					var rotatedPoint1 = Vector3Utils.RotateAround(point1, point, rotationDiff);
					CurrentSpline.UpdatePoint(point1Index, handleTransform.InverseTransformPoint(rotatedPoint1));
				}

				if (point2Index >= 0 && point2Index < CurrentSpline.PointsCount)
				{
					var point2 = handleTransform.TransformPoint(CurrentSpline.Points[point2Index].position);
					var rotatedPoint2 = Vector3Utils.RotateAround(point2, point, rotationDiff);
					CurrentSpline.UpdatePoint(point2Index, handleTransform.InverseTransformPoint(rotatedPoint2));
				}

				lastRotation = rotation;
				wasSplineModified = true;
			}
			else if (isRotating && currentEvent.type == EventType.MouseUp)
			{
				lastRotation = handleRotation;
				isRotating = false;
				wasSplineModified = true;
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

				var splineColor = !snapEndPointsFlag && i == selectedSplineIndex ? editorSettings.SelectedCurveColor : editorSettings.SplineColor;
				Handles.DrawBezier(p0, p3, p1, p2, splineColor, null, editorSettings.SplineWidth * 1.5f);
			}
		}

		private static void DrawSplineDirections(BezierSpline spline)
		{
			var point = spline.GetPoint(1f);
			Handles.DrawLine(point, point - spline.GetDirection(1f) * editorSettings.DirectionScale);

			var curveSteps = editorSettings.CurveStepsCount * spline.CurvesCount;
			for (int i = curveSteps - 1; i >= 0; i--)
			{
				point = spline.GetPoint(i / (float)curveSteps);
				Handles.color = editorSettings.DirectionLineColor;
				Handles.DrawLine(point, point - spline.GetDirection(i / (float)curveSteps) * editorSettings.DirectionScale);
			}
		}

		private static void DrawSplineSegments(BezierSpline spline)
		{
			var point = spline.GetPoint(1f);
			Handles.color = editorSettings.SegmentsColor;
			var curveSteps = editorSettings.CurveStepsCount * spline.CurvesCount;
			for (int i = curveSteps - 1; i >= 0; i--)
			{
				var size = HandleUtility.GetHandleSize(point);
				point = spline.GetPoint(i / (float)curveSteps);
				Handles.Button(point, Quaternion.identity, size * editorSettings.HandleSegmentSize, size * editorSettings.HandleSegmentSize, Handles.DotHandleCap);
			}
		}

		#endregion

	}

}
