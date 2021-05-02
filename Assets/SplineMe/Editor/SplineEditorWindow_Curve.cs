using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
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
            GUILayout.Label(BezierGroupTitle);
            GUI.enabled = isCurveEditorEnabled;
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            //TODO: Add event
            GUI.enabled = isCurveEditorEnabled && BezierSplineEditor.CurrentEditor.CanNewCurveBeAdded;
            var addCurveButtonContent = new GUIContent(AddCurveButtonTitle, AddCurveButtonTooltip);
            if (GUILayout.Button(addCurveButtonContent, ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.AddCurveShortcut();
				repaintScene = true;
            }

            //TODO: Add event
            GUI.enabled = isCurveEditorEnabled && BezierSplineEditor.CurrentEditor.CanSelectedCurveBeRemoved;
            var removeCurveButtonContent = new GUIContent(RemoveCurveButtonTitle, RemoveCurveButtonTooltip);
            if (GUILayout.Button(removeCurveButtonContent, ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.RemoveSelectedCurveShortcut();
				repaintScene = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var splitCurveButtonContent = new GUIContent(SplitCurveButtonTitle, SplitCurveButtonTooltip);
            GUI.enabled = isCurveEditorEnabled;
            if (GUILayout.Button(splitCurveButtonContent, ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.SplitCurveByPoint(splitCurveValue);
				repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			splitCurveValue = EditorGUILayout.Slider(SplitPointSliderLabel, splitCurveValue, 0.001f, 0.999f, CustomTransformFieldWidth);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
