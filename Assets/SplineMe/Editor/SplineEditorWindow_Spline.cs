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

        private const string CastSplineButtonTitle = "Cast Spline";
        private const string CastSplineButtonTooltip = "Cast spline regarding to cast transform or self (transform == null).";

        private const string CastSplineToCameraButtonTitle = "Cast Spline To Camera View";
        private const string CastSplineToCameraButtonTooltip = "Cast spline regarding to camera view.";
        private const string CastTransformFieldLabel = "Cast Transform";

        private Transform customTransform = null;
        private GUILayoutOption CustomTransformFieldWidth { get; } = GUILayout.Width(175);

        private void DrawSplineGroup()
        {
            var prevEnabled = GUI.enabled;
            GUI.enabled = isSplineEditorEnabled;
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
            var prevEnabled = GUI.enabled;
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.Label(SplineOptionsTitle);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var factorSplineButtonContent = new GUIContent(useText ? FactorSplineButtonTitle : string.Empty, useImages ? editorSettings.factorSplineIcon : null, useText ? FactorSplineButtonTooltip : FactorSplineButtonTitle);
            if (GUILayout.Button(factorSplineButtonContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.FactorSplineShortcut();
                repaintScene = true;
            }

            var simplifyButtonContent = new GUIContent(useText ? SimplifySplineButtonTitle : string.Empty, useImages ? editorSettings.simplifySplineIcon : null, useText ? SimplifySplineButtonTooltip : SimplifySplineButtonTitle);
            //TODO: Needs event to be added
            GUI.enabled = isSplineEditorEnabled && BezierSplineEditor.CurrentEditor.CanBeSimplified;
            if (GUILayout.Button(simplifyButtonContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.SimplifySplineShortcut();
				repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

        private void DrawCastButtons()
		{
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var castSplineContent = new GUIContent(useText ? CastSplineButtonTitle : string.Empty, useImages ? editorSettings.castSplineIcon : null, useText ? CastSplineButtonTooltip : CastSplineButtonTitle);
            if (GUILayout.Button(castSplineContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				var referenceTransform = customTransform == null ? BezierSplineEditor.CurrentSpline.transform : customTransform;
				var castDirection = -referenceTransform.up;
				BezierSplineEditor.CastCurvePoints(castDirection);
				repaintScene = true;
            }

            var castSplineToCameraContent = new GUIContent(useText ? CastSplineToCameraButtonTitle : string.Empty, useImages ? editorSettings.castToCameraSplineIcon : null, useText ? CastSplineToCameraButtonTooltip : CastSplineToCameraButtonTitle);
            if (GUILayout.Button(castSplineToCameraContent, buttonStyle, ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.CastSplineToCameraView();
				repaintScene = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(CastTransformFieldLabel);
            customTransform = EditorGUILayout.ObjectField(customTransform, typeof(Transform), true, CustomTransformFieldWidth) as Transform;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }


    }
}
