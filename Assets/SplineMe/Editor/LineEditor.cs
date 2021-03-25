using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	[CustomEditor(typeof(Line))]
	public class LineEditor : UnityEditor.Editor
	{

		private int selectedIndex = -1;

		private Vector3 lineStart, lineEnd;
		private Quaternion handleRotation;

		private Line line;
		private Event currentEvent;
		private Transform handleTransform;

		private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

		private bool IsAnyPointSelected => selectedIndex != -1;

		private LineEditorState editorState;
		
		private void OnEnable()
		{
			LineEditorTools.InitializeGUI(ref editorState);

			editorState.AddPointAction = AddPoint;
			editorState.RemovePointAction = RemoveSelectedPoint;
			editorState.isAnyPointSelected = IsAnyPointSelected;
		}

		private void OnDisable()
		{
			LineEditorTools.ReleaseGUI(ref editorState);
		}

		private void OnSceneGUI()
		{
			currentEvent = Event.current;
			line = target as Line;
			handleTransform = line.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			CheckInput();

			if(selectedIndex > line.PointsCount)
			{
				SelectIndex(line.PointsCount - 1);
			}

			LineEditorTools.DrawGUI(ref editorState);

			DrawLine();
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
			var currentIndex = IsAnyPointSelected ? selectedIndex : line.PointsCount-1;
			if(currentIndex != -1)
			{
				var referencePoint = line.Points[currentIndex];
				Undo.RecordObject(line, "Add Line Point");
				EditorUtility.SetDirty(line);
				line.AddPoint(referencePoint.position, currentIndex);
			}
			else
			{
				Undo.RecordObject(line, "Add Line Point");
				EditorUtility.SetDirty(line);
				line.AddPoint(Vector3.zero);
				SelectIndex(0);
			}
		}

		private void RemoveSelectedPoint()
		{
			if(!IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(line, "Remove Line Point");
			EditorUtility.SetDirty(line);
			line.RemovePoint(selectedIndex);
			selectedIndex = Mathf.Min(selectedIndex, line.PointsCount - 1);
		}

		private void DrawLine()
		{
			if(line.PointsCount==0)
			{
				return;
			}

			var endLineColor = line.PointsCount == 1 ? LineEditorTools.LineStartPointColor : LineEditorTools.LineEndPointColor;
			lineStart = DrawPoint(0, endLineColor); //Line end
			for (var i = 1; i < line.PointsCount; i++)
			{
				var pointColor = i == line.PointsCount - 1
										? LineEditorTools.LineStartPointColor //Line beginning
										: LineEditorTools.LineMidPointColor;

				lineEnd = DrawPoint(i, pointColor);
				DrawLine(lineStart, lineEnd);
				lineStart = lineEnd;
			}
		}

		private void DrawLine(Vector3 p0, Vector3 p1)
		{
			Handles.color = LineEditorTools.LineColor;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(line.Points[index].position);
			float size = HandleUtility.GetHandleSize(point);
			Handles.color = pointColor;
			if (Handles.Button(point, handleRotation, size * LineEditorTools.HandlePointSize, size * LineEditorTools.PickPointSize, Handles.DotHandleCap))
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
					Undo.RecordObject(line, "Move Line Point");
					EditorUtility.SetDirty(line);
					line.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
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