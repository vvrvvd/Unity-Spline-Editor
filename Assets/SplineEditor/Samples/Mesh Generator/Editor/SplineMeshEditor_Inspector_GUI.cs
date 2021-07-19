using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

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

			var toggleState = EditorGUILayout.Toggle(GuiOptionsDrawPointsToggleContent, meshEditorState.DrawPoints);
			if (toggleState != meshEditorState.DrawPoints)
			{
				Undo.RecordObject(meshEditorState, "Toggle Draw Points");
				meshEditorState.DrawPoints = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.EndHorizontal();
		}

		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();

			var toggleState = EditorGUILayout.Toggle(GuiOptionsDrawNormalToggleContent, meshEditorState.DrawNormals);
			if (toggleState != meshEditorState.DrawNormals)
			{
				Undo.RecordObject(meshEditorState, "Toggle Draw Normals");
				meshEditorState.DrawNormals = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.EndHorizontal();
		}

	}

}
