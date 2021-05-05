using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private const string DrawerGroupTitle = "Drawer Tool";
        private const string DrawCurveButtonTitle = "Drawer Tool";
        private const string DrawCurveButtonTooltip = "Draw spline using mouse.";
        private const string DrawCurveSegmentLengthLabel = "Segment length";
        private const string DrawCurveSmoothAnglesLabel = "Smooth angles";
        private const string DrawCurveFirstHookLabel = "1st point hook";
        private const string DrawCurveSecondHookLabel = "2nd point hook";

        private void DrawDrawerToolOptions()
		{
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;
            GUI.enabled = isSplineEditorEnabled;
            GUILayout.Label(DrawerGroupTitle);
            var groupStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(groupStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = isSplineEditorEnabled && !SplineEditor.CurrentSpline.IsLoop;
            var addCurveButtonContent = new GUIContent(useText ? DrawCurveButtonTitle : string.Empty, useImages ? editorSettings.drawerToolIcon : null, useText ? DrawCurveButtonTooltip : DrawCurveButtonTitle);
            var toggleState = isSplineEditorEnabled ? SplineEditor.CurrentEditor.isCurveDrawerMode : false;
            if (GUILayout.Toggle(toggleState, addCurveButtonContent, editorSettings.guiSkin.FindStyle("DrawerButton"), ButtonWidth, ButtonHeight))
            {
				SplineEditor.ToggleDrawSplineMode();
				repaintScene = true;
            }

            GUI.color = prevColor;

            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            SplineEditor.DrawCurveSmoothAcuteAngles = EditorGUILayout.Toggle(DrawCurveSmoothAnglesLabel, SplineEditor.DrawCurveSmoothAcuteAngles);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			SplineEditor.DrawCurveSegmentLength = EditorGUILayout.FloatField(DrawCurveSegmentLengthLabel, SplineEditor.DrawCurveSegmentLength);
            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(DrawCurveFirstHookLabel);
            SplineEditor.DrawCurveFirstPointHook = EditorGUILayout.Slider(SplineEditor.DrawCurveFirstPointHook, 0.001f, SplineEditor.DrawCurveSecondPointHook, CustomSliderWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(DrawCurveSecondHookLabel);
            SplineEditor.DrawCurveSecondPointHook = EditorGUILayout.Slider(SplineEditor.DrawCurveSecondPointHook, SplineEditor.DrawCurveFirstPointHook, 0.999f, CustomSliderWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
