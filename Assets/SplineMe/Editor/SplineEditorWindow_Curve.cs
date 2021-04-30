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
                BezierSplineEditor.CurrentEditor.AddCurve();
                repaintScene = true;
            }

            //TODO: Add event
            GUI.enabled = isCurveEditorEnabled && BezierSplineEditor.CurrentEditor.CanSelectedCurveBeRemoved;
            var removeCurveButtonContent = new GUIContent(RemoveCurveButtonTitle, RemoveCurveButtonTooltip);
            if (GUILayout.Button(removeCurveButtonContent, ButtonWidth, ButtonHeight))
            {
                BezierSplineEditor.CurrentEditor.RemoveSelectedCurve();
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
                BezierSplineEditor.CurrentEditor.AddMidCurve();
                repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
