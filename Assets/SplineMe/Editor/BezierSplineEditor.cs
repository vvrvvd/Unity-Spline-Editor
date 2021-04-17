using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace SplineMe.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public class BezierSplineEditor : UnityEditor.Editor
	{

		public static int selectedIndex = -1;
		public static int selectedCurveIndex = -1;

		//TODO: Change to static editor
		private static Quaternion handleRotation;
		private static BezierSpline spline;
		private static Event currentEvent;
		private static Transform handleTransform;
		private static BezierSplineEditor editor;

		private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

		private bool IsAnyPointSelected => selectedIndex != -1 && spline != null && selectedIndex < spline.PointsCount;
		private bool IsAnyCurveSelected => selectedCurveIndex != -1 && spline != null && selectedCurveIndex < spline.CurveCount;
		private bool IsMoreThanOneCurve => spline.CurveCount > 1;

		private SplineEditorState editorState;

		private bool isRotating;
		private Quaternion lastRotation;

		private static bool isCurveDrawerMode;
		private Vector3 curveDrawerPosition = Vector3.zero;
		private Vector3[] newCurvePoints = new Vector3[4];
		private bool isDraggingNewCurve = false;
		private bool firstControlPointSet = false;
		private bool secondControlPointSet = false;

		private static bool isDraggingPoint = false;

		private static bool factorCurveFlag = false;
		private static bool simplifyCurveFlag = false;
		private static bool addCurveFlag = false;
		private static bool removeSelectedCurveFlag = false;
		private static bool castSelectedPointFlag = false;
		private static bool castSelectedPointShortcutFlag = false;
		private static bool drawCurveModeFlag = false;
		private static bool castCurveFlag = false;

		private void OnEnable()
		{
			editor = this;
			spline = target as BezierSpline;
			SplineMeTools.InitializeGUI(ref editorState);

			editorState.title = "Spline Tools";
			editorState.AddCurveAction = AddCurve;
			editorState.RemoveCurveAction = RemoveSelectedCurve;
			editorState.isAnyCurveSelected = IsAnyPointSelected;

			isDraggingPoint = false;

			//Reset flags
			factorCurveFlag = false;
			simplifyCurveFlag = false;
			addCurveFlag = false;
			removeSelectedCurveFlag = false;
			drawCurveModeFlag = false;
			castCurveFlag = false;
			castSelectedPointFlag = false;
			castSelectedPointShortcutFlag = false;

			if (isCurveDrawerMode)
			{
				ToggleDrawCurveMode(true);
			}
		}

		private void OnDisable()
		{
			spline = null;
			editor = null;
			SplineMeTools.ReleaseGUI(ref editorState);
		}

		public override void OnInspectorGUI()
		{
			editor = this;
			spline = target as BezierSpline;
			EditorGUI.BeginChangeCheck();
			var prevEnabled = GUI.enabled;
			GUI.enabled = IsMoreThanOneCurve;
			bool loop = EditorGUILayout.Toggle("Loop", spline.IsLoop);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Toggle Loop");
				spline.IsLoop = loop;

				if (spline.IsLoop)
				{
					ToggleDrawCurveMode(false);
					EditorUtility.SetDirty(spline);
				}
			}

			GUI.enabled = prevEnabled;

			if (selectedIndex >= spline.PointsCount)
			{
				SelectIndex(spline.PointsCount - 1);
			}

			if (selectedIndex != -1)
			{
				DrawSelectedPointInspector();
			}

			if (GUILayout.Button("Cast Curve Points"))
			{
				CastCurve();
				EditorUtility.SetDirty(spline);
			}

			GUI.enabled = IsAnyCurveSelected;
			if (GUILayout.Button("Add Mid Curve"))
			{
				AddMidCurve();
				EditorUtility.SetDirty(spline);
			}
			GUI.enabled = prevEnabled;

			if (GUILayout.Button("Factor Curve"))
			{
				FactorCurve();
				EditorUtility.SetDirty(spline);
			}

			GUI.enabled = IsMoreThanOneCurve && (!spline.IsLoop || spline.CurveCount > 2);
			if (GUILayout.Button("Simplify Curve"))
			{
				SimplifyCurve();
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
				spline.UpdatePoint(selectedIndex, point);
				EditorUtility.SetDirty(spline);
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
			if (spline == curve)
			{
				return;
			}

			var prevSpline = spline;
			var prevHandleTransform = handleTransform;
			var prevHandleRotation = handleRotation;

			spline = curve;
			handleTransform = spline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
			DrawSpline();

			spline = prevSpline;
			handleTransform = prevHandleTransform;
			handleRotation = prevHandleRotation;
		}

		private void OnSceneGUI()
		{

			var prevSpline = spline;
			currentEvent = Event.current;
			spline = target as BezierSpline;

			if (spline == null)
			{
				return;
			}

			if (spline != prevSpline)
			{
				ToggleDrawCurveModeShortcut();
			}

			handleTransform = spline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (drawCurveModeFlag)
			{
				ToggleDrawCurveMode(!isCurveDrawerMode);
				drawCurveModeFlag = false;
			}

			if (factorCurveFlag)
			{
				FactorCurve();
				factorCurveFlag = false;
			}

			if (simplifyCurveFlag)
			{
				SimplifyCurve();
				simplifyCurveFlag = false;
			}

			if (castCurveFlag)
			{
				CastCurve();
				castCurveFlag = false;
			}

			if (!isCurveDrawerMode)
			{
				if (addCurveFlag)
				{
					AddCurve();
					addCurveFlag = false;
				}

				if (removeSelectedCurveFlag)
				{
					RemoveSelectedCurve();
					removeSelectedCurveFlag = false;
				}
			}

			if (selectedIndex >= spline.PointsCount)
			{
				SelectIndex(spline.PointsCount - 1);
			}

			if (Event.current.type == EventType.Repaint)
			{
				DrawSpline();

				if (editorState.showDirectionsLines)
				{
					DrawDirections();
				}

				if (editorState.showSegmentsPoints)
				{
					DrawSegments();
				}

			}

			if (isCurveDrawerMode)
			{
				DrawCurveMode();
			}

			if (editorState.showPointsHandles)
			{
				DrawPoints();
			}
			else
			{
				SelectIndex(-1);
			}

			SplineMeTools.DrawGUI(ref editorState);
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
				var p2 = DrawPoint(curveStartIndex + 2);
				var p3 = handleTransform.TransformPoint(spline.Points[curveStartIndex + 3].position);

				if (!isCurveDrawerMode || i < spline.CurveCount - 1)
				{
					p3 = DrawPoint(curveStartIndex + 3);
				}

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

		[ShortcutAttribute("Spline Editor/Cast Curve Points", KeyCode.U, ShortcutModifiers.Action)]
		private static void CastCurvePointsShortcut()
		{
			castCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Add Mid Curve", KeyCode.M, ShortcutModifiers.Action)]
		private static void AddMidCurve()
		{
			if (spline == null || editor == null)
			{
				return;
			}

			Undo.RecordObject(spline, "Add Mid Curve");
			var wasLastPoint = selectedIndex == spline.PointsCount - 1;
			spline.AddMidCurveAndApplyConstraints(selectedCurveIndex);
			if (wasLastPoint && !spline.IsLoop)
			{
				editor.SelectIndex(spline.PointsCount - 4);
			}
			else if (selectedIndex != 0 && !(wasLastPoint && spline.IsLoop))
			{
				editor.SelectIndex(selectedIndex + 3);
			}
			else
			{
				editor.SelectIndex(3);
			}
		}

		[ShortcutAttribute("Spline Editor/Factor Curve", KeyCode.G, ShortcutModifiers.Action)]
		private static void FactorCurvesShortcut()
		{
			factorCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Simplify Curve", KeyCode.H, ShortcutModifiers.Action)]
		private static void SimplifyCurveShortcut()
		{
			simplifyCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Add Curve", KeyCode.Home, ShortcutModifiers.Action)]
		private static void AddCurveShortcut()
		{
			addCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Remove Curve", KeyCode.End, ShortcutModifiers.Action)]
		private static void RemoveSelectedCurveShortcut()
		{
			removeSelectedCurveFlag = true;
		}

		private static void CastCurve()
		{
			Undo.RecordObject(spline, "Cast Curve Points");
			spline.CastCurve();
		}

		private static void FactorCurve()
		{
			if (spline == null || editor == null)
			{
				return;
			}

			Undo.RecordObject(spline, "Factor Curve");
			spline.FactorCurve();
			if (selectedIndex != -1)
			{
				editor.SelectIndex(selectedIndex * 2);
			}
		}

		private static void SimplifyCurve()
		{
			if (spline == null || editor == null)
			{
				return;
			}

			Undo.RecordObject(spline, "Simplify Curve");
			spline.SimplifyCurve();
			if (selectedIndex != -1)
			{
				editor.SelectIndex(selectedIndex / 2);
			}
		}

		private static void AddCurve()
		{
			if (spline == null)
			{
				return;
			}

			Undo.RecordObject(spline, "Add Curve");
			spline.AddCurve(SplineMeTools.CreateCurveSegmentSize);
			editor.UpdateSelectedIndex(spline.PointsCount - 1);
		}

		private static void RemoveSelectedCurve()
		{
			if (spline == null)
			{
				return;
			}

			if (!editor.IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(spline, "Remove Curve");
			var curveToRemove = spline.PointsCount - selectedIndex < 3 ? selectedCurveIndex + 1 : selectedCurveIndex;
			spline.RemoveCurve(curveToRemove);
			var nextSelectedIndex = Mathf.Min(selectedIndex, spline.PointsCount - 1);
			editor.UpdateSelectedIndex(nextSelectedIndex);
		}

		[ShortcutAttribute("Spline Editor/Toggle Draw Curve Mode", KeyCode.Slash, ShortcutModifiers.Action)]
		private static void ToggleDrawCurveModeShortcut()
		{
			drawCurveModeFlag = !drawCurveModeFlag;
		}

		[ClutchShortcut("Spline Editor/Cast Selected Point To Mouse Position", KeyCode.U, ShortcutModifiers.None)]
		private static void TryCastSelectedPointShortcut()
		{
			castSelectedPointFlag = !castSelectedPointShortcutFlag;
			castSelectedPointShortcutFlag = castSelectedPointFlag;
		}

		private bool TryCastMousePoint(out Vector3 castedPoint)
		{
			var mousePosition = Event.current.mousePosition;
			var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
			var isCorrectPosition = Physics.Raycast(ray, out var hit, SplineMeTools.MaxRaycastDistance, Physics.AllLayers);

			if (isCorrectPosition)
			{
				castedPoint = hit.point;
			}
			else
			{
				castedPoint = Vector3.zero;
			}

			return isCorrectPosition;
		}

		private void ToggleDrawCurveMode(bool state)
		{
			if (isCurveDrawerMode != state)
			{
				Undo.RecordObject(spline, "Toggle Draw Curve Mode");
			}

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
			var pointColor = index % 3 == 0 ? SplineMeTools.CurvePointColor : SplineMeTools.ModeColors[(int)mode];

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
				if (SplineMeTools.savedTool == Tool.Rotate && index % 3 == 0)
				{
					EditorGUI.BeginChangeCheck();
					RotateLocal(index, point);
				}
				else
				{
					if (castSelectedPointFlag)
					{
						var castedPosition = Vector3.zero;
						if (TryCastMousePoint(out castedPosition))
						{
							Undo.RecordObject(spline, "Cast Line Point To Mouse");
							point = castedPosition;
							spline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
						}
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						point = Handles.DoPositionHandle(point, handleRotation);
						var wasChanged = EditorGUI.EndChangeCheck();
						if(wasChanged)
						{
							Undo.RecordObject(spline, "Move Line Point");
							isDraggingPoint = true;
							spline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
						}
						else if ((isDraggingPoint && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
						{
							spline.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
							isDraggingPoint = false;
							castSelectedPointFlag = false;
						}
					}				
				}
			}

			return point;
		}

		private void DrawCurveMode()
		{
			var curveDrawerPointLocal = curveDrawerPosition;
			var curveDrawerPointWorld = spline.transform.TransformPoint(curveDrawerPointLocal);
			var size = HandleUtility.GetHandleSize(curveDrawerPointWorld);

			if (isDraggingNewCurve)
			{
				VisualizeCurveModeCurve();
			}

			Handles.color = Color.green;
			Handles.Button(curveDrawerPointWorld, handleRotation, size * SplineMeTools.DrawCurveSphereSize, size * SplineMeTools.DrawCurveSphereSize, Handles.SphereHandleCap);

			if (castSelectedPointFlag)
			{
				isDraggingNewCurve = true;
				var castedPosition = Vector3.zero;
				if (TryCastMousePoint(out castedPosition))
				{
					Undo.RecordObject(spline, "Cast Drawer Point");
					curveDrawerPosition = spline.transform.InverseTransformPoint(castedPosition);
					UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				var newEndPositionGlobal = Handles.DoPositionHandle(curveDrawerPointWorld, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					isDraggingNewCurve = true;
					Undo.RecordObject(spline, "Move Drawer Point");
					curveDrawerPosition = spline.transform.InverseTransformPoint(newEndPositionGlobal);
					UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
				}
				else if ((isDraggingNewCurve && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					if (firstControlPointSet && secondControlPointSet)
					{
						SpawnDrawCurveModeSpline();
					}

					var defaultDrawerPosition = spline.Points[spline.PointsCount - 1].position;
					InitializeDrawCurveMode(defaultDrawerPosition);
					isDraggingNewCurve = false;
					castSelectedPointFlag = false;
				}
			}
		}

		private void VisualizeCurveModeCurve()
		{
			var p0 = newCurvePoints[0];
			var p3 = newCurvePoints[3];

			if (Vector3.Distance(p0, p3) < 0.1f)
			{
				return;
			}

			spline.GetInverseControlPoints(p0, p3, newCurvePoints[1], newCurvePoints[2], SplineMeTools.DrawCurveFirstControlPoint, SplineMeTools.DrawCurveSecondControlPoint, out var p1, out var p2);

			p0 = handleTransform.TransformPoint(p0);
			p1 = handleTransform.TransformPoint(p1);
			p2 = handleTransform.TransformPoint(p2);
			p3 = handleTransform.TransformPoint(p3);

			Handles.DrawBezier(p0, p3, p1, p2, SplineMeTools.LineColor, null, SplineMeTools.LineWidth * 1.5f);

			if (editorState.showPointsHandles)
			{
				if (firstControlPointSet)
				{
					var f = handleTransform.TransformPoint(newCurvePoints[1]);
					var size = HandleUtility.GetHandleSize(f);
					Handles.color = SplineMeTools.DrawCurvePointColor;
					Handles.CubeHandleCap(0, f, Quaternion.identity, size * 0.1f, EventType.Repaint);
				}

				if (secondControlPointSet)
				{
					var g = handleTransform.TransformPoint(newCurvePoints[2]);
					var size = HandleUtility.GetHandleSize(g);
					Handles.color = SplineMeTools.DrawCurvePointColor;
					Handles.CubeHandleCap(0, g, Quaternion.identity, size * 0.1f, EventType.Repaint);
				}

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

			var dir = (newEndPosition - newCurvePoints[0]).normalized;
			var firstPointDistance = SplineMeTools.DrawCurveFirstControlPoint * SplineMeTools.DrawCurveSegmentLength;
			var secondPointDistance = SplineMeTools.DrawCurveSecondControlPoint * SplineMeTools.DrawCurveSegmentLength;

			if (!firstControlPointSet)
			{
				if (distance < firstPointDistance)
				{
					newCurvePoints[1] = newEndPosition;
				}
				else if (distance >= secondPointDistance)
				{
					newCurvePoints[1] = newCurvePoints[0] + dir * firstPointDistance;
					firstControlPointSet = true;
				}
			}

			if (!secondControlPointSet)
			{
				if (distance < secondPointDistance)
				{
					newCurvePoints[2] = newEndPosition;
				}
				else if (distance >= SplineMeTools.DrawCurveSegmentLength)
				{
					newCurvePoints[2] = newCurvePoints[0] + dir * secondPointDistance;
					secondControlPointSet = true;
				}
			}

			if (distance < secondPointDistance && distance >= firstPointDistance && !firstControlPointSet)
			{
				var prevFirstPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) / SplineMeTools.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / SplineMeTools.DrawCurveSegmentLength;
				var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevFirstPointNormalizedDistance);
				newCurvePoints[1] = Vector3.Lerp(newCurvePoints[1], newEndPosition, Mathf.Abs(SplineMeTools.DrawCurveFirstControlPoint - prevFirstPointNormalizedDistance) / normalizedDistancesDiff);
				firstControlPointSet = true;
			}

			if (distance < SplineMeTools.DrawCurveSegmentLength)
			{
				newCurvePoints[3] = newEndPosition;

				if (distance >= secondPointDistance && !secondControlPointSet)
				{
					var prevSecondPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[2]) / SplineMeTools.DrawCurveSegmentLength;
					var normalizedCurrentDistance = distance / SplineMeTools.DrawCurveSegmentLength;
					var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevSecondPointNormalizedDistance);
					newCurvePoints[2] = Vector3.Lerp(newCurvePoints[2], newEndPosition, Mathf.Abs(SplineMeTools.DrawCurveSecondControlPoint - prevSecondPointNormalizedDistance) / normalizedDistancesDiff);
					secondControlPointSet = true;
				}
			}
			else
			{
				var prevLastPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[3]) / SplineMeTools.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / SplineMeTools.DrawCurveSegmentLength;
				var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevLastPointNormalizedDistance);
				newCurvePoints[3] = Vector3.Lerp(newCurvePoints[3], newEndPosition, Mathf.Abs(1f - prevLastPointNormalizedDistance) / normalizedDistancesDiff);
				SpawnDrawCurveModeSpline();
				InitializeDrawCurveMode(newCurvePoints[3]);
				UpdateNewDrawCurvePainterPosition(newEndPosition);
			}

		}

		private void SpawnDrawCurveModeSpline()
		{
			var p0 = newCurvePoints[0];
			var p3 = newCurvePoints[3];
			spline.GetInverseControlPoints(p0, p3, newCurvePoints[1], newCurvePoints[2], SplineMeTools.DrawCurveFirstControlPoint, SplineMeTools.DrawCurveSecondControlPoint, out var p1, out var p2);
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

		public void SelectIndex(int index)
		{
			if (index == -1 && selectedIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(index);
			Repaint();
		}

		private void UpdateSelectedIndex(int index)
		{
			selectedIndex = index;
			selectedCurveIndex = index != -1 ? index / 3 : -1;
			if (selectedCurveIndex == spline.CurveCount)
			{
				selectedCurveIndex = spline.IsLoop ? 0 : spline.CurveCount - 1;
			}

			if (index != -1)
			{
				ToggleDrawCurveMode(false);
			}

			editorState.isMoreThanOneCurve = IsMoreThanOneCurve && !isCurveDrawerMode;
			editorState.isAnyCurveSelected = IsAnyCurveSelected;

			Repaint();
		}

	}

}