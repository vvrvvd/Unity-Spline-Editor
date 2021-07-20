using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		private void InitializeSceneGUI()
		{
			editorState.isScaling = false;
			editorState.isRotating = false;
			editorState.isDraggingPoint = false;
			editorState.lastRotation = Quaternion.identity;
		}

		private void DrawSceneGUI()
		{
			if (editorState.DrawSpline)
			{
				DrawSpline(editorState.CurrentSpline, editorState.SelectedCurveIndex);
			}

			if (editorState.IsDrawerMode)
			{
				DrawCurveModeSceneGUI();
			}

			if (editorState.DrawPoints)
			{
				DrawPoints();
			}

			if (editorState.DrawNormals)
			{
				DrawNormals();
			}

		}

		private void DrawPoints()
		{
			Vector3 p0, p1, p2, p3;
			for (var i = 0; i < editorState.CurrentSpline.CurvesCount; i++)
			{
				var curveStartIndex = i * 3;
				if(i > 0 && i%3!=0)
				{
					p0 = handleTransform.TransformPoint(editorState.CurrentSpline.Points[curveStartIndex].position);
				}
				else
				{
					p0 = DrawPoint(curveStartIndex);
				}
				p1 = DrawPoint(curveStartIndex + 1);
				p2 = DrawPoint(curveStartIndex + 2);
				p3 = handleTransform.TransformPoint(editorState.CurrentSpline.Points[curveStartIndex + 3].position);

				if (!editorState.IsDrawerMode || i < editorState.CurrentSpline.CurvesCount - 1)
				{
					p3 = DrawPoint(curveStartIndex + 3);
				}

				DrawLine(p0, p1, editorSettings.TangentLineColor);
				DrawLine(p3, p2, editorSettings.TangentLineColor);
			}
		}

		private void DrawNormals()
		{
			for (var i = 0; i < editorState.CurrentSpline.PointsCount; i+=3)
			{
				var point = handleTransform.TransformPoint(editorState.CurrentSpline.Points[i].position);
				var normalIndex = i / 3;
				if (editorState.CurrentSpline.Normals.Length <= normalIndex)
				{
					editorState.CurrentSpline.RecalculateNormals();
				}
				var normalVector = editorState.CurrentSpline.GetNormal(normalIndex);
				var normalLength = editorSettings.NormalVectorLength;
				Handles.color = editorSettings.NormalsColor;
				Handles.DrawLine(point, point + editorState.CurrentSpline.transform.TransformDirection((normalVector) * normalLength));
			}
			
		}

		private static void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index)
		{
			var mode = editorState.CurrentSpline.GetControlPointMode(index);
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
			var worldPoint = handleTransform.TransformPoint(editorState.CurrentSpline.Points[index].position);
			var size = editorSettings.ScalePointOnScreen ? HandleUtility.GetHandleSize(worldPoint) : 1f;

			Handles.color = pointColor;
			var handleSize = index % 3 == 0 ? editorSettings.MainPointSize : editorSettings.TangentPointSize;

			if (Handles.Button(worldPoint, handleRotation, size * handleSize, size * handleSize, Handles.DotHandleCap))
			{
				SelectIndex(index);
				Repaint();
				editorState.wasSplineModified = true;
			}

			if (editorState.SelectedPointIndex == index)
			{
				if (editorState.savedTool == Tool.Rotate && index % 3 == 0)
				{
					if (editorState.IsNormalsEditorMode)
					{
						RotateNormals(index, worldPoint);
					}
					else
					{
						RotatePoints(index, worldPoint);
					}
				}
				else if(editorState.savedTool == Tool.Scale && index % 3 == 0)
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
					Undo.RecordObject(editorState.CurrentSpline, "Cast Spline Point To Mouse");
					worldPoint = castedPosition;
					editorState.CurrentSpline.SetPoint(index, handleTransform.InverseTransformPoint(worldPoint));
					editorState.wasSplineModified = true;
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				worldPoint = Handles.DoPositionHandle(worldPoint, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					Undo.RecordObject(editorState.CurrentSpline, "Move Spline Point");
					editorState.isDraggingPoint = true;
					editorState.CurrentSpline.SetPoint(index, handleTransform.InverseTransformPoint(worldPoint));
					editorState.wasSplineModified = true;
				}
				else if ((editorState.isDraggingPoint && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					Undo.RecordObject(editorState.CurrentSpline, "Move Spline Point");
					editorState.CurrentSpline.SetPoint(index, handleTransform.InverseTransformPoint(worldPoint));
					editorState.isDraggingPoint = false;
					castSelectedPointFlag = false;
					editorState.wasSplineModified = true;
				}
			}
		}

		private void ScalePoint(int index, Vector3 worldPoint)
		{
			var normalIndex = index / 3;
			var pointScaleIndex = index / 3;
			
			var handleSize = HandleUtility.GetHandleSize(worldPoint);
			var pointScale = editorState.CurrentSpline.PointsScales[pointScaleIndex];
			var normalVector = editorState.CurrentSpline.GetNormal(normalIndex);
			var tangentVector = editorState.CurrentSpline.Tangents[normalIndex];

			var normalWorldVector = editorState.CurrentSpline.transform.TransformDirection(normalVector).normalized;
			var tangentWorldVector = editorState.CurrentSpline.transform.TransformDirection(tangentVector).normalized;
			var baseHandleRotation = Quaternion.LookRotation(normalWorldVector, tangentWorldVector);

			EditorGUI.BeginChangeCheck();
			editorState.lastScale = Handles.DoScaleHandle(editorState.isScaling ? editorState.lastScale : pointScale, worldPoint, baseHandleRotation, handleSize);
			var wasChanged = EditorGUI.EndChangeCheck();
			if (wasChanged)
			{

				if (!editorState.isScaling)
				{
					editorState.isScaling = true;
					editorState.lastScale = new Vector3(editorState.lastScale.x, editorState.lastScale.y, editorState.lastScale.z);
					return;
				}

				Undo.RecordObject(editorState.CurrentSpline, "Scale Spline Point");
				editorState.isScaling = true;

				editorState.CurrentSpline.SetPointsScale(pointScaleIndex, editorState.lastScale);
				editorState.wasSplineModified = true;
			}
			else if ((editorState.isScaling && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				editorState.CurrentSpline.SetPointsScale(pointScaleIndex, editorState.lastScale);
				editorState.isScaling = false;
				editorState.wasSplineModified = true;
			}
		}

		private void RotatePoints(int index, Vector3 worldPoint)
		{
			EditorGUI.BeginChangeCheck();
			var rotation = Handles.DoRotationHandle(handleRotation, worldPoint);
			if (EditorGUI.EndChangeCheck())
			{
				if (!editorState.isRotating)
				{
					editorState.lastRotation = handleRotation;
					editorState.isRotating = true;
				}

				var rotationDiff = rotation * Quaternion.Inverse(editorState.lastRotation);

				Undo.RecordObject(editorState.CurrentSpline, "Rotate Spline Point");
				var point1Index = index == editorState.CurrentSpline.PointsCount - 1 && editorState.CurrentSpline.IsLoop ? 1 : index + 1;
				var point2Index = index == 0 && editorState.CurrentSpline.IsLoop ? editorState.CurrentSpline.PointsCount - 2 : index - 1;

				if (point1Index >= 0 && point1Index < editorState.CurrentSpline.PointsCount)
				{
					var point1 = handleTransform.TransformPoint(editorState.CurrentSpline.Points[point1Index].position);
					var rotatedPoint1 = VectorUtils.RotateAround(point1, worldPoint, rotationDiff);
					editorState.CurrentSpline.SetPoint(point1Index, handleTransform.InverseTransformPoint(rotatedPoint1));
				}

				if (point2Index >= 0 && point2Index < editorState.CurrentSpline.PointsCount)
				{
					var point2 = handleTransform.TransformPoint(editorState.CurrentSpline.Points[point2Index].position);
					var rotatedPoint2 = VectorUtils.RotateAround(point2, worldPoint, rotationDiff);
					editorState.CurrentSpline.SetPoint(point2Index, handleTransform.InverseTransformPoint(rotatedPoint2));
				}

				editorState.lastRotation = rotation;
				editorState.wasSplineModified = true;
			}
			else if ((editorState.isRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				editorState.lastRotation = handleRotation;
				editorState.isRotating = false;
				editorState.wasSplineModified = true;
			}
		}

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

				var splineColor = (i == selectedSplineIndex && editorState.DrawPoints) ? editorSettings.SelectedCurveColor : editorSettings.SplineColor;
				Handles.DrawBezier(p0, p3, p1, p2, splineColor, null, editorSettings.SplineWidth * 1.5f);
			}
		}

	}

}
