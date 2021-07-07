using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private void DrawCurveOptions()
		{
            var prevEnabled = GUI.enabled;
            var isGroupEnabled = isCurveEditorEnabled;

            editorWindowState.isCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.isCurveSectionFolded, BezierGroupTitle);
            GUI.enabled = isGroupEnabled;
            if (editorWindowState.isCurveSectionFolded)
            {
                GUILayout.BeginVertical(groupsStyle);
                GUILayout.Space(10);

                DrawAddAndRemoveCurveSection();
                DrawSplitCurveSection();
                DrawCurveParametersSection();

                GUILayout.Space(10);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUI.enabled = prevEnabled;
        }

        private void DrawAddAndRemoveCurveSection()
		{
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            DrawAddCurveButton();
            DrawRemoveCurveButton();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawAddCurveButton()
		{
            var prevEnabled = GUI.enabled;
            GUI.enabled &= editorState.CanNewCurveBeAdded;
            if (GUILayout.Button(AddCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleAddCurve(editorWindowState.addCurveLength);
                repaintScene = true;
            }

            GUI.enabled = prevEnabled;
        }

        private void DrawRemoveCurveButton()
		{
            var prevEnabled = GUI.enabled;
            GUI.enabled &= editorState.CanSelectedCurveBeRemoved;
            if (GUILayout.Button(RemoveCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleRemoveSelectedCurve();
                repaintScene = true;
            }
            GUI.enabled = prevEnabled;
        }

        private void DrawSplitCurveSection()
		{
            var prevEnabled = GUI.enabled;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled &= editorState.IsAnyPointSelected;
            if (GUILayout.Button(SplitCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleSplitCurve(editorWindowState.splitCurveValue);
                repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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
            GUILayout.FlexibleSpace();
            var prevState = editorWindowState.addCurveLength;
            var nextState = EditorGUILayout.FloatField(AddCurveLengthFieldContent, editorWindowState.addCurveLength);
            if (nextState != prevState)
            {
                Undo.RecordObject(editorWindowState, "Change Add Curve Length");
                editorWindowState.addCurveLength = nextState;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawSplitCurveSlider()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(SplitPointSliderContent);
            var prevState = editorWindowState.splitCurveValue;
            var nextState = EditorGUILayout.Slider(editorWindowState.splitCurveValue, 0.001f, 0.999f, ToolsSliderWidth);
            if (nextState != prevState)
            {
                Undo.RecordObject(editorWindowState, "Change Split Curve Point");
                editorWindowState.splitCurveValue = nextState;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

    }

}
