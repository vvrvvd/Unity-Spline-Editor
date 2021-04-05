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

		private bool IsAnyPointSelected => selectedIndex != -1 && spline != null && selectedIndex < spline.PointsCount;
		private bool IsAnyCurveSelected => selectedCurveIndex != -1 && spline != null && selectedCurveIndex < spline.CurveCount;
		private bool IsMoreThanOneCurve => spline.CurveCount > 1;

		private SplineEditorState editorState;

		private bool isRotating;
		private Quaternion lastRotation;

		private bool isCurveDrawerMode;
		private Vector3 curveDrawerPosition = Vector3.zero;
		private Vector3[] newCurvePoints = new Vector3[4];
		private bool isDraggingNewCurve = false;
		private bool firstControlPointSet = false;
		private bool secondControlPointSet = false;


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

			if (GUILayout.Button("Cast Curve Points"))
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
				if (wasLastPoint && !spline.IsLoop)
				{
					SelectIndex(spline.PointsCount - 4);
				}
				else if (selectedIndex != 0 && !(wasLastPoint && spline.IsLoop))
				{
					SelectIndex(selectedIndex + 3);
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
				spline.FactorCurve();
				if (selectedIndex != -1)
				{
					SelectIndex(selectedIndex * 2);
				}
				EditorUtility.SetDirty(spline);
			}

			GUI.enabled = IsMoreThanOneCurve && (!spline.IsLoop || spline.CurveCount > 2);
			if (GUILayout.Button("Simplify Curve"))
			{
				Undo.RecordObject(spline, "Simplify Curve");
				spline.SimplifyCurve();
				if (selectedIndex != -1)
				{
					SelectIndex(selectedIndex / 2);
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
			BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
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

			if (editorState.showDirectionsLines)
			{
				DrawDirections();
			}

			if (editorState.showSegmentsPoints)
			{
				DrawSegments();
			}

			if (editorState.showPointsHandles)
			{
				DrawPoints();
			}
			else
			{
				SelectIndex(-1);
			}

			if (isCurveDrawerMode)
			{
				DrawCurveMode();
			}

		}

		private void DrawCurveMode()
		{
			var curveDrawerPointLocal = curveDrawerPosition;
			var curveDrawerPointWorld = spline.transform.TransformPoint(curveDrawerPointLocal);
			var size = HandleUtility.GetHandleSize(curveDrawerPointWorld);
			Handles.color = Color.green;
			Handles.Button(curveDrawerPointWorld, handleRotation, size * SplineMeTools.DrawCurveSphereSize, size * SplineMeTools.DrawCurveSphereSize, Handles.SphereHandleCap);

			EditorGUI.BeginChangeCheck();
			var newEndPositionGlobal = Handles.DoPositionHandle(curveDrawerPointWorld, handleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Move Drawer Point");
				EditorUtility.SetDirty(spline);
				isDraggingNewCurve = true;
				curveDrawerPosition = spline.transform.InverseTransformPoint(newEndPositionGlobal);
				UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
			}
			else if ((isDraggingNewCurve && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				var defaultDrawerPosition = spline.Points[spline.PointsCount - 1].position;
				InitializeDrawCurveMode(defaultDrawerPosition);
				isDraggingNewCurve = false;
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

			var firstPointDistance = SplineMeTools.DrawCurveFirstControlPoint * SplineMeTools.DrawCurveSegmentLength;
			var secondPointDistance = SplineMeTools.DrawCurveSecondControlPoint * SplineMeTools.DrawCurveSegmentLength;

			if (distance < firstPointDistance)
			{
				firstControlPointSet = false;
				newCurvePoints[1] = newEndPosition;
			}
			else if (distance < secondPointDistance)
			{
				secondControlPointSet = false;
				newCurvePoints[2] = newEndPosition;

				if (!firstControlPointSet)
				{
					var prevFirstPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) / SplineMeTools.DrawCurveSegmentLength;
					var normalizedCurrentDistance = distance / SplineMeTools.DrawCurveSegmentLength;
					var normalizedDistancesDiff = normalizedCurrentDistance - prevFirstPointNormalizedDistance;
					newCurvePoints[1] = Vector3.Lerp(newCurvePoints[1], newEndPosition, (SplineMeTools.DrawCurveFirstControlPoint - prevFirstPointNormalizedDistance) / normalizedDistancesDiff);
					firstControlPointSet = true;
				}

			}
			else if (distance < SplineMeTools.DrawCurveSegmentLength)
			{
				newCurvePoints[3] = newEndPosition;

				if (!secondControlPointSet)
				{
					var prevSecondPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[2]) / SplineMeTools.DrawCurveSegmentLength;
					var normalizedCurrentDistance = distance / SplineMeTools.DrawCurveSegmentLength;
					var normalizedDistancesDiff = normalizedCurrentDistance - prevSecondPointNormalizedDistance;
					newCurvePoints[2] = Vector3.Lerp(newCurvePoints[2], newEndPosition, (SplineMeTools.DrawCurveSecondControlPoint - prevSecondPointNormalizedDistance) / normalizedDistancesDiff);
					secondControlPointSet = true;
				}
			}
			else
			{
				var prevLastPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[3]) / SplineMeTools.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / SplineMeTools.DrawCurveSegmentLength;
				var normalizedDistancesDiff = normalizedCurrentDistance - prevLastPointNormalizedDistance;
				newCurvePoints[3] = Vector3.Lerp(newCurvePoints[3], newEndPosition, (1f - prevLastPointNormalizedDistance) / normalizedDistancesDiff);
				SpawnDrawCurveModeSpline();
				InitializeDrawCurveMode(newCurvePoints[3]);
				UpdateNewDrawCurvePainterPosition(newEndPosition);
			}

		}

		private void SpawnDrawCurveModeSpline()
		{
			var p3 = newCurvePoints[3];
			spline.GetInverseControlPoints(newCurvePoints[0], newCurvePoints[3], newCurvePoints[1], newCurvePoints[2], SplineMeTools.DrawCurveFirstControlPoint, SplineMeTools.DrawCurveSecondControlPoint, out var p1, out var p2);
			spline.AddCurve(p1, p2, p3, BezierControlPointMode.Free);
		}

		private void InitializeDrawCurveMode(Vector3 newPoint)
		{
			for (var i = 0; i < newCurvePoints.Length; i++)
			{
				newCurvePoints[i] = newPoint;
			}

			curveDrawerPosition = newPoint;
			firstControlPointSet = false;
			secondControlPointSet = false;
		}

		public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
		{
			Vector3 AB = b - a;
			Vector3 AV = value - a;
			return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
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
				var p1 = DrawPoint(curveStartIndex + 1);
				var p2 = handleTransform.TransformPoint(spline.Points[curveStartIndex + 2].position);
				var p3 = handleTransform.TransformPoint(spline.Points[curveStartIndex + 3].position);
				if (!isCurveDrawerMode || i < spline.CurveCount - 1)
				{
					p2 = DrawPoint(curveStartIndex + 2);
					p3 = DrawPoint(curveStartIndex + 3);
				}

				DrawLine(p0, p1, SplineMeTools.TangentLineColor);
				if (!isCurveDrawerMode || i < spline.CurveCount - 1)
				{
					DrawLine(p3, p2, SplineMeTools.TangentLineColor);
				}
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
			else if (pressedKey == KeyCode.C)
			{
				ToggleDrawCurveMode();
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

		private void ToggleDrawCurveMode()
		{
			ToggleDrawCurveMode(!isCurveDrawerMode);
		}

		private void ToggleDrawCurveMode(bool state)
		{
			isCurveDrawerMode = state;
		
			if (state)
			{
				var lastPoint = spline.Points[spline.PointsCount - 1];
				InitializeDrawCurveMode(lastPoint.position);
				SelectIndex(-1);
			}
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

			if (index == 0 || index == spline.PointsCount - 1)
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
				if (Tools.current == Tool.Rotate && index % 3 == 0)
				{
					RotateLocal(index, point);
				}
				else
				{
					point = Handles.DoPositionHandle(point, handleRotation);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(spline, "Move Line Point");
						EditorUtility.SetDirty(spline);
						spline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
					}
				}
			}

			return point;
		}

		private void RotateLocal(int index, Vector3 point)
		{
			var rotation = Handles.DoRotationHandle(handleRotation, point);
			if (EditorGUI.EndChangeCheck())
			{
				if (!isRotating)
				{
					lastRotation = rotation;
					isRotating = true;
				}

				var rotationDiff = rotation * Quaternion.Inverse(lastRotation);

				Undo.RecordObject(spline, "Rotate Line Point");
				EditorUtility.SetDirty(spline);
				var point1Index = index == spline.PointsCount - 1 && spline.IsLoop ? 1 : index + 1;
				var point2Index = index == 0 && spline.IsLoop ? spline.PointsCount - 2 : index - 1;

				if (point1Index >= 0 && point1Index < spline.PointsCount)
				{
					var point1 = handleTransform.TransformPoint(spline.Points[point1Index].position);
					var rotatedPoint1 = RotateAround(point1, point, rotationDiff);
					spline.UpdatePoint(point1Index, handleTransform.InverseTransformPoint(rotatedPoint1));
				}

				if (point2Index >= 0 && point2Index < spline.PointsCount)
				{
					var point2 = handleTransform.TransformPoint(spline.Points[point2Index].position);
					var rotatedPoint2 = RotateAround(point2, point, rotationDiff);
					spline.UpdatePoint(point2Index, handleTransform.InverseTransformPoint(rotatedPoint2));
				}

				lastRotation = rotation;
			}
			else if (isRotating && currentEvent.type == EventType.MouseUp)
			{
				lastRotation = handleRotation;
				isRotating = false;
			}
		}

		private static Vector3 RotateAround(Vector3 target, Vector3 pivotPoint, Quaternion rot)
		{
			return rot * (target - pivotPoint) + pivotPoint;
		}

		private void SelectIndex(int index)
		{
			if (index == -1 && selectedIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(index);
		}

		private void UpdateSelectedIndex(int index)
		{
			selectedIndex = index;
			selectedCurveIndex = index != -1 ? index / 3 : -1;
			if (selectedCurveIndex == spline.CurveCount)
			{
				selectedCurveIndex = spline.IsLoop ? 0 : spline.CurveCount - 1;
			}
			editorState.isMoreThanOneCurve = IsMoreThanOneCurve;
			editorState.isAnyCurveSelected = IsAnyCurveSelected;

			if (index != -1)
			{
				ToggleDrawCurveMode(false);
			}
		}

	}

}