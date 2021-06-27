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

				DrawDrawPointsToggle();
				DrawDrawNormalsToggle();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawDrawPointsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(GuiOptionsDrawPointsToggleContent);
			var toggleState = GUILayout.Toggle(SplineMeshEditorState.instance.drawPoints, string.Empty);
			if (toggleState != SplineMeshEditorState.instance.drawPoints)
			{
				Undo.RecordObject(splineMesh, "Toggle Draw Points");
				SplineMeshEditorState.instance.drawPoints = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(GuiOptionsDrawNormalToggleContent);
			var toggleState = GUILayout.Toggle(SplineMeshEditorState.instance.drawNormals, string.Empty);
			if (toggleState != SplineMeshEditorState.instance.drawNormals)
			{
				Undo.RecordObject(splineMesh, "Toggle Draw Normals");
				SplineMeshEditorState.instance.drawNormals = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

}
