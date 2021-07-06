using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{



        private void DrawDrawerToolOptions()
		{
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;

            editorWindowState.isDrawerSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.isDrawerSectionFolded, DrawerGroupTitle);
            GUI.enabled = isSplineEditorEnabled;

            if (editorWindowState.isDrawerSectionFolded)
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

			GUI.enabled = isSplineEditorEnabled && !editorState.CurrentSpline.IsLoop;
			var toggleState = isSplineEditorEnabled && editorWindowState.isDrawerMode;
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
			editorState.DrawCurveSmoothAcuteAngles = EditorGUILayout.Toggle(DrawCurveSmoothAnglesContent, editorState.DrawCurveSmoothAcuteAngles);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSecondPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(DrawCurveSecondHookContent);
			editorState.DrawCurveSecondPointHook = EditorGUILayout.Slider(editorState.DrawCurveSecondPointHook, editorState.DrawCurveFirstPointHook, 0.999f, ToolsSliderWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawFirstPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(DrawCurveFirstHookContent);
			editorState.DrawCurveFirstPointHook = EditorGUILayout.Slider(editorState.DrawCurveFirstPointHook, 0.001f, editorState.DrawCurveSecondPointHook, ToolsSliderWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSegmentLengthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			editorState.DrawCurveSegmentLength = EditorGUILayout.FloatField(DrawCurveSegmentLengthContent, editorState.DrawCurveSegmentLength);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

}
