using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
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

            GUI.enabled = isSplineEditorEnabled && !BezierSplineEditor.CurrentSpline.IsLoop;
            var addCurveButtonContent = new GUIContent(DrawCurveButtonTitle, DrawCurveButtonTooltip);
            var toggleState = isSplineEditorEnabled ? BezierSplineEditor.CurrentEditor.isCurveDrawerMode : false;
            //TODO: Add event
            if (GUILayout.Toggle(toggleState, addCurveButtonContent, editorSettings.guiSkin.FindStyle("DrawerButton"), ButtonWidth, ButtonHeight))
            {
				BezierSplineEditor.ToggleDrawSplineModeShortcut();
				repaintScene = true;
            }

            GUI.color = prevColor;

            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            BezierSplineEditor.drawCurveSmoothAcuteAngles = EditorGUILayout.Toggle(DrawCurveSmoothAnglesLabel, BezierSplineEditor.drawCurveSmoothAcuteAngles);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			BezierSplineEditor.drawCurveSegmentLength = EditorGUILayout.FloatField(DrawCurveSegmentLengthLabel, BezierSplineEditor.drawCurveSegmentLength);
            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            BezierSplineEditor.drawCurveFirstPointHook = EditorGUILayout.Slider(DrawCurveFirstHookLabel, BezierSplineEditor.drawCurveFirstPointHook, 0.001f, BezierSplineEditor.drawCurveSecondPointHook);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            BezierSplineEditor.drawCurveSecondPointHook = EditorGUILayout.Slider(DrawCurveSecondHookLabel, BezierSplineEditor.drawCurveSecondPointHook, BezierSplineEditor.drawCurveFirstPointHook, 0.999f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
