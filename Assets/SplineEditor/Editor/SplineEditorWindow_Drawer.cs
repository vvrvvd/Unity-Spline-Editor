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

            editorWindowState.IsDrawerSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.IsDrawerSectionFolded, DrawerGroupTitle);
            GUI.enabled = isSplineEditorEnabled;

            if (editorWindowState.IsDrawerSectionFolded)
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
			var toggleState = isSplineEditorEnabled && editorState.IsDrawerMode;
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
			var prevState = editorState.DrawCurveSmoothAcuteAngles;
			var nextState = EditorGUILayout.Toggle(DrawCurveSmoothAnglesContent, prevState);
			if(nextState != prevState)
			{
				Undo.RecordObject(editorState, "Toggle Draw Smooth Angles");
				editorState.DrawCurveSmoothAcuteAngles = nextState;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSecondPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(DrawCurveSecondHookContent);
			var prevState = editorState.DrawCurveSecondPointHook;
			var nextState = EditorGUILayout.Slider(editorState.DrawCurveSecondPointHook, editorState.DrawCurveFirstPointHook, 0.999f, ToolsSliderWidth);
			if (nextState != prevState)
			{
				Undo.RecordObject(editorState, "Change Second Point Hook");
				editorState.DrawCurveSecondPointHook = nextState;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawFirstPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(DrawCurveFirstHookContent);
			var prevState = editorState.DrawCurveFirstPointHook;
			var nextState = EditorGUILayout.Slider(editorState.DrawCurveFirstPointHook, 0.001f, editorState.DrawCurveSecondPointHook, ToolsSliderWidth);
			if (nextState != prevState)
			{
				Undo.RecordObject(editorState, "Change First Point Hook");
				editorState.DrawCurveFirstPointHook = nextState;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSegmentLengthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var prevState = editorState.DrawCurveSegmentLength;
			var nextState = EditorGUILayout.FloatField(DrawCurveSegmentLengthContent, editorState.DrawCurveSegmentLength);
			if (nextState != prevState)
			{
				Undo.RecordObject(editorState, "Change Draw Curve Segment Length");
				editorState.DrawCurveSegmentLength = nextState;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

}
