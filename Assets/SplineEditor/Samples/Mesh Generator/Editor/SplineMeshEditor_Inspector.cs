// <copyright file="SplineMeshEditor_Inspector.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing custom inspector GUI to SplineMesh component.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private bool initializeStyles = true;

		/// <summary>
		/// Draws custom inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			if (initializeStyles)
			{
				InitializeStyles();
				initializeStyles = false;
			}

			GUILayout.BeginVertical();
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((SplineMesh)target), typeof(SplineMesh), false);
			GUI.enabled = true;

			DrawMeshOptions();
			DrawWidthCurveOptions();
			DrawUvOptions();
			DrawGUIOptions();
			GUILayout.EndVertical();

			var prevEnabled = GUI.enabled;

			GUI.enabled = prevEnabled;
		}
	}
}
