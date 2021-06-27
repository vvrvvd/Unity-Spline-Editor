using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

    public class SplineMeshSettingsProvider : SettingsProvider
    {
        private static UnityEditor.Editor cachedEditor;

        public SplineMeshSettingsProvider(string path, SettingsScope scope)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
			return true;
		}

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
			var settingsScriptable = SplineMeshEditorSettings.instance;

            if(cachedEditor==null)
			{
                UnityEditor.Editor.CreateCachedEditor(settingsScriptable, null, ref cachedEditor);
			}

			//TODO: Move to consts
            var provider = new SettingsProvider("Project/Spline Editor/Mesh Generator", SettingsScope.Project)
            {
				//TODO: Move to consts
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