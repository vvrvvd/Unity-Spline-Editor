using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	[CustomEditor(typeof(BezierCurve))]
	public class BezierCurveEditor : UnityEditor.Editor
	{
		private const int lineSteps = 10;

		private int selectedIndex = -1;

		private Vector3 lineStart, lineEnd;
		private Quaternion handleRotation;

		private BezierCurve curve;
		private Event currentEvent;
		private Transform handleTransform;

		private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

		private bool IsAnyPointSelected => selectedIndex != -1;

		private LineEditorState editorState;

		private void OnEnable()
		{
			SplineMeTools.InitializeGUI(ref editorState);
			
			editorState.AddPointAction = AddPoint;
			editorState.RemovePointAction = RemoveSelectedPoint;
			editorState.isAnyPointSelected = IsAnyPointSelected;
		}

		private void OnDisable()
		{
			SplineMeTools.ReleaseGUI(ref editorState);
		}

		private void OnSceneGUI()
		{

			currentEvent = Event.current;
			curve = target as BezierCurve;
			handleTransform = curve.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			CheckInput();

			if (selectedIndex > curve.PointsCount)
			{
				SelectIndex(curve.PointsCount - 1);
			}

			DrawLine();
			ShowDirections();
		}

		private void CheckInput()
		{
			var currentKeyCode = currentEvent.keyCode;

			if (currentEvent.type == EventType.KeyDown)
			{
				if (!pressedKeys.Contains(currentKeyCode))
				{
					OnKeyPressed(currentKeyCode);
					pressedKeys.Add(currentKeyCode);
				}
				else
				{
					OnKeyHeld(currentKeyCode);
				}
			}
			else if (currentEvent.type == EventType.KeyUp && pressedKeys.Contains(currentKeyCode))
			{
				OnKeyReleased(currentKeyCode);
				pressedKeys.Remove(currentKeyCode);
			}

		}

		private void OnKeyPressed(KeyCode pressedKey)
		{
			if (pressedKey == KeyCode.Equals)
			{
				AddPoint();
			}
			else if (pressedKey == KeyCode.Minus)
			{
				RemoveSelectedPoint();
			}
		}

		private void OnKeyHeld(KeyCode heldKey) { }

		private void OnKeyReleased(KeyCode releasedKey) { }

		private void AddPoint()
		{
			var currentIndex = IsAnyPointSelected ? selectedIndex : curve.PointsCount - 1;
			if (currentIndex != -1)
			{
				var referencePoint = curve.Points[currentIndex];
				Undo.RecordObject(curve, "Add Line Point");
				EditorUtility.SetDirty(curve);
				curve.AddPoint(referencePoint.position, currentIndex);
			}
			else
			{
				Undo.RecordObject(curve, "Add Line Point");
				EditorUtility.SetDirty(curve);
				curve.AddPoint(Vector3.zero);
				SelectIndex(0);
			}
		}

		private void RemoveSelectedPoint()
		{
			if (!IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(curve, "Remove Line Point");
			EditorUtility.SetDirty(curve);
			curve.RemovePoint(selectedIndex);
			selectedIndex = Mathf.Min(selectedIndex, curve.PointsCount - 1);
		}

		private void DrawLine()
		{
			if (curve.PointsCount == 0)
			{
				return;
			}

			var endLineColor = curve.PointsCount == 1 ? SplineMeTools.LineStartPointColor : SplineMeTools.LineEndPointColor;
			DrawPoint(0, endLineColor);
			lineStart = curve.GetPoint(1f);//Line end
			for (var i = lineSteps-1; i >= 0; i--)
			{
				var t = i / (float)lineSteps;
				lineEnd = curve.GetPoint(t);
				DrawLine(lineStart, lineEnd);
				lineStart = lineEnd;
			}

			DrawPoint(1, SplineMeTools.LineMidPointColor);
			DrawPoint(2, SplineMeTools.LineMidPointColor);
			lineEnd = DrawPoint(3, SplineMeTools.LineStartPointColor);
		}

		private void ShowDirections()
		{
			Handles.color = Color.green;
			Vector3 point = curve.GetPoint(1f);
			Handles.DrawLine(point, point - curve.GetDirection(1f) * SplineMeTools.DirectionScale);
			for (int i = lineSteps-1; i >= 0; i--)
			{
				point = curve.GetPoint(i / (float)lineSteps);
				Handles.DrawLine(point, point - curve.GetDirection(i / (float)lineSteps) * SplineMeTools.DirectionScale);
			}
		}

		private void DrawLine(Vector3 p0, Vector3 p1)
		{
			DrawLine(p0, p1, SplineMeTools.LineColor);
		}

		private void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(curve.Points[index].position);
			float size = HandleUtility.GetHandleSize(point);
			Handles.color = pointColor;
			if (Handles.Button(point, handleRotation, size * SplineMeTools.HandlePointSize, size * SplineMeTools.PickPointSize, Handles.DotHandleCap))
			{
				SelectIndex(index);
				Repaint();
			}

			if (selectedIndex == index)
			{
				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, handleRotation);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(curve, "Move Line Point");
					EditorUtility.SetDirty(curve);
					curve.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
				}
			}

			return point;
		}

		private void SelectIndex(int index)
		{
			selectedIndex = index;
			editorState.isAnyPointSelected = IsAnyPointSelected;
		}


	}

}