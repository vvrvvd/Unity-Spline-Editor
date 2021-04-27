using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{
        private const string Title = "Spline Editor";

        private static SplineEditorSettings editorSettings = default;


        [MenuItem("Window/Spline Editor")]
        static void Init()
        {
            SplineEditorWindow window = (SplineEditorWindow)EditorWindow.GetWindow(typeof(SplineEditorWindow), false, Title);
            window.Show();

        }

        private void OnGUI()
        {
            if(editorSettings==null)
			{
                InitSettings();
			}

            var prevGUISkin = GUI.skin;
            GUI.skin = editorSettings.guiSkin;
            EditorGUILayout.BeginVertical();
            DrawSplineGroup();
            //DrawUILine(Color.grey, 2, 10);
            DrawBezierCurveOptions();
            EditorGUILayout.EndVertical();
            GUI.skin = prevGUISkin;
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

        private void InitSettings()
		{
            editorSettings = Resources.Load<SplineEditorSettings>("SplineEditorSettings");
        }

    }

}
