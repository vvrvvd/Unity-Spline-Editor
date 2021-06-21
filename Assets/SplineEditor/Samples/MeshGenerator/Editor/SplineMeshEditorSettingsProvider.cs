using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

    public class SplineMeshEditorSettingsProvider : SettingsProvider
    {
        private static string SettingsPath => $"Resources/{SplineMeshEditor.SplineMeshEditorSettingsName}.asset";
        private static UnityEditor.Editor cachedEditor;

        public SplineMeshEditorSettingsProvider(string path, SettingsScope scope)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(SettingsPath);
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var settingsScriptable = Resources.Load(SplineMeshEditor.SplineMeshEditorSettingsName);

            if(cachedEditor==null)
			{
                UnityEditor.Editor.CreateCachedEditor(settingsScriptable, null, ref cachedEditor);
			}

            var provider = new SettingsProvider("Project/Spline Editor/Mesh Generator", SettingsScope.Project)
            {
                label = "Mesh Generator",
                guiHandler = (searchContext) =>
                {
                    var prevLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 250;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.Space(10);
                    cachedEditor.OnInspectorGUI();
                    EditorGUILayout.Space(10);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(20);
                    EditorGUI.indentLevel--;
                    EditorGUIUtility.labelWidth = prevLabelWidth;
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Spline", "Editor", "Bezier", "Curve", "Mesh", "Generator" })
            };

            return provider;
        }

    }

}