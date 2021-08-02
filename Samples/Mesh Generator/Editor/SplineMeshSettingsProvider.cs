// <copyright file="SplineMeshSettingsProvider.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using UnityEditor;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Component providing a neat Unity settings window.
	/// Accessible through "Edit/Project Settings.../Spline Editor/Mesh Generator".
	/// </summary>
	public class SplineMeshSettingsProvider : SettingsProvider
	{
		private const string SettingsPath = "Project/Spline Editor/Mesh Generator";
		private const string SettingsLabel = "Mesh Generator";

		private static UnityEditor.Editor cachedEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="SplineMeshSettingsProvider"/> class.
		/// </summary>
		/// <param name="path">Settings path.</param>
		/// <param name="scope">Settings scope.</param>
		public SplineMeshSettingsProvider(string path, SettingsScope scope) : base(path, scope)
		{
		}

		/// <summary>
		/// Creates settings provider for SplineMesh editor configuration.
		/// </summary>
		/// <returns>Instance of settings provider for SplineMesh editor configuration.</returns>
		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			var settingsScriptable = SplineMeshConfiguration.Instance;

			if (cachedEditor == null)
			{
				UnityEditor.Editor.CreateCachedEditor(settingsScriptable, null, ref cachedEditor);
			}

			var provider = new SettingsProvider(SettingsPath, SettingsScope.Project)
			{
				label = SettingsLabel,
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