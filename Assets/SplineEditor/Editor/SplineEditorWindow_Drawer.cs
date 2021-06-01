using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private bool isDrawerMode = false;
        private bool isDrawerSectionFolded = true;

        private void DrawDrawerToolOptions()
		{
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;

            isDrawerSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isDrawerSectionFolded, DrawerGroupTitle);
            GUI.enabled = isSplineEditorEnabled;

            if (isDrawerSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);

				DrawDrawerToolButton();
				DrawSmoothAnglesToggle();
				DrawSegmentLengthField();
				DrawFirstPointHookSlider();
				DrawSecondPointHookSlider();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
            GUI.enabled = prevEnabled;
        }

		private void DrawDrawerToolButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUI.enabled = isSplineEditorEnabled && !SplineEditor.CurrentSpline.IsLoop;
			var toggleState = isSplineEditorEnabled && isDrawerMode;
			if (GUILayout.Toggle(toggleState, DrawCurveButtonContent, toggleButtonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				SplineEditor.ToggleDrawSplineMode();
				repaintScene = true;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSmoothAnglesToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			SplineEditor.DrawCurveSmoothAcuteAngles = EditorGUILayout.Toggle(DrawCurveSmoothAnglesContent, SplineEditor.DrawCurveSmoothAcuteAngles);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSecondPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(DrawCurveSecondHookContent);
			SplineEditor.DrawCurveSecondPointHook = EditorGUILayout.Slider(SplineEditor.DrawCurveSecondPointHook, SplineEditor.DrawCurveFirstPointHook, 0.999f, ToolsSliderWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawFirstPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(DrawCurveFirstHookContent);
			SplineEditor.DrawCurveFirstPointHook = EditorGUILayout.Slider(SplineEditor.DrawCurveFirstPointHook, 0.001f, SplineEditor.DrawCurveSecondPointHook, ToolsSliderWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSegmentLengthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			SplineEditor.DrawCurveSegmentLength = EditorGUILayout.FloatField(DrawCurveSegmentLengthContent, SplineEditor.DrawCurveSegmentLength);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

}
