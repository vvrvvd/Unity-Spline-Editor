using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		private void InitializeSceneGUI()
		{
			EditorState.isScaling = false;
			EditorState.isRotating = false;
			EditorState.isDraggingPoint = false;
			EditorState.lastRotation = Quaternion.identity;
		}

		private void DrawSceneGUI()
		{
			if (EditorState.DrawSpline)
			{
				DrawSpline(EditorState.CurrentSpline, EditorState.SelectedCurveIndex);
			}

			if (EditorState.IsDrawerMode)
			{
				DrawCurveModeSceneGUI();
			}

			if (EditorState.DrawPoints)
			{
				DrawPoints();
			}

			if (EditorState.DrawNormals)
			{
				DrawNormals();
			}

		}

		private void DrawPoints()
		{
			Vector3 p0, p1, p2, p3;
			for (var i = 0; i < EditorState.CurrentSpline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				if(i > 0 && i%3!=0)
				{
					p0 = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[curveStartIndex].Position);
				}
				else
				{
					p0 = DrawPoint(curveStartIndex);
				}
				p1 = DrawPoint(curveStartIndex + 1);
				p2 = DrawPoint(curveStartIndex + 2);
				p3 = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[curveStartIndex + 3].Position);

				if (!EditorState.IsDrawerMode || i < EditorState.CurrentSpline.CurvesCount - 1)
				{
					p3 = DrawPoint(curveStartIndex + 3);
				}

				DrawLine(p0, p1, EditorSettings.TangentLineColor);
				DrawLine(p3, p2, EditorSettings.TangentLineColor);
			}
		}

		private void DrawNormals()
		{
			for (var i = 0; i < EditorState.CurrentSpline.PointsCount; i+=3)
			{
				var point = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[i].Position);
				var normalIndex = i / 3;
				if (EditorState.CurrentSpline.Normals.Count <= normalIndex)
				{
					EditorState.CurrentSpline.RecalculateNormals();
				}
				var normalVector = EditorState.CurrentSpline.GetNormal(normalIndex);
				var normalLength = EditorSettings.NormalVectorLength;
				Handles.color = EditorSettings.NormalsColor;
				Handles.DrawLine(point, point + EditorState.CurrentSpline.transform.TransformDirection((normalVector) * normalLength));
			}
			
		}

		private static void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index)
		{
			var mode = EditorState.CurrentSpline.GetControlPointMode(index);
			var modeColor = GetModeColor(mode);
			var pointColor = index % 3 == 0 ? EditorSettings.PointColor : modeColor;

			return DrawPoint(index, pointColor);
		}

		private Color GetModeColor(BezierSpline.BezierControlPointMode mode)
		{
			switch (mode)
			{
				case BezierSpline.BezierControlPointMode.Free:
					return EditorSettings.FreeModeColor;
				case BezierSpline.BezierControlPointMode.Aligned:
					return EditorSettings.AlignedModeColor;
				case BezierSpline.BezierControlPointMode.Mirrored:
					return EditorSettings.MirroredModeColor;
				case BezierSpline.BezierControlPointMode.Auto:
					return EditorSettings.AutoModeColor;
			}

			return Color.cyan;
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var worldPoint = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[index].Position);
			var size = EditorSettings.ScalePointOnScreen ? HandleUtility.GetHandleSize(worldPoint) : 1f;

			Handles.color = pointColor;
			var handleSize = index % 3 == 0 ? EditorSettings.MainPointSize : EditorSettings.TangentPointSize;

			if (Handles.Button(worldPoint, handleRotation, size * handleSize, size * handleSize, Handles.DotHandleCap))
			{
				SelectIndex(index);
				Repaint();
				EditorState.wasSplineModified = true;
			}

			if (EditorState.SelectedPointIndex == index)
			{
				if (EditorState.savedTool == Tool.Rotate && index % 3 == 0)
				{
					if (EditorState.IsNormalsEditorMode)
					{
						RotateNormals(index, worldPoint);
					}
					else
					{
						RotatePoints(index, worldPoint);
					}
				}
				else if(EditorState.savedTool == Tool.Scale && index % 3 == 0)
				{
					ScalePoint(index, worldPoint);
				}
				else
				{
					MovePoint(index, worldPoint);
				}
			}

			return worldPoint;
		}

		private void MovePoint(int index, Vector3 worldPoint)
		{
			if (castSelectedPointFlag)
			{
				if (TryCastMousePoint(out var castedPosition))
				{
					Undo.RecordObject(EditorState.CurrentSpline, "Cast Spline Point To Mouse");
					worldPoint = castedPosition;
					EditorState.CurrentSpline.SetPoint(index, handleTransform.InverseTransformPoint(worldPoint));
					EditorState.wasSplineModified = true;
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				worldPoint = Handles.DoPositionHandle(worldPoint, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					Undo.RecordObject(EditorState.CurrentSpline, "Move Spline Point");
					EditorState.isDraggingPoint = true;
					EditorState.CurrentSpline.SetPoint(index, handleTransform.InverseTransformPoint(worldPoint));
					EditorState.wasSplineModified = true;
				}
				else if ((EditorState.isDraggingPoint && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					Undo.RecordObject(EditorState.CurrentSpline, "Move Spline Point");
					EditorState.CurrentSpline.SetPoint(index, handleTransform.InverseTransformPoint(worldPoint));
					EditorState.isDraggingPoint = false;
					castSelectedPointFlag = false;
					EditorState.wasSplineModified = true;
				}
			}
		}

		private void ScalePoint(int index, Vector3 worldPoint)
		{
			var normalIndex = index / 3;
			var pointScaleIndex = index / 3;
			
			var handleSize = HandleUtility.GetHandleSize(worldPoint);
			var pointScale = EditorState.CurrentSpline.PointsScales[pointScaleIndex];
			var normalVector = EditorState.CurrentSpline.GetNormal(normalIndex);
			var tangentVector = EditorState.CurrentSpline.Tangents[normalIndex];

			var normalWorldVector = EditorState.CurrentSpline.transform.TransformDirection(normalVector).normalized;
			var tangentWorldVector = EditorState.CurrentSpline.transform.TransformDirection(tangentVector).normalized;
			var baseHandleRotation = Quaternion.LookRotation(normalWorldVector, tangentWorldVector);

			EditorGUI.BeginChangeCheck();
			EditorState.lastScale = Handles.DoScaleHandle(EditorState.isScaling ? EditorState.lastScale : pointScale, worldPoint, baseHandleRotation, handleSize);
			var wasChanged = EditorGUI.EndChangeCheck();
			if (wasChanged)
			{

				if (!EditorState.isScaling)
				{
					EditorState.isScaling = true;
					EditorState.lastScale = new Vector3(EditorState.lastScale.x, EditorState.lastScale.y, EditorState.lastScale.z);
					return;
				}

				Undo.RecordObject(EditorState.CurrentSpline, "Scale Spline Point");
				EditorState.isScaling = true;

				EditorState.CurrentSpline.SetPointsScale(pointScaleIndex, EditorState.lastScale);
				EditorState.wasSplineModified = true;
			}
			else if ((EditorState.isScaling && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				EditorState.CurrentSpline.SetPointsScale(pointScaleIndex, EditorState.lastScale);
				EditorState.isScaling = false;
				EditorState.wasSplineModified = true;
			}
		}

		private void RotatePoints(int index, Vector3 worldPoint)
		{
			EditorGUI.BeginChangeCheck();
			var rotation = Handles.DoRotationHandle(handleRotation, worldPoint);
			if (EditorGUI.EndChangeCheck())
			{
				if (!EditorState.isRotating)
				{
					EditorState.lastRotation = handleRotation;
					EditorState.isRotating = true;
				}

				var rotationDiff = rotation * Quaternion.Inverse(EditorState.lastRotation);

				Undo.RecordObject(EditorState.CurrentSpline, "Rotate Spline Point");
				var point1Index = index == EditorState.CurrentSpline.PointsCount - 1 && EditorState.CurrentSpline.IsLoop ? 1 : index + 1;
				var point2Index = index == 0 && EditorState.CurrentSpline.IsLoop ? EditorState.CurrentSpline.PointsCount - 2 : index - 1;

				if (point1Index >= 0 && point1Index < EditorState.CurrentSpline.PointsCount)
				{
					var point1 = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[point1Index].Position);
					var rotatedPoint1 = VectorUtils.RotateAround(point1, worldPoint, rotationDiff);
					EditorState.CurrentSpline.SetPoint(point1Index, handleTransform.InverseTransformPoint(rotatedPoint1));
				}

				if (point2Index >= 0 && point2Index < EditorState.CurrentSpline.PointsCount)
				{
					var point2 = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[point2Index].Position);
					var rotatedPoint2 = VectorUtils.RotateAround(point2, worldPoint, rotationDiff);
					EditorState.CurrentSpline.SetPoint(point2Index, handleTransform.InverseTransformPoint(rotatedPoint2));
				}

				EditorState.lastRotation = rotation;
				EditorState.wasSplineModified = true;
			}
			else if ((EditorState.isRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				EditorState.lastRotation = handleRotation;
				EditorState.isRotating = false;
				EditorState.wasSplineModified = true;
			}
		}

		private static void DrawSpline(BezierSpline spline, int selectedSplineIndex = -1)
		{
			var transformHandle = spline.transform;
			for (var i = 0; i < spline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				var p0 = transformHandle.TransformPoint(spline.Points[curveStartIndex].Position);
				var p1 = transformHandle.TransformPoint(spline.Points[curveStartIndex + 1].Position);
				var p2 = transformHandle.TransformPoint(spline.Points[curveStartIndex + 2].Position);
				var p3 = transformHandle.TransformPoint(spline.Points[curveStartIndex + 3].Position);

				var splineColor = (i == selectedSplineIndex && EditorState.DrawPoints) ? EditorSettings.SelectedCurveColor : EditorSettings.SplineColor;
				Handles.DrawBezier(p0, p3, p1, p2, splineColor, null, EditorSettings.SplineWidth * 1.5f);
			}
		}

	}

}
