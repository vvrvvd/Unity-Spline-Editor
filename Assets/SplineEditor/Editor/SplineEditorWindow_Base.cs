using SplineEditor.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{
        private static SplineEditorState editorState => SplineEditorState.instance;
        private static SplineEditorWindowState editorWindowState => SplineEditorWindowState.instance;
        private static SplineEditorConfiguration editorSettings => SplineEditorConfiguration.instance;

        private bool repaintScene = false;
        private bool isSplineEditorEnabled = false;
        private bool isCurveEditorEnabled = false;

        private int buttonsLayoutIndex = 2;
        private Vector2 scrollPos = Vector2.zero;

        private bool initializeStyles = false;

        [MenuItem("Window/Spline Editor")]
        public static void Initialize()
        {
            var inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<SplineEditorWindow>(WindowTitle, false, inspectorType);
            window.initializeStyles = true;
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        private void OnEnable()
		{
            editorState.OnSplineModified += OnSplineModified;
            editorState.OnSelectedSplineChanged += OnSelectedSplineChanged;
            editorState.OnSelectedPointChanged += OnSelectedCurveChanged;
        }

        private void OnDisable()
		{
            editorState.OnSplineModified -= OnSplineModified;
            editorState.OnSelectedSplineChanged -= OnSelectedSplineChanged;
            editorState.OnSelectedPointChanged -= OnSelectedCurveChanged;
        }

        private void OnSplineModified()
		{
            editorState.IsDrawerMode = editorState.CurrentEditor != null && editorState.IsDrawerMode;
            editorState.IsNormalsEditorMode = editorState.CurrentEditor != null && editorState.IsNormalsEditorMode;
            Repaint();
		}

        private void OnSelectedSplineChanged()
		{
            isSplineEditorEnabled = editorState.CurrentSpline != null;
            editorState.IsDrawerMode = editorState.CurrentEditor != null && editorState.IsDrawerMode;
            editorState.IsNormalsEditorMode = editorState.CurrentEditor != null && editorState.IsNormalsEditorMode;
            OnSelectedCurveChanged();
        }

        private void OnSelectedCurveChanged()
        {
            isCurveEditorEnabled = isSplineEditorEnabled && editorState.IsAnyPointSelected;
            Repaint();
        }


        private void OnGUI()
        {
            repaintScene = false;

            //Hack for getting hover mouse visuals before showing tooltip when using custom GUI.skin pt.1
            wantsMouseMove = true;

            if(initializeStyles)
			{
                InitializeStyles();
                initializeStyles = false;
            }

            editorState.UpdateSplineStates();

            isCurveEditorEnabled &= editorState.CurrentSpline != null;
            isSplineEditorEnabled &= editorState.CurrentSpline != null;

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
			DrawHeader();
            GUILayout.Space(10);
            DrawSettingsButton();
			DrawLayoutsToolbar();

            UpdateStyles();

            DrawPointGroup();
            GUILayout.Space(3);
            DrawCurveOptions();
            GUILayout.Space(3);
            DrawSplineGroup();
            GUILayout.Space(3);
            DrawNormalsEditorOptions();
            GUILayout.Space(3);
            DrawDrawerToolOptions();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            //Hack for getting hover mouse visuals before showing tooltip when using custom GUI.skin pt.2
            if (Event.current.type == EventType.MouseMove)
            {
                Repaint();
            }
        
            if(repaintScene)
			{
                SceneView.RepaintAll();
            }

        }

        private void DrawHeader()
		{
            var headerContent = new GUIContent(HeaderTitle);
            GUILayout.Label(headerContent, headerLabelStyle);
        }

        private void DrawLayoutsToolbar()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			buttonsLayoutIndex = GUILayout.Toolbar(buttonsLayoutIndex, layoutsButtonsContent, ToolsHeaderToolbarWidth, ToolsHeaderToolbarHeight);
			
            GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSettingsButton()
		{
			GUILayout.Space(-40);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button(settingsButtonContent, settingsButtonStyle, ToolsSettingsButtonWidth, ToolsSettingsButtonHeight))
			{
                SettingsService.OpenProjectSettings("Project/Spline Editor");
            }

			GUILayout.EndHorizontal();
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

    }

}
