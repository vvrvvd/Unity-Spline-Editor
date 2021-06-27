using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private bool isUvSectionFolded = true;

		private void DrawUvOptions()
		{
			var prevEnabled = GUI.enabled;

			isUvSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isUvSectionFolded, UvOptionsGroupTitle);
			GUI.enabled = true;
			if (isUvSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);

				DrawMirrorUvToggle();
				DrawUvModeDropdown();
				GUILayout.Space(10);
				DrawShowUvButton();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawMirrorUvToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(UvOptionsMirrorUvToggleContent);
			GUILayout.Space(10);
			var toggleState = GUILayout.Toggle(splineMesh.MirrorUV, string.Empty);
			if (toggleState != splineMesh.MirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.MirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawUvModeDropdown()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(UvOptionsUvModeDropdownContent);
			var modeState = (SplineMesh.UVMode)EditorGUILayout.EnumPopup(string.Empty, splineMesh.UvMode, MaxDropdownWidth);
			if (modeState != splineMesh.UvMode)
			{
				Undo.RecordObject(splineMesh, "Change mesh UV Mode");
				splineMesh.UvMode = modeState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawShowUvButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var uvButtonContent = isVisualizingUV ? UvOptionsHideDebugUvViewButtonContent : UvOptionsShowDebugUvViewButtonContent;
			if (GUILayout.Button(uvButtonContent, ButtonMaxWidth, ButtonHeight))
			{
				ToggleUV(!isVisualizingUV);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void ToggleUV(bool state)
		{
			if (isVisualizingUV == state)
			{
				return;
			}

			var settingsScriptable = SplineMeshEditorSettings.instance;

			isVisualizingUV = state;
			var prevMaterial = splineMesh.MeshRenderer.sharedMaterial;
			splineMesh.MeshRenderer.sharedMaterial = isVisualizingUV ? settingsScriptable.uvMaterial : savedMaterial;
			savedMaterial = isVisualizingUV ? prevMaterial : settingsScriptable.uvMaterial;
		}

	}

}
