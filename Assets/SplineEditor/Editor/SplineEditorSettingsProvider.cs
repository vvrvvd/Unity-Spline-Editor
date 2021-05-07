using System.Collections.Generic;
using System.IO;
using UnityEditor;

class SplineEditorSettingsProvider : SettingsProvider
{
    const string k_MyCustomSettingsPath = "Resources/SplineEditorWindowSettings.asset";
    public SplineEditorSettingsProvider(string path, SettingsScope scope)
        : base(path, scope) { }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_MyCustomSettingsPath);
    }

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/Spline Editor", SettingsScope.Project)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "Spline Editor",
            guiHandler = (searchContext) =>
            {
                //var settings = MyCustomSettings.GetSerializedSettings();
                //EditorGUILayout.PropertyField(settings.FindProperty("m_Number"), new GUIContent("My Number"));
                //EditorGUILayout.PropertyField(settings.FindProperty("m_SomeString"), new GUIContent("My String"));
                //settings.ApplyModifiedPropertiesWithoutUndo();
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };

        return provider;
    }

}