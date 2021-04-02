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

		//TODO: Change to static editor
		private static Quaternion handleRotation;
		private static BezierSpline spline;
		private static Event currentEvent;
		private static Transform handleTransform;

		private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

		private bool IsAnyPointSelected => selectedIndex != -1 && spline !=null && selectedIndex < spline.PointsCount;
		private bool IsAnyCurveSelected => selectedCurveIndex != -1 && spline != null && selectedCurveIndex < spline.CurveCount;
		private bool IsMoreThanOneCurve => spline.CurveCount > 1;

		private SplineEditorState editorState;

		private bool isRotating;
		private Quaternion lastRotation;


		private void OnEnable()
		{
			SplineMeTools.InitializeGUI(ref editorState);
			
			editorState.title = "Spline Tools";
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
			spline = target as BezierSpline;
			EditorGUI.BeginChangeCheck();
			var prevEnabled = GUI.enabled;
			GUI.enabled = IsMoreThanOneCurve;
			bool loop = EditorGUILayout.Toggle("Loop", spline.IsLoop);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Toggle Loop");
				EditorUtility.SetDirty(spline);
				spline.IsLoop = loop;
			}

			GUI.enabled = prevEnabled;

			if (IsAnyPointSelected)
			{
				DrawSelectedPointInspector();
			}

			if(GUILayout.Button("Cast Curve Points"))
			{
				Undo.RecordObject(spline, "Cast Curve Points");
				spline.CastCurve();
				EditorUtility.SetDirty(spline);
			}

			GUI.enabled = IsAnyCurveSelected;
			if (GUILayout.Button("Add Mid Curve"))
			{
				Undo.RecordObject(spline, "Add Mid Curve");
				var wasLastPoint = selectedIndex == spline.PointsCount - 1;
				spline.AddMidCurveAndApplyConstraints(selectedCurveIndex);
				if(wasLastPoint && !spline.IsLoop)
				{
					SelectIndex(spline.PointsCount - 4);
				}
				else if(selectedIndex!=0 && !(wasLastPoint && spline.IsLoop))
				{
					SelectIndex(selectedIndex+3);
				}
				else
				{
					SelectIndex(3);
				}
				EditorUtility.SetDirty(spline);
			}
			GUI.enabled = prevEnabled;

			if (GUILayout.Button("Factor Curve"))
			{
				Undo.RecordObject(spline, "Factor Curve");
				var wasLastPoint = selectedIndex == spline.PointsCount - 1;
				spline.FactorCurve();
				if(selectedIndex!=-1)
				{
					SelectIndex(selectedIndex * 2);
				}
				EditorUtility.SetDirty(spline);
			}

			GUI.enabled = IsMoreThanOneCurve && (!spline.IsLoop || spline.CurveCount > 2);
			if (GUILayout.Button("Simplify Curve"))
			{
				Undo.RecordObject(spline, "Simplify Curve");
				var wasLastPoint = selectedIndex == spline.PointsCount - 1;
				spline.SimplifyCurve();
				if (selectedIndex != -1)
				{
					SelectIndex(selectedIndex/2);
				}
				EditorUtility.SetDirty(spline);
			}

			GUI.enabled = prevEnabled;
		}

		private void DrawSelectedPointInspector()
		{
			GUILayout.Label("Selected Point");
			EditorGUI.BeginChangeCheck();
			Vector3 point = EditorGUILayout.Vector3Field("Position", spline.Points[selectedIndex].position);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.UpdatePoint(selectedIndex, point);
			}

			EditorGUI.BeginChangeCheck();
			BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Change Point Mode");
				spline.SetControlPointMode(selectedIndex, mode);
				EditorUtility.SetDirty(spline);
			}
		}

		//TODO: Change to static methods
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			spline = curve;
			handleTransform = spline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
			DrawSpline();
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

		private static void DrawSpline()
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
				var p0 = DrawPoint(curveStartIndex);
				var p1 = DrawPoint(curveStartIndex+1);
				var p2 = DrawPoint(curveStartIndex+2);
				var p3 = DrawPoint(curveStartIndex+3);

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
			Handles.color = SplineMeTools.SegmentsColor;
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
			UpdateSelectedIndex(selectedIndex);
		}

		private void RemoveSelectedCurve()
		{
			if (!IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(spline, "Remove Curve");
			EditorUtility.SetDirty(spline);
			spline.RemoveCurve(selectedCurveIndex);
			var nextSelectedIndex = Mathf.Min(selectedIndex, spline.PointsCount - 1);
			UpdateSelectedIndex(nextSelectedIndex);
		}

		private void DrawLine(Vector3 p0, Vector3 p1, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p0, p1);
		}

		private Vector3 DrawPoint(int index)
		{
			var mode = spline.GetControlPointMode(index);
			var pointColor = SplineMeTools.ModeColors[(int)mode];

			return DrawPoint(index, pointColor);
		}

		private Vector3 DrawPoint(int index, Color pointColor)
		{
			var point = handleTransform.TransformPoint(spline.Points[index].position);
			var size = HandleUtility.GetHandleSize(point);

			if(index==0 || index == spline.PointsCount-1)
			{
				size *= 2f;
			}

			Handles.color = pointColor;
			if (Handles.Button(point, handleRotation, size * SplineMeTools.HandlePointSize, size * SplineMeTools.PickPointSize, Handles.DotHandleCap))
			{
				SelectIndex(index);
				Repaint();
			}

			if (selectedIndex == index)
			{
				EditorGUI.BeginChangeCheck();
				if(Tools.current == Tool.None || Tools.current == Tool.Move)
				{
					point = Handles.DoPositionHandle(point, handleRotation);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(spline, "Move Line Point");
						EditorUtility.SetDirty(spline);
						spline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					}
				} else if(Tools.current == Tool.Rotate && index % 3 == 0)
				{
					var rotation = Handles.DoRotationHandle(handleRotation, point);
					if (EditorGUI.EndChangeCheck())
					{
						if(!isRotating)
						{
							lastRotation = rotation;
							isRotating = true;
						}

						var rotationDiff = rotation * Quaternion.Inverse(lastRotation);

						Undo.RecordObject(spline, "Rotate Line Point");
						EditorUtility.SetDirty(spline);
						var point1Index = index == spline.PointsCount-1 && spline.IsLoop ? 1 : index + 1;
						var point2Index = index == 0 && spline.IsLoop ? spline.PointsCount - 2 : index - 1;

						if(point1Index >= 0 && point2Index < spline.PointsCount)
						{
							var point1 = handleTransform.TransformPoint(spline.Points[point1Index].position);
							var rotatedPoint1 = RotateAround(point1, point, rotationDiff);
							spline.UpdatePoint(point1Index, handleTransform.InverseTransformPoint(rotatedPoint1));
						}

						if(point2Index >= 0 && point2Index < spline.PointsCount)
						{
							var point2 = handleTransform.TransformPoint(spline.Points[point2Index].position);
							var rotatedPoint2 = RotateAround(point2, point, rotationDiff);
							spline.UpdatePoint(point2Index, handleTransform.InverseTransformPoint(rotatedPoint2));
						}

						lastRotation = rotation;
					}
					else if(isRotating && currentEvent.type == EventType.MouseUp)
					{
						lastRotation = handleRotation;
						isRotating = false; 
					}
				}
				
			}

			return point;
		}

		private static Vector3 RotateAround(Vector3 target, Vector3 pivotPoint, Quaternion rot)
		{
			return rot * (target - pivotPoint) + pivotPoint;
		}

		private void SelectIndex(int index)
		{
			if(index == -1 && selectedIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(index);
		}

		private void UpdateSelectedIndex(int index)
		{
			selectedIndex = index;
			selectedCurveIndex = index!=-1 ? index / 3 : -1;
			if (selectedCurveIndex == spline.CurveCount)
			{
				selectedCurveIndex = spline.IsLoop ? 0 : spline.CurveCount-1;
			}
			editorState.isMoreThanOneCurve = IsMoreThanOneCurve;
			editorState.isAnyCurveSelected = IsAnyCurveSelected;
		}

	}

}