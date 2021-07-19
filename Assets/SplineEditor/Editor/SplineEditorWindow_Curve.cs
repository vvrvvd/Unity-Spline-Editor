using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private void DrawCurveOptions()
		{
            var prevEnabled = GUI.enabled;
            var isGroupEnabled = IsCurveEditorEnabled;

            editorWindowState.IsCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.IsCurveSectionFolded, BezierGroupTitle);
            GUI.enabled = isGroupEnabled;
            if (editorWindowState.IsCurveSectionFolded)
            {
                GUILayout.BeginVertical(groupsStyle);
                GUILayout.Space(10);
                EditorGUI.indentLevel++;

                DrawCurveParametersSection();
                GUILayout.Space(10);
                DrawCurveButtonsSection();

                EditorGUI.indentLevel--;
                GUILayout.Space(10);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUI.enabled = prevEnabled;
        }

        private void DrawCurveButtonsSection()
		{
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            DrawAddCurveButton();
            DrawRemoveCurveButton();
            DrawSplitCurveButton();
            GUILayout.Space(15);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawAddCurveButton()
		{
            var prevEnabled = GUI.enabled;
            GUI.enabled &= editorState.CanNewCurveBeAdded;
            if (GUILayout.Button(AddCurveButtonContent, buttonStyle, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleAddCurve(editorWindowState.AddCurveLength);
                repaintScene = true;
            }

            GUI.enabled = prevEnabled;
        }

        private void DrawRemoveCurveButton()
		{
            var prevEnabled = GUI.enabled;
            GUI.enabled &= editorState.CanSelectedCurveBeRemoved;
            if (GUILayout.Button(RemoveCurveButtonContent, buttonStyle, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleRemoveSelectedCurve();
                repaintScene = true;
            }
            GUI.enabled = prevEnabled;
        }

        private void DrawSplitCurveButton()
		{
            var prevEnabled = GUI.enabled;
            GUI.enabled &= editorState.IsAnyPointSelected;
            if (GUILayout.Button(SplitCurveButtonContent, buttonStyle, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleSplitCurve(editorWindowState.SplitCurveValue);
                repaintScene = true;
            }
            GUI.enabled = prevEnabled;
        }

        private void DrawCurveParametersSection()
		{
			DrawAddCurveLengthField();
			DrawSplitCurveSlider();
		}

		private void DrawAddCurveLengthField()
        {
            GUILayout.BeginHorizontal();
            var prevState = editorWindowState.AddCurveLength;
            var nextState = EditorGUILayout.FloatField(AddCurveLengthFieldContent, editorWindowState.AddCurveLength);
            if (nextState != prevState)
            {
                Undo.RecordObject(editorWindowState, "Change Add Curve Length");
                editorWindowState.AddCurveLength = nextState;
            }
            GUILayout.Space(15);
            GUILayout.EndHorizontal();
        }

        private void DrawSplitCurveSlider()
        {
            GUILayout.BeginHorizontal();
            var prevState = editorWindowState.SplitCurveValue;
            var nextState = EditorGUILayout.Slider(SplitPointSliderContent, editorWindowState.SplitCurveValue, 0.001f, 0.999f);
            if (nextState != prevState)
            {
                Undo.RecordObject(editorWindowState, "Change Split Curve Point");
                editorWindowState.SplitCurveValue = nextState;
            }
            GUILayout.Space(15);
            GUILayout.EndHorizontal();
        }

    }

}
