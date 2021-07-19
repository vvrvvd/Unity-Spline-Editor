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
            GUI.enabled = IsSplineEditorEnabled;

            if (editorWindowState.IsDrawerSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawSmoothAnglesToggle();
				DrawSegmentLengthField();
				DrawFirstPointHookSlider();
				DrawSecondPointHookSlider();
				DrawDrawerToolButton();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
            GUI.enabled = prevEnabled;
        }

		private void DrawDrawerToolButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			GUI.enabled = IsSplineEditorEnabled && !editorState.CurrentSpline.IsLoop;
			var toggleState = IsSplineEditorEnabled && editorState.IsDrawerMode;
			if (GUILayout.Toggle(toggleState, DrawCurveButtonContent, toggleButtonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ToggleDrawSplineMode();
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSmoothAnglesToggle()
		{
			GUILayout.BeginHorizontal();
			var prevState = editorState.DrawCurveSmoothAcuteAngles;
			var nextState = EditorGUILayout.Toggle(DrawCurveSmoothAnglesContent, prevState);
			if(nextState != prevState)
			{
				Undo.RecordObject(editorState, "Toggle Draw Smooth Angles");
				editorState.DrawCurveSmoothAcuteAngles = nextState;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSecondPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			var prevState = editorState.DrawCurveSecondPointHook;
			var nextState = EditorGUILayout.Slider(DrawCurveSecondHookContent, editorState.DrawCurveSecondPointHook, editorState.DrawCurveFirstPointHook, 0.999f);
			if (nextState != prevState)
			{
				Undo.RecordObject(editorState, "Change Second Point Hook");
				editorState.DrawCurveSecondPointHook = nextState;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawFirstPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			var prevState = editorState.DrawCurveFirstPointHook;
			var nextState = EditorGUILayout.Slider(DrawCurveFirstHookContent, editorState.DrawCurveFirstPointHook, 0.001f, editorState.DrawCurveSecondPointHook);
			if (nextState != prevState)
			{
				Undo.RecordObject(editorState, "Change First Point Hook");
				editorState.DrawCurveFirstPointHook = nextState;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSegmentLengthField()
		{
			GUILayout.BeginHorizontal();
			var prevState = editorState.DrawCurveSegmentLength;
			var nextState = EditorGUILayout.FloatField(DrawCurveSegmentLengthContent, editorState.DrawCurveSegmentLength);
			if (nextState != prevState)
			{
				Undo.RecordObject(editorState, "Change Draw Curve Segment Length");
				editorState.DrawCurveSegmentLength = nextState;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

	}

}
