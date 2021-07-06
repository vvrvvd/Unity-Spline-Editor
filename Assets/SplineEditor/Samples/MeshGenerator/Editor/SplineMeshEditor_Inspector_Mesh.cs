using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{


		private void DrawMeshOptions()
		{
			var prevEnabled = GUI.enabled;

			meshEditorState.isMeshSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(meshEditorState.isMeshSectionFolded, MeshOptionsGroupTitle);
			GUI.enabled = true;
			if (meshEditorState.isMeshSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);

				DrawUsePointsScaleToggle();
				DrawMeshWidthField();
				DrawMeshSpacingField();

				GUILayout.Space(10);

				DrawBakeMeshButton();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawUsePointsScaleToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var toggleState = EditorGUILayout.Toggle(MeshOptionsUsePointsScaleToggleContent, splineMesh.UsePointsScale);
			if (toggleState != splineMesh.UsePointsScale)
			{
				Undo.RecordObject(splineMesh, "Toggle use points scale");
				splineMesh.UsePointsScale = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawMeshSpacingField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var nextMeshSpacing = EditorGUILayout.FloatField(MeshOptionsMeshSpacingFieldContent, splineMesh.Spacing);
			if (nextMeshSpacing != splineMesh.Spacing)
			{
				Undo.RecordObject(splineMesh, "Change spline mesh spacing");
				splineMesh.Spacing = nextMeshSpacing;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawMeshWidthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var nextMeshWidth = EditorGUILayout.FloatField(MeshOptionsMeshWidthFieldContent, splineMesh.Width);
			if (nextMeshWidth != splineMesh.Width)
			{
				Undo.RecordObject(splineMesh, "Change spline mesh width");
				splineMesh.Width = nextMeshWidth;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawBakeMeshButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(MeshOptionsBakeMeshButtonContent, ButtonMaxWidth, ButtonHeight))
			{
				splineMesh.ConstructMesh();
				var path = EditorUtility.SaveFilePanel(MeshOptionsBakeMeshWindowTitle, MeshOptionsBakeMeshWindowFolderPath, MeshOptionsBakeMeshWindowFileName, MeshOptionsBakeMeshWindowFileExtension);
				if (string.IsNullOrEmpty(path)) return;

				path = FileUtil.GetProjectRelativePath(path);
				var mesh = splineMesh.MeshFilter.sharedMesh;
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}


	}

}
