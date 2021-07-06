using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		#region Draw Scene GUI

		private void DrawNormalsEditorOptions()
		{
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;

			editorWindowState.isNormalsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.isNormalsSectionFolded, NormalsEditorGroupTitle);
            GUI.enabled = isSplineEditorEnabled;

            if (editorWindowState.isNormalsSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);

				GUILayout.Space(10);
				DrawRotateNormalsButton();
				DrawFlipNormalsToggle();
				DrawNormalLocalRotationField();
				DrawNormalsGlobalRotationField();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
            GUI.enabled = prevEnabled;
        }

		private void DrawRotateNormalsButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUI.enabled = isSplineEditorEnabled;
			var toggleState = isSplineEditorEnabled && editorWindowState.isNormalsEditorMode;
			if (GUILayout.Toggle(toggleState, NormalsEditorButtonContent, toggleButtonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				SplineEditor.ToggleNormalsEditorMode();
				repaintScene = true;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawFlipNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			editorWindowState.previousFlipNormals = editorState.CurrentSpline != null ? editorState.CurrentSpline.FlipNormals : editorWindowState.previousFlipNormals;
			GUILayout.Label(FlipNormalsToggleFieldContent);
			GUILayout.Space(75);
			var nextFlipNormals = GUILayout.Toggle(editorWindowState.previousFlipNormals, string.Empty);
			if (nextFlipNormals != editorWindowState.previousFlipNormals)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Flip Normals");
				editorState.CurrentSpline.FlipNormals = nextFlipNormals;
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}
			editorWindowState.previousFlipNormals = nextFlipNormals;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawNormalLocalRotationField()
		{
			var prevEnabled = GUI.enabled;
			var currentSpline = editorState.CurrentSpline;
			var isNormalsEditorEnabled = currentSpline != null && editorState.IsAnyPointSelected && editorState.SelectedPointIndex % 3 == 0;
			var normalIndex = editorState.SelectedPointIndex / 3;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.enabled = isNormalsEditorEnabled;
			editorWindowState.previousNormalLocalRotation = isNormalsEditorEnabled ? currentSpline.NormalsAngularOffsets[normalIndex] : editorWindowState.previousNormalLocalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorLocalRotationContent, editorWindowState.previousNormalLocalRotation);
			if (nextNormalsRotation != editorWindowState.previousNormalLocalRotation)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Change Normal Local Rotation");
				currentSpline.UpdateNormalAngularOffset(normalIndex, nextNormalsRotation);
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}

			editorWindowState.previousNormalLocalRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawNormalsGlobalRotationField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			editorWindowState.previousNormalsGlobalRotation = editorState.CurrentSpline != null ? editorState.CurrentSpline.GlobalNormalsRotation : editorWindowState.previousNormalsGlobalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorGlobalRotationContent, editorWindowState.previousNormalsGlobalRotation);
			if (nextNormalsRotation != editorWindowState.previousNormalsGlobalRotation)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Change Normals Global Rotation");
				editorState.CurrentSpline.GlobalNormalsRotation = nextNormalsRotation;

				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}

			editorWindowState.previousNormalsGlobalRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

	#endregion

}
