using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private const string BezierGroupTitle = "Curve";
        private const string AddCurveButtonTitle = "Add Curve";
        private const string AddCurveButtonTooltip = "Add curve at the beginning or the end of the spline.";
        private const string RemoveCurveButtonTitle = "Remove Curve";
        private const string RemoveCurveButtonTooltip = "Remove selected curve.";
        private const string SplitCurveButtonTitle = "Split Curve";
        private const string SplitCurveButtonTooltip = "Split curve by adding mid point.";
        private const string SplitPointSliderLabel = "Split Point";

		private float splitCurveValue = 0.5f;

        private void DrawBezierCurveOptions()
		{
            var prevEnabled = GUI.enabled;
            var isGroupEnabled = isCurveEditorEnabled;
            GUI.enabled = isGroupEnabled;
            GUILayout.Label(BezierGroupTitle);
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = isGroupEnabled && SplineEditor.CanNewCurveBeAdded;
            var addCurveButtonContent = new GUIContent(useText ? AddCurveButtonTitle : string.Empty, useImages ? editorSettings.addCurveIcon : null, useText ? AddCurveButtonTooltip : AddCurveButtonTitle);
            if (GUILayout.Button(addCurveButtonContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				SplineEditor.ScheduleAddCurve();
				repaintScene = true;
            }

            GUI.enabled = isGroupEnabled && SplineEditor.CanSelectedCurveBeRemoved;
            var removeCurveButtonContent = new GUIContent(useText ? RemoveCurveButtonTitle : string.Empty, useImages ? editorSettings.removeCurveIcon : null, useText ? RemoveCurveButtonTooltip : RemoveCurveButtonTitle);
            if (GUILayout.Button(removeCurveButtonContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				SplineEditor.ScheduleRemoveSelectedCurve();
				repaintScene = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var splitCurveButtonContent = new GUIContent(useText ? SplitCurveButtonTitle : string.Empty, useImages ? editorSettings.splitCurveIcon : null, useText ? SplitCurveButtonTooltip : SplitCurveButtonTitle);
            GUI.enabled = isGroupEnabled && SplineEditor.IsAnyPointSelected;
            if (GUILayout.Button(splitCurveButtonContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				SplineEditor.ScheduleSplitCurve(splitCurveValue);
				repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
            GUILayout.Label(SplitPointSliderLabel);
			splitCurveValue = EditorGUILayout.Slider(splitCurveValue, 0.001f, 0.999f, CustomSliderWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
