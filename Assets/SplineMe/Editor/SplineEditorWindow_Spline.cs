using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{

    public partial class SplineEditorWindow : EditorWindow
    {
        private const string SplineOptionsTitle = "Spline";
        private const string FactorSplineButtonTitle = "Factor Spline";
        private const string FactorSplineButtonTooltip = "Factor spline by adding mid points to every curve.";

        private const string SimplifySplineButtonTitle = "Simplify Spline";
        private const string SimplifySplineButtonTooltip = "Simplify spline by removing every second curve.";

        private const string CastOptionsTitle = "Cast";
        private const string CastSplineButtonTitle = "Cast Spline";
        private const string CastSplineButtonTooltip = "Cast spline regarding to custom Transform or self (transform == null).";

        private const string CastSplineToCameraButtonTitle = "Cast To Camera View";
        private const string CastSplineToCameraButtonTooltip = "Cast spline regarding to camera view.";

        private Transform customTransform = null;
        private GUILayoutOption CustomTransformWidth { get; } = GUILayout.Width(345);


        private void DrawSplineGroup()
        {
            var prevEnabled = GUI.enabled;
            var isVisible = BezierSplineEditor.currentEditor != null && BezierSplineEditor.currentSpline!=null;
            GUI.enabled = isVisible;
            var prevColor = GUI.color;

            GUILayout.BeginVertical();
            DrawSplineButtons();
            DrawCastButtons();
            GUILayout.EndVertical();

            GUI.color = prevColor;
            GUI.enabled = prevEnabled;
        }

        private void DrawSplineButtons()
		{
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.Label(SplineOptionsTitle);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var factorSplineButtonContent = new GUIContent(FactorSplineButtonTitle, FactorSplineButtonTooltip);
            if (GUILayout.Button(factorSplineButtonContent, ButtonWidth, ButtonHeight))
            {

            }

            var simplifyButtonContent = new GUIContent(SimplifySplineButtonTitle, SimplifySplineButtonTooltip);
            if (GUILayout.Button(simplifyButtonContent, ButtonWidth, ButtonHeight))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void DrawCastButtons()
		{
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var castSplineContent = new GUIContent(CastSplineButtonTitle, CastSplineButtonTooltip);
            if (GUILayout.Button(castSplineContent, ButtonWidth, ButtonHeight))
            {

            }

            var castSplineToCameraContent = new GUIContent(CastSplineToCameraButtonTitle, CastSplineToCameraButtonTooltip);
            if (GUILayout.Button(castSplineToCameraContent, ButtonWidth, ButtonHeight))
            {

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            customTransform = EditorGUILayout.ObjectField("Custom transform", customTransform, typeof(Transform), true, CustomTransformWidth) as Transform;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }


    }
}
