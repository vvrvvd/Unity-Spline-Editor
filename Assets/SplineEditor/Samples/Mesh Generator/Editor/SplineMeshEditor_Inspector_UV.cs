using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{


		private void DrawUvOptions()
		{
			var prevEnabled = GUI.enabled;

			meshEditorState.IsUvSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(meshEditorState.IsUvSectionFolded, UvOptionsGroupTitle);
			GUI.enabled = true;
			if (meshEditorState.IsUvSectionFolded)
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
				EditorUtility.SetDirty(splineMesh);
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
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.EndHorizontal();
		}

		private void DrawShowUvButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);

			var isDebugModeView = meshEditorState.IsDebugModeView(splineMesh);
			var uvButtonContent = isDebugModeView ? UvOptionsHideDebugViewButtonContent : UvOptionsShowDebugViewButtonContent;
			if (GUILayout.Button(uvButtonContent, buttonStyle, ButtonHeight))
			{
				meshEditorState.SetDebugModeView(splineMesh, !isDebugModeView);
			}
			
			GUILayout.Space(20);
			GUILayout.EndHorizontal();
		}



	}

}
