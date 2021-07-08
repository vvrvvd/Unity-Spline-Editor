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

			editorWindowState.IsNormalsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.IsNormalsSectionFolded, NormalsEditorGroupTitle);
            GUI.enabled = isSplineEditorEnabled;

            if (editorWindowState.IsNormalsSectionFolded)
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
			var toggleState = isSplineEditorEnabled && editorState.IsNormalsEditorMode;
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
			editorWindowState.PreviousFlipNormals = editorState.CurrentSpline != null ? editorState.CurrentSpline.FlipNormals : editorWindowState.PreviousFlipNormals;
			GUILayout.Label(FlipNormalsToggleFieldContent);
			GUILayout.Space(75);
			var nextFlipNormals = GUILayout.Toggle(editorWindowState.PreviousFlipNormals, string.Empty);
			if (nextFlipNormals != editorWindowState.PreviousFlipNormals)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Flip Normals");
				editorState.CurrentSpline.FlipNormals = nextFlipNormals;
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}
			editorWindowState.PreviousFlipNormals = nextFlipNormals;
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
			editorWindowState.PreviousNormalLocalRotation = isNormalsEditorEnabled ? currentSpline.NormalsAngularOffsets[normalIndex] : editorWindowState.PreviousNormalLocalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorLocalRotationContent, editorWindowState.PreviousNormalLocalRotation);
			if (nextNormalsRotation != editorWindowState.PreviousNormalLocalRotation)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Change Normal Local Rotation");
				currentSpline.UpdateNormalAngularOffset(normalIndex, nextNormalsRotation);
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}

			editorWindowState.PreviousNormalLocalRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawNormalsGlobalRotationField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			editorWindowState.PreviousNormalsGlobalRotation = editorState.CurrentSpline != null ? editorState.CurrentSpline.GlobalNormalsRotation : editorWindowState.PreviousNormalsGlobalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorGlobalRotationContent, editorWindowState.PreviousNormalsGlobalRotation);
			if (nextNormalsRotation != editorWindowState.PreviousNormalsGlobalRotation)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Change Normals Global Rotation");
				editorState.CurrentSpline.GlobalNormalsRotation = nextNormalsRotation;

				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}

			editorWindowState.PreviousNormalsGlobalRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

	#endregion

}
