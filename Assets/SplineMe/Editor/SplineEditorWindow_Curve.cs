using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private float splitCurveValue = 0.5f;

        private bool isCurveSectionFolded = true;

        private void DrawCurveOptions()
		{
            var prevEnabled = GUI.enabled;
            var isGroupEnabled = isCurveEditorEnabled;

            isCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCurveSectionFolded, BezierGroupTitle);
            GUI.enabled = isGroupEnabled;
            if (isCurveSectionFolded)
            {
                GUILayout.BeginVertical(groupsStyle);
                GUILayout.Space(10);

                DrawAddAndRemoveCurveSection();
                DrawSplitCurveSection();

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

            GUI.enabled &= SplineEditor.CanNewCurveBeAdded;
            if (GUILayout.Button(AddCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleAddCurve();
                repaintScene = true;
            }
        }

        private void DrawRemoveCurveButton()
		{
            GUI.enabled &= SplineEditor.CanSelectedCurveBeRemoved;
            if (GUILayout.Button(RemoveCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleRemoveSelectedCurve();
                repaintScene = true;
            }
        }

        private void DrawSplitCurveSection()
		{
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled &= SplineEditor.IsAnyPointSelected;
            if (GUILayout.Button(SplitCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
                SplineEditor.ScheduleSplitCurve(splitCurveValue);
                repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(SplitPointSliderContent);
            splitCurveValue = EditorGUILayout.Slider(splitCurveValue, 0.001f, 0.999f, ToolsSliderWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

    }

}
