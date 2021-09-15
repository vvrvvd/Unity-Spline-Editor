// <copyright file="SplineMeshEditor_Inspector_UV.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing UV options for custom inspector to SplineMesh component.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private void DrawUvOptions()
		{
			var prevEnabled = GUI.enabled;

			MeshEditorState.IsUvSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(MeshEditorState.IsUvSectionFolded, UvOptionsGroupTitle);
			GUI.enabled = true;
			if (MeshEditorState.IsUvSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawMirrorUvToggle();
				DrawUvModeDropdown();
				GUILayout.Space(10);
				DrawShowUvButton();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawMirrorUvToggle()
		{
			GUILayout.BeginHorizontal();

			var toggleState = EditorGUILayout.Toggle(UvOptionsMirrorUvToggleContent, splineMesh.MirrorUV);
			if (toggleState != splineMesh.MirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.MirrorUV = toggleState;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawUvModeDropdown()
		{
			GUILayout.BeginHorizontal();

			var modeState = (SplineMesh.UVMode)EditorGUILayout.EnumPopup(UvOptionsUvModeDropdownContent, splineMesh.UvMode);
			if (modeState != splineMesh.UvMode)
			{
				Undo.RecordObject(splineMesh, "Change mesh UV Mode");
				splineMesh.UvMode = modeState;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawShowUvButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);

			var isDebugModeView = MeshEditorState.IsDebugModeView(splineMesh);
			var uvButtonContent = isDebugModeView ? UvOptionsHideDebugViewButtonContent : UvOptionsShowDebugViewButtonContent;
			if (GUILayout.Button(uvButtonContent, buttonStyle, ButtonHeight))
			{
				MeshEditorState.SetDebugModeView(splineMesh, !isDebugModeView);
			}

			GUILayout.Space(20);
			GUILayout.EndHorizontal();
		}
	}
}
