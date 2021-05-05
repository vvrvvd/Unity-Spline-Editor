using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private float splitCurveValue = 0.5f;

        private void DrawBezierCurveOptions()
		{
            var prevEnabled = GUI.enabled;
            var isGroupEnabled = isCurveEditorEnabled;
            GUI.enabled = isGroupEnabled;
            GUILayout.Label(BezierGroupTitle);
            GUILayout.BeginVertical(groupsStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = isGroupEnabled && SplineEditor.CanNewCurveBeAdded;
            if (GUILayout.Button(AddCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
				SplineEditor.ScheduleAddCurve();
				repaintScene = true;
            }

            GUI.enabled = isGroupEnabled && SplineEditor.CanSelectedCurveBeRemoved;
            if (GUILayout.Button(RemoveCurveButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
            {
				SplineEditor.ScheduleRemoveSelectedCurve();
				repaintScene = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = isGroupEnabled && SplineEditor.IsAnyPointSelected;
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

			GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
