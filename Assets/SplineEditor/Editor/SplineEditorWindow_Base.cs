using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{
        private static SplineEditorConfiguration editorSettings = default;

        private bool repaintScene = false;
        private bool isSplineEditorEnabled = false;
        private bool isCurveEditorEnabled = false;

        private int buttonsLayoutIndex = 2;
        private Vector2 scrollPos = Vector2.zero;

        [MenuItem("Window/Spline Editor")]
        public static void Initialize()
        {
            SplineEditorWindow window = (SplineEditorWindow)EditorWindow.GetWindow(typeof(SplineEditorWindow), false, WindowTitle);
            window.LoadSettings();
            window.InitializeStyles(editorSettings);
            window.Show();
        }

		private void OnEnable()
		{
            SplineEditor.OnSplineModified += OnSplineModified;
            SplineEditor.OnSelectedSplineChanged += OnSelectedSplineChanged;
            SplineEditor.OnSelectedPointChanged += OnSelectedCurveChanged;
        }

        private void OnDisable()
		{
            SplineEditor.OnSplineModified -= OnSplineModified;
            SplineEditor.OnSelectedSplineChanged -= OnSelectedSplineChanged;
            SplineEditor.OnSelectedPointChanged -= OnSelectedCurveChanged;
        }

        private void OnSplineModified()
		{
            isDrawerMode = SplineEditor.CurrentEditor != null && SplineEditor.IsDrawerMode;
            Repaint();
		}

        private void OnSelectedSplineChanged()
		{
            isSplineEditorEnabled = SplineEditor.CurrentSpline != null;
            isDrawerMode = SplineEditor.CurrentEditor != null && SplineEditor.IsDrawerMode;
            OnSelectedCurveChanged();
        }

        private void OnSelectedCurveChanged()
        {
            isCurveEditorEnabled = isSplineEditorEnabled && SplineEditor.IsAnyPointSelected;
            Repaint();
        }

        private void LoadSettings()
        {
            editorSettings = Resources.Load<SplineEditorConfiguration>(SplineEditor.SplineEditorSettingsName);

            if (editorSettings == null)
            {
                Debug.LogError("[Spline Editor] Spline Editor settings couldn't be loaded!");
                return;
            }
        }


        private void OnGUI()
        {
            repaintScene = false;
            //Hack for getting hover mouse visuals before showing tooltip when using custom GUI.skin pt.1
            wantsMouseMove = true;

            if(editorSettings==null)
			{
                LoadSettings();
            }

            SplineEditor.UpdateSplineStates();

            isCurveEditorEnabled &= SplineEditor.CurrentSpline != null;
            isSplineEditorEnabled &= SplineEditor.CurrentSpline != null;

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
			DrawHeader();
            GUILayout.Space(10);
            DrawSettingsButton();
			DrawLayoutsToolbar();

            UpdateStyles(editorSettings);

            DrawPointGroup();
            GUILayout.Space(3);
            DrawCurveOptions();
            GUILayout.Space(3);
            DrawSplineGroup();
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
