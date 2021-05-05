using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

        private static SplineEditorSettings editorSettings = default;

        private bool repaintScene = false;
        private bool isSplineEditorEnabled = false;
        private bool isCurveEditorEnabled = false;

        private int buttonsLayoutIndex = 2;

        [MenuItem("Window/Spline Editor")]
        static void Initialize()
        {
            SplineEditorWindow window = (SplineEditorWindow)EditorWindow.GetWindow(typeof(SplineEditorWindow), false, WindowTitle);
            window.LoadSettings();
            window.InitializeStyles();
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
            Repaint();
		}

        private void OnSelectedSplineChanged()
		{
            isSplineEditorEnabled = SplineEditor.CurrentSpline != null;
            OnSelectedCurveChanged();
        }

        private void OnSelectedCurveChanged()
        {
            isCurveEditorEnabled = isSplineEditorEnabled && SplineEditor.IsAnyPointSelected;
            Repaint();
        }

        private void LoadSettings()
        {
            editorSettings = Resources.Load<SplineEditorSettings>("SplineEditorSettings");
        }

        private void OnGUI()
        {
            repaintScene = false;
            //Hack for getting hover mouse visuals before showing tooltip when using custom GUI.skin pt.1
            wantsMouseMove = true;

            if (editorSettings==null)
            {
                LoadSettings();
			}

            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
			DrawHeader();
            GUILayout.Space(10);
			DrawLayoutsToolbar();

            UpdateStyles();

            DrawBezierCurveOptions();
            GUILayout.Space(10);
            DrawSplineGroup();
            GUILayout.Space(10);
            DrawDrawerToolOptions();
			EditorGUILayout.EndVertical();
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
            GUILayout.Label(headerContent, editorSettings.guiSkin.FindStyle("Header"));
        }

        private void DrawLayoutsToolbar()
		{
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            buttonsLayoutIndex = GUILayout.Toolbar(buttonsLayoutIndex, layoutsButtonsContent, ToolsHeaderToolbarWidth, ToolsHeaderToolbarHeight);
            GUILayout.FlexibleSpace();
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
