// <copyright file="SplineEditorSettingsProvider.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Component providing a neat Unity settings window.
	/// Accessible through "Edit/Project Settings.../Spline Editor".
	/// </summary>
	public class SplineEditorSettingsProvider : SettingsProvider
	{
		private const string SplineEditorSettingsName = "SplineEditorSettings";

		private static UnityEditor.Editor cachedEditor;

		/// <summary>
		/// Initializes a new instance of the <see cref="SplineEditorSettingsProvider"/> class.
		/// </summary>
		/// <param name="path">Settings path.</param>
		/// <param name="scope">Settings scope.</param>
		public SplineEditorSettingsProvider(string path, SettingsScope scope) : base(path, scope)
		{
		}

		/// <summary>
		/// Creates settings provider for BezierSpline editor configuration.
		/// </summary>
		/// <returns>Instance of settings provider for BezierSpline editor configuration.</returns>
		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			var settingsScriptable = Resources.Load(SplineEditorSettingsName);

			if (cachedEditor == null)
			{
				UnityEditor.Editor.CreateCachedEditor(settingsScriptable, null, ref cachedEditor);
			}

			var provider = new SettingsProvider("Project/Spline Editor", SettingsScope.Project)
			{
				label = "Spline Editor",
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
				keywords = new HashSet<string>(new[] { "Spline", "Editor", "Bezier", "Curve" })
			};

			return provider;
		}
	}
}