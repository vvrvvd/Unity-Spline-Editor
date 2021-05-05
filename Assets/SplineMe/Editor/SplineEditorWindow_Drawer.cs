using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private void DrawDrawerToolOptions()
		{
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;
            GUI.enabled = isSplineEditorEnabled;
            GUILayout.Label(DrawerGroupTitle);
            GUILayout.BeginVertical(groupsStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = isSplineEditorEnabled && !SplineEditor.CurrentSpline.IsLoop;
            var toggleState = isSplineEditorEnabled ? SplineEditor.CurrentEditor.isCurveDrawerMode : false;
            if (GUILayout.Toggle(toggleState, DrawCurveButtonContent, editorSettings.guiSkin.FindStyle("DrawerButton"), ToolsButtonsWidth, ToolsButtonsHeight))
            {
				SplineEditor.ToggleDrawSplineMode();
				repaintScene = true;
            }

            GUI.color = prevColor;

            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            SplineEditor.DrawCurveSmoothAcuteAngles = EditorGUILayout.Toggle(DrawCurveSmoothAnglesContent, SplineEditor.DrawCurveSmoothAcuteAngles);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			SplineEditor.DrawCurveSegmentLength = EditorGUILayout.FloatField(DrawCurveSegmentLengthContent, SplineEditor.DrawCurveSegmentLength);
            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(DrawCurveFirstHookContent);
            SplineEditor.DrawCurveFirstPointHook = EditorGUILayout.Slider(SplineEditor.DrawCurveFirstPointHook, 0.001f, SplineEditor.DrawCurveSecondPointHook, ToolsSliderWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(DrawCurveSecondHookContent);
            SplineEditor.DrawCurveSecondPointHook = EditorGUILayout.Slider(SplineEditor.DrawCurveSecondPointHook, SplineEditor.DrawCurveFirstPointHook, 0.999f, ToolsSliderWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUI.enabled = prevEnabled;
        }

    }

}
