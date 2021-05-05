using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{
        private const string WindowTitle = "Spline Editor";
        private const string HeaderTitle = "Spline Editor";

        private static SplineEditorSettings editorSettings = default;

        private GUILayoutOption ButtonWidth { get; } = GUILayout.Width(110);
        private GUILayoutOption ButtonHeight { get; } = GUILayout.Height(50);

        private bool repaintScene = false;
        private bool isSplineEditorEnabled = false;
        private bool isCurveEditorEnabled = false;

        private GUILayoutOption CustomSliderWidth { get; } = GUILayout.Width(175);

        private int buttonsLayoutIndex = 2;
        private GUIContent[] layoutsButtonsContent = new GUIContent[3];

        private bool useText = false;
        private bool useImages = false;
        private GUIStyle buttonStyle;

        [MenuItem("Window/Spline Editor")]
        static void Initialize()
        {
            SplineEditorWindow window = (SplineEditorWindow)EditorWindow.GetWindow(typeof(SplineEditorWindow), false, WindowTitle);
            window.LoadSettings();
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

            buttonStyle = editorSettings.guiSkin.FindStyle("button");

            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
			DrawHeader();
            GUILayout.Space(10);
            DrawLayoutsToolbar();
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
            layoutsButtonsContent[0] = new GUIContent("T", "Text Layout");
            layoutsButtonsContent[1] = new GUIContent(editorSettings.imageLayoutIcon, "Image Layout");
            layoutsButtonsContent[2] = new GUIContent("+T", editorSettings.imageLayoutIcon, "Text & Image Layout");
            buttonsLayoutIndex = GUILayout.Toolbar(buttonsLayoutIndex, layoutsButtonsContent, GUILayout.Width(128), GUILayout.Height(18));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            useText = buttonsLayoutIndex == 0 || buttonsLayoutIndex == 2;
            useImages = buttonsLayoutIndex == 1 || buttonsLayoutIndex == 2;
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
