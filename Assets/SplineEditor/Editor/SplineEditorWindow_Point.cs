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

			editorWindowState.isPointSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.isPointSectionFolded, PointGroupTitle);
			GUI.enabled = isCurveEditorEnabled;
			if(editorWindowState.isPointSectionFolded)
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
			DrawSelectedPointInspector();
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

		private void DrawPointsScaleField()
		{
			var prevEnabled = GUI.enabled;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(PointScaleContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var isScaleFieldActive = editorState.IsAnyPointSelected && editorState.SelectedPointIndex % 3 == 0;

			GUI.enabled = isScaleFieldActive;

			var pointIndex = editorState.SelectedPointIndex / 3;
			var currentPointScale = isScaleFieldActive ? editorState.CurrentSpline.PointsScales[pointIndex] : editorWindowState.previousPointScale;

			var nextPointScale = EditorGUILayout.Vector3Field(string.Empty, currentPointScale, ToolsPointPositionWidth);
			if (nextPointScale != currentPointScale)
			{
				editorState.CurrentSpline.UpdatePointsScale(pointIndex, nextPointScale);
			}
			editorWindowState.previousPointScale = currentPointScale;

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}


		private void DrawPositionField()
		{
			var currentSpline = editorState.CurrentSpline;
			var selectedPointIndex = editorState.SelectedPointIndex;
			var isPointSelected = currentSpline != null && editorState.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(PointPositionContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			var pointPosition = isPointSelected ? currentSpline.Points[selectedPointIndex].position : editorWindowState.previousPointPosition;
			var point = EditorGUILayout.Vector3Field(string.Empty, pointPosition, ToolsPointPositionWidth);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Move Point");
				currentSpline.UpdatePoint(selectedPointIndex, point);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			editorWindowState.previousPointPosition = point;
		}

		private void DrawModePopupField()
		{
			var currentSpline = editorState.CurrentSpline;
			var selectedPointIndex = editorState.SelectedPointIndex;
			var isPointSelected = currentSpline != null && editorState.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			GUILayout.Label(PointModeContent, ToolsPointPopupLabelWidth);
			var currentMode = isPointSelected ? currentSpline.GetControlPointMode(selectedPointIndex) : editorWindowState.previousPointMode;
			var mode = (BezierControlPointMode)EditorGUILayout.EnumPopup(currentMode, ToolsPointPopupWidth);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Change Point Mode");
				currentSpline.SetControlPointMode(selectedPointIndex, mode);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(ApplyToAllPoinstButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				Undo.RecordObject(currentSpline, "Change All Points Mode");
				currentSpline.SetAllControlPointsMode(mode);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			editorWindowState.previousPointMode = mode;
		}



	}
}
