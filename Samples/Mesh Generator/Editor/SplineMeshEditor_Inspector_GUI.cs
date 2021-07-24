// <copyright file="SplineMeshEditor_Inspector_GUI.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing GUI options for custom inspector to SplineMesh component.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private bool isGuiSectionFolded = true;

		private void DrawGUIOptions()
		{
			var prevEnabled = GUI.enabled;

			isGuiSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isGuiSectionFolded, GuiOptionsGroupTitle);
			GUI.enabled = true;
			if (isGuiSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawDrawPointsToggle();
				DrawDrawNormalsToggle();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawDrawPointsToggle()
		{
			GUILayout.BeginHorizontal();

			var toggleState = EditorGUILayout.Toggle(GuiOptionsDrawPointsToggleContent, MeshEditorState.DrawPoints);
			if (toggleState != MeshEditorState.DrawPoints)
			{
				Undo.RecordObject(MeshEditorState, "Toggle Draw Points");
				MeshEditorState.DrawPoints = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.EndHorizontal();
		}

		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();

			var toggleState = EditorGUILayout.Toggle(GuiOptionsDrawNormalToggleContent, MeshEditorState.DrawNormals);
			if (toggleState != MeshEditorState.DrawNormals)
			{
				Undo.RecordObject(MeshEditorState, "Toggle Draw Normals");
				MeshEditorState.DrawNormals = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.EndHorizontal();
		}
	}
}
