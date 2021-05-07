using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private Vector3 previousPointPosition = Vector3.zero;
		private BezierControlPointMode previousPointMode = BezierControlPointMode.Free;

		private bool isPointSectionFolded = true;

		private void DrawPointGroup()
		{
			var prevEnabled = GUI.enabled;
			var prevColor = GUI.color;

			isPointSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isPointSectionFolded, PointGroupTitle);
			GUI.enabled = isCurveEditorEnabled;
			if(isPointSectionFolded)
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

			var currentSpline = SplineEditor.CurrentSpline;
			var isPointSelected = currentSpline != null && SplineEditor.IsAnyPointSelected;
			GUI.enabled &= isPointSelected;

			DrawPositionField();
			DrawModePopupField();

			GUI.enabled = prevEnabled;
		}

		private void DrawPositionField()
		{
			var currentSpline = SplineEditor.CurrentSpline;
			var selectedPointIndex = SplineEditor.SelectedPointIndex;
			var isPointSelected = currentSpline != null && SplineEditor.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(PointPositionContent, ToolsPointPopupLabelWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			var pointPosition = isPointSelected ? currentSpline.Points[selectedPointIndex].position : previousPointPosition;
			var point = EditorGUILayout.Vector3Field(string.Empty, pointPosition, ToolsPointPositionWidth);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Move Point");
				currentSpline.UpdatePoint(selectedPointIndex, point);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			previousPointPosition = point;
		}

		private void DrawModePopupField()
		{
			var currentSpline = SplineEditor.CurrentSpline;
			var selectedPointIndex = SplineEditor.SelectedPointIndex;
			var isPointSelected = currentSpline != null && SplineEditor.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			GUILayout.Label(PointModeContent, ToolsPointPopupLabelWidth);
			var currentMode = isPointSelected ? currentSpline.GetControlPointMode(selectedPointIndex) : previousPointMode;
			var mode = (BezierControlPointMode)EditorGUILayout.EnumPopup(currentMode, ToolsPointPopupWidth);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Change Point Mode");
				currentSpline.SetControlPointMode(selectedPointIndex, mode);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			previousPointMode = mode;
		}

	}
}
