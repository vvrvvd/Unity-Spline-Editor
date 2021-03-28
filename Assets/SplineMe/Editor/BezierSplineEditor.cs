using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public class BezierSplineEditor : UnityEditor.Editor
	{

		private int selectedIndex = -1; 
		private int selectedCurveIndex = -1;

		private Quaternion handleRotation;

		private BezierSpline spline;
		private Event currentEvent;
		private Transform handleTransform;

		private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

		private bool IsAnyPointSelected => selectedIndex != -1;
		private bool IsAnyCurveSelected => selectedCurveIndex > 0;

		private SplineEditorState editorState;

		private void OnEnable()
		{
			SplineMeTools.InitializeGUI(ref editorState);
			
			editorState.title = "Spline Me";
			editorState.AddCurveAction = AddCurve;
			editorState.RemoveCurveAction = RemoveSelectedCurve;
			editorState.isAnyCurveSelected = IsAnyPointSelected;
		}

		private void OnDisable()
		{
			SplineMeTools.ReleaseGUI(ref editorState);
		}
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			spline = target as BezierSpline;
			if (GUILayout.Button("Add Curve"))
			{
				Undo.RecordObject(spline, "Add Curve");
				spline.AddCurve();
				EditorUtility.SetDirty(spline);
			}
		}

		private void OnSceneGUI()
		{
			currentEvent = Event.current;
			spline = target as BezierSpline;
			handleTransform = spline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			CheckInput();

			if (selectedIndex > spline.PointsCount)
			{
				SelectIndex(spline.PointsCount - 1);
			}
			
			SplineMeTools.DrawGUI(ref editorState);

			DrawSpline();

			if(editorState.showDirectionsLines)
			{
				DrawDirections();
			}

			if (editorState.showSegmentsPoints)
			{
				DrawSegments();
			}

			if(editorState.showPointsHandles)
			{
				DrawPoints();
			}
			else
			{
				SelectIndex(-1);
			}
		}

		private void DrawSpline()
		{
			for (var i = 0; i < spline.CurveCount; i++)
			{
				var curveStartIndex = i * 3;
				var p0 = handleTransform.TransformPoint(spline.Points[curveStartIndex].position);
				var p1 = handleTransform.TransformPoint(spline.Points[curveStartIndex + 1].position);
				var p2 = handleTransform.TransformPoint(spline.Points[curveStartIndex + 2].position);
				var p3 = handleTransform.TransformPoint(spline.Points[curveStartIndex + 3].position);

				Handles.DrawBezier(p0, p3, p1, p2, SplineMeTools.LineColor, null, SplineMeTools.LineWidth * 1.5f);
			}
		}

		private void DrawPoints()
		{
			for (var i = 0; i < spline.CurveCount; i++)
			{
				var curveStartIndex = i * 3;
				var endCurveColor = curveStartIndex + 3 == spline.PointsCount - 1 ? SplineMeTools.LineEndPointColor : SplineMeTools.LineMidPointColor;
				var startSplineColor = curveStartIndex == 0 ? SplineMeTools.LineStartPointColor : SplineMeTools.LineMidPointColor;
				var p0 = DrawPoint(curveStartIndex, startSplineColor);
				var p1 = DrawPoint(curveStartIndex + 1, SplineMeTools.LineMidPointColor);
				var p2 = DrawPoint(curveStartIndex + 2, SplineMeTools.LineMidPointColor);
				var p3 = DrawPoint(curveStartIndex + 3, endCurveColor);

				DrawLine(p0, p1, SplineMeTools.TangentLineColor);
				DrawLine(p3, p2, SplineMeTools.TangentLineColor);
			}
		}

		private void DrawDirections()
		{
			var point = spline.GetPoint(1f);
			Handles.DrawLine(point, point - spline.GetDirection(1f) * SplineMeTools.DirectionScale);

			var curveSteps = SplineMeTools.CurveStepsCount * spline.CurveCount;
			for (int i = curveSteps - 1; i >= 0; i--)
			{
				point = spline.GetPoint(i / (float)curveSteps);
				Handles.color = SplineMeTools.DirectionLineColor;
				Handles.DrawLine(point, point - spline.GetDirection(i / (float)curveSteps) * SplineMeTools.DirectionScale);
			}
		}

		private void DrawSegments()
		{
			var point = spline.GetPoint(1f);
			Handles.color = Color.magenta;
			var curveSteps = SplineMeTools.CurveStepsCount * spline.CurveCount;
			for (int i = curveSteps - 1; i >= 0; i--)
			{
				var size = HandleUtility.GetHandleSize(point);
				point = spline.GetPoint(i / (float)curveSteps);
				Handles.Button(point, handleRotation, size * SplineMeTools.HandleSegmentSize, size * SplineMeTools.HandleSegmentSize, Handles.DotHandleCap);
			}
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
				AddCurve();
			}
			else if (pressedKey == KeyCode.Minus)
			{
				RemoveSelectedCurve();
			}
		}

		private void OnKeyHeld(KeyCode heldKey) { }

		private void OnKeyReleased(KeyCode releasedKey) { }

		private void AddCurve()
		{
			Undo.RecordObject(spline, "Add Curve");
			EditorUtility.SetDirty(spline);
			spline.AddCurve();
		}

		private void RemoveSelectedCurve()
		{
			if (!IsAnyPointSelected || selectedCurveIndex ==0)
			{
				return;
			}

			Undo.RecordObject(spline, "Remove Curve");
			EditorUtility.SetDirty(spline);
			spline.RemoveCurve(selectedCurveIndex);
			selectedIndex = Mathf.Min(selectedIndex, spline.PointsCount - 1);
		}

		private void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(spline.Points[index].position);
			var size = HandleUtility.GetHandleSize(point);
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
					Undo.RecordObject(spline, "Move Line Point");
					EditorUtility.SetDirty(spline);
					spline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
				}
			}

			return point;
		}

		private void SelectIndex(int index)
		{
			if(selectedIndex==index)
			{
				return;
			}

			selectedIndex = index;
			selectedCurveIndex = index / 3;
			editorState.isAnyCurveSelected = IsAnyCurveSelected;
		}


	}

}