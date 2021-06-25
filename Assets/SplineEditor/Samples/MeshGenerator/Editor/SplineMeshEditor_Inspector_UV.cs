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
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
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
			var modeState = (SplineMesh.UVMode)EditorGUILayout.EnumPopup(string.Empty, splineMesh.uvMode, MaxDropdownWidth);
			if (modeState != splineMesh.uvMode)
			{
				Undo.RecordObject(splineMesh, "Change mesh UV Mode");
				splineMesh.uvMode = modeState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawShowUvButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var uvButtonTitle = splineMesh.IsVisualizingUV ? "Hide UV" : "Show UV";
			if (GUILayout.Button(uvButtonTitle))
			{
				splineMesh.ToggleUV();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

}
