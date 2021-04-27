using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private const string BezierGroupTitle = "Bezier Curve";
        private const string AddCurveButtonTitle = "Add Curve";
        private const string AddCurveButtonTooltip = "Add curve at the beginning or the end of the spline.";
        private const string RemoveCurveButtonTitle = "Remove Curve";
        private const string RemoveCurveButtonTooltip = "Remove selected curve.";
        private const string SplitCurveButtonTitle = "Split Curve";
        private const string SplitCurveButtonTooltip = "Split curve by adding mid point.";

        private void DrawBezierCurveOptions()
		{
            var prevEnabled = GUI.enabled;
            var isVisible = BezierSplineEditor.currentEditor != null && BezierSplineEditor.currentEditor.SelectedCurveIndex != -1;
            GUILayout.Label(BezierGroupTitle);
            GUI.enabled = isVisible;
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var addCurveButtonContent = new GUIContent(AddCurveButtonTitle, AddCurveButtonTooltip);
            if (GUILayout.Button(addCurveButtonContent, ButtonWidth, ButtonHeight))
            {

            }

            var removeCurveButtonContent = new GUIContent(RemoveCurveButtonTitle, RemoveCurveButtonTooltip);
            if (GUILayout.Button(removeCurveButtonContent, ButtonWidth, ButtonHeight))
            {

            }


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var splitCurveButtonContent = new GUIContent(SplitCurveButtonTitle, SplitCurveButtonTooltip);
            if (GUILayout.Button(splitCurveButtonContent, ButtonWidth, ButtonHeight))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
