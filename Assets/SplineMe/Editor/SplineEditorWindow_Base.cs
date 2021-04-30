using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{
        private const string WindowTitle = "Spline Editor";
        private const string HeaderTitle = "Spline Editor";

        private static SplineEditorSettings editorSettings = default;

        private GUILayoutOption ButtonWidth { get; } = GUILayout.Width(90);
        private GUILayoutOption ButtonHeight { get; } = GUILayout.Height(50);

        private bool repaintScene = false;
        private bool isSplineEditorEnabled = false;
        private bool isCurveEditorEnabled = false;

        [MenuItem("Window/Spline Editor")]
        static void Initialize()
        {
            SplineEditorWindow window = (SplineEditorWindow)EditorWindow.GetWindow(typeof(SplineEditorWindow), false, WindowTitle);
            window.LoadSettings();
            window.Show();
        }

		private void OnEnable()
		{
            BezierSplineEditor.OnCurrentSplineChanged += OnSelectedSplineChanged;
            BezierSplineEditor.OnSelectedCurveChanged += OnSelectedCurveChanged;
        }

        private void OnDisable()
		{
            BezierSplineEditor.OnCurrentSplineChanged -= OnSelectedSplineChanged;
            BezierSplineEditor.OnSelectedCurveChanged -= OnSelectedCurveChanged;
        }

        private void OnSelectedSplineChanged()
		{
            isSplineEditorEnabled = BezierSplineEditor.CurrentSpline != null;
            isCurveEditorEnabled &= isSplineEditorEnabled;
            Repaint();
        }

        private void OnSelectedCurveChanged()
        {
            isCurveEditorEnabled = isSplineEditorEnabled && BezierSplineEditor.CurrentEditor != null  && BezierSplineEditor.CurrentEditor.SelectedCurveIndex != -1;
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

            var prevGUISkin = GUI.skin;
            GUI.skin = editorSettings.guiSkin;
            EditorGUILayout.BeginVertical();
            DrawHeader();
            DrawSplineGroup();
            DrawBezierCurveOptions();
            EditorGUILayout.EndVertical();
            GUI.skin = prevGUISkin;

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
            GUILayout.Space(10);
            var headerContent = new GUIContent(HeaderTitle);
            GUILayout.Label(headerContent, editorSettings.guiSkin.FindStyle("Header"));

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
