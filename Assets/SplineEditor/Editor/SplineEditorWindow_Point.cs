using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private void DrawPointGroup()
		{
			var prevEnabled = GUI.enabled;
			var prevColor = GUI.color;

			editorWindowState.IsPointSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.IsPointSectionFolded, PointGroupTitle);
			GUI.enabled = editorState.IsAnyPointSelected;
			if(editorWindowState.IsPointSectionFolded)
			{
				DrawSelectedSplineInspector();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			GUI.color = prevColor;
			GUI.enabled = prevEnabled;
		}

		private void DrawSelectedSplineInspector()
		{
			var isGroupEnabled = GUI.enabled;
			GUILayout.BeginVertical(groupsStyle);
			GUILayout.Space(10);
			EditorGUI.indentLevel++;

			DrawSelectedPointInspector();

			EditorGUI.indentLevel--;
			GUILayout.Space(10);
			GUILayout.EndVertical();
			GUI.enabled = isGroupEnabled;
		}

		private void DrawSelectedPointInspector()
		{
			var prevEnabled = GUI.enabled;

			var currentSpline = editorState.CurrentSpline;
			var isPointSelected = currentSpline != null && editorState.IsAnyPointSelected;
			GUI.enabled &= isPointSelected;

			DrawPositionField();
			DrawPointsScaleField();
			GUILayout.Space(10);
			DrawModePopupField();

			GUI.enabled = prevEnabled;
		}


		private void DrawPositionField()
		{
			var currentSpline = editorState.CurrentSpline;
			var selectedPointIndex = editorState.SelectedPointIndex;
			var isPointSelected = currentSpline != null && editorState.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			var pointPosition = isPointSelected ? currentSpline.Points[selectedPointIndex].position : editorWindowState.PreviousPointPosition;
			var point = EditorGUILayout.Vector3Field(PointPositionContent, pointPosition);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Move Point");
				currentSpline.UpdatePoint(selectedPointIndex, point);
				repaintScene = true;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			editorWindowState.PreviousPointPosition = point;
		}

		private void DrawPointsScaleField()
		{
			var prevEnabled = GUI.enabled;
			var currentSpline = editorState.CurrentSpline;
			var isScaleFieldActive = GUI.enabled & editorState.SelectedPointIndex % 3 == 0;
			GUI.enabled = isScaleFieldActive;
			
			GUILayout.BeginHorizontal();

			var pointIndex = editorState.SelectedPointIndex / 3;
			var currentPointScale = isScaleFieldActive ? currentSpline.PointsScales[pointIndex] : editorWindowState.PreviousPointScale;
			EditorGUI.BeginChangeCheck();
			var nextPointScale = EditorGUILayout.Vector3Field(PointScaleContent, currentPointScale);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Scale Point");
				currentSpline.UpdatePointsScale(pointIndex, nextPointScale);
			}
			editorWindowState.PreviousPointScale = currentPointScale;

			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawModePopupField()
		{
			var currentSpline = editorState.CurrentSpline;
			var selectedPointIndex = editorState.SelectedPointIndex;
			var isPointSelected = currentSpline != null && editorState.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			var currentMode = isPointSelected ? currentSpline.GetControlPointMode(selectedPointIndex) : editorWindowState.PreviousPointMode;
			var mode = (BezierControlPointMode)EditorGUILayout.EnumPopup(PointModeContent, currentMode);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Change Point Mode");
				currentSpline.SetControlPointMode(selectedPointIndex, mode);
				repaintScene = true;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			if (GUILayout.Button(ApplyToAllPoinstButtonContent, buttonStyle, ToolsButtonsHeight))
			{
				Undo.RecordObject(currentSpline, "Change All Points Mode");
				currentSpline.SetAllControlPointsMode(mode);
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			editorWindowState.PreviousPointMode = mode;
		}



	}
}
