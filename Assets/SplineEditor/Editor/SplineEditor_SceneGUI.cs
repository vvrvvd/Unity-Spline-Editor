using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Private Fields

		private bool isRotating;
		private bool isScaling;
		private bool isDraggingPoint;

		private Vector3 lastScale;
		private Quaternion lastRotation;

		#endregion

		#region Initialize Scene GUI

		private void InitializeSceneGUI()
		{
			isScaling = false;
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
			}

			if (IsDrawerMode)
			{
				DrawCurveModeSceneGUI();
			}

			if (CurrentSpline.drawPoints)
			{
				DrawPoints();
			}

		}

		private void DrawPoints()
		{
			Vector3 p0, p1, p2, p3;
			for (var i = 0; i < CurrentSpline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				if(i > 0 && i%3!=0)
				{
					p0 = handleTransform.TransformPoint(currentSpline.Points[curveStartIndex].position);
				}
				else
				{
					p0 = DrawPoint(curveStartIndex);
				}
				p1 = DrawPoint(curveStartIndex + 1);
				p2 = DrawPoint(curveStartIndex + 2);
				p3 = handleTransform.TransformPoint(CurrentSpline.Points[curveStartIndex + 3].position);

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
			var modeColor = GetModeColor(mode);
			var pointColor = index % 3 == 0 ? editorSettings.PointColor : modeColor;

			return DrawPoint(index, pointColor);
		}

		private Color GetModeColor(BezierSpline.BezierControlPointMode mode)
		{
			switch (mode)
			{
				case BezierSpline.BezierControlPointMode.Free:
					return editorSettings.FreeModeColor;
				case BezierSpline.BezierControlPointMode.Aligned:
					return editorSettings.AlignedModeColor;
				case BezierSpline.BezierControlPointMode.Mirrored:
					return editorSettings.MirroredModeColor;
				case BezierSpline.BezierControlPointMode.Auto:
					return editorSettings.AutoModeColor;
			}

			return Color.cyan;
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(CurrentSpline.Points[index].position);
			var size = editorSettings.ScalePointOnScreen ? HandleUtility.GetHandleSize(point) : 1f;

			Handles.color = pointColor;
			var handleSize = index % 3 == 0 ? editorSettings.MainPointSize : editorSettings.TangentPointSize;

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
					if (isNormalsEditorMode)
					{
						RotateNormals(index, point);
					}
					else
					{
						RotatePoints(index, point);
					}
				}
				else if(savedTool == Tool.Scale && index % 3 == 0)
				{
					ScalePoint(index, point);
				}
				else
				{
					MovePoint(index, point);
				}
			}

			if (currentSpline.drawNormals && index % 3 == 0)
			{
				var normalIndex = index / 3;
				if (currentSpline.Normals.Length <= normalIndex)
				{
					currentSpline.RecalculateNormals();
				}
				var normalVector = currentSpline.GetNormal(normalIndex);
				var rightVector = Vector3.Cross(normalVector, currentSpline.Tangents[normalIndex]);
				var normalLength = editorSettings.NormalVectorLength;
				Handles.color = editorSettings.NormalsColor;
				Handles.DrawLine(point, point + currentSpline.transform.TransformDirection((normalVector) * normalLength));
			}

			return point;
		}

		private void MovePoint(int index, Vector3 point)
		{
			if (castSelectedPointFlag)
			{
				if (TryCastMousePoint(out var castedPosition))
				{
					Undo.RecordObject(CurrentSpline, "Cast Spline Point To Mouse");
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
					Undo.RecordObject(CurrentSpline, "Move Spline Point");
					isDraggingPoint = true;
					CurrentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					wasSplineModified = true;
				}
				else if ((isDraggingPoint && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					Undo.RecordObject(CurrentSpline, "Move Spline Point");
					CurrentSpline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					isDraggingPoint = false;
					castSelectedPointFlag = false;
					wasSplineModified = true;
				}
			}
		}

		private void ScalePoint(int index, Vector3 point)
		{
			var normalIndex = index / 3;

			var normalAngularOffset = currentSpline.NormalsAngularOffsets[normalIndex];
			var globalRotation = Quaternion.AngleAxis(currentSpline.GlobalNormalsRotation, currentSpline.Tangents[normalIndex]);
			var normalRotation = globalRotation * Quaternion.AngleAxis(normalAngularOffset + MagicAngleOffset, currentSpline.Tangents[normalIndex]);
			var normalHandleRotation = normalRotation * Quaternion.LookRotation(currentSpline.Tangents[normalIndex]);
			var baseHandleRotation = handleTransform.rotation * normalHandleRotation;

			var handleSize = HandleUtility.GetHandleSize(point);

			var pointScaleIndex = index / 3;
			var pointScale = currentSpline.PointsScales[pointScaleIndex];

			var leftControlPointIndex = index - 1;
			var isLeftPointIndexValid = leftControlPointIndex >= 0;

			var rightControlPointIndex = index + 1;
			var isRightPointIndexValid = rightControlPointIndex < currentSpline.PointsCount;

			EditorGUI.BeginChangeCheck();
			lastScale = Handles.DoScaleHandle(isScaling ? lastScale : pointScale, point, baseHandleRotation, handleSize);
			var wasChanged = EditorGUI.EndChangeCheck();
			if (wasChanged)
			{

				if (!isScaling)
				{
					isScaling = true;
					lastScale = new Vector3(lastScale.x, lastScale.y, lastScale.z);
					return;
				}

				Undo.RecordObject(CurrentSpline, "Scale Spline Point");
				isScaling = true;

				CurrentSpline.UpdatePointsScale(pointScaleIndex, lastScale);
				wasSplineModified = true;
			}
			else if ((isScaling && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				CurrentSpline.UpdatePointsScale(pointScaleIndex, lastScale);
				isScaling = false;
				wasSplineModified = true;
			}
		}

		private void RotatePoints(int index, Vector3 point)
		{
			EditorGUI.BeginChangeCheck();
			var rotation = Handles.DoRotationHandle(handleRotation, point);
			if (EditorGUI.EndChangeCheck())
			{
				if (!isRotating)
				{
					lastRotation = handleRotation;
					isRotating = true;
				}

				var rotationDiff = rotation * Quaternion.Inverse(lastRotation);

				Undo.RecordObject(CurrentSpline, "Rotate Spline Point");
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
			else if ((isRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
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

				var splineColor = (i == selectedSplineIndex && currentSpline.drawPoints) ? editorSettings.SelectedCurveColor : editorSettings.SplineColor;
				Handles.DrawBezier(p0, p3, p1, p2, splineColor, null, editorSettings.SplineWidth * 1.5f);
			}
		}

		#endregion

	}

}
