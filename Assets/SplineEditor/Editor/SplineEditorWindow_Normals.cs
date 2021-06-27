using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		#region Private Fields

		private bool isNormalsEditorMode = false;
        private bool isNormalsSectionFolded = true;
		private bool previousDrawNormals = false;
		private bool previousFlipNormals = false;
		private float previousNormalsGlobalRotation = 0f;
		private float previousNormalLocalRotation = 0f;

		#endregion

		#region Initialize Normals Editor Mode

		#endregion

		#region Draw Scene GUI

		private void DrawNormalsEditorOptions()
		{
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;

			isNormalsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isNormalsSectionFolded, NormalsEditorGroupTitle);
            GUI.enabled = isSplineEditorEnabled;

            if (isNormalsSectionFolded)
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
			var toggleState = isSplineEditorEnabled && isNormalsEditorMode;
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
			previousFlipNormals = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.FlipNormals : previousFlipNormals;
			GUILayout.Label(FlipNormalsToggleFieldContent);
			GUILayout.Space(75);
			var nextFlipNormals = GUILayout.Toggle(previousFlipNormals, string.Empty);
			if (nextFlipNormals != previousFlipNormals)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Toggle Flip Normals");
				SplineEditor.CurrentSpline.FlipNormals = nextFlipNormals;
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}
			previousFlipNormals = nextFlipNormals;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawNormalLocalRotationField()
		{
			var prevEnabled = GUI.enabled;
			var currentSpline = SplineEditor.CurrentSpline;
			var isNormalsEditorEnabled = currentSpline != null && SplineEditor.IsAnyPointSelected && SplineEditor.SelectedPointIndex % 3 == 0;
			var normalIndex = SplineEditor.SelectedPointIndex / 3;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUI.enabled = isNormalsEditorEnabled;
			previousNormalLocalRotation = isNormalsEditorEnabled ? currentSpline.NormalsAngularOffsets[normalIndex] : previousNormalLocalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorLocalRotationContent, previousNormalLocalRotation);
			if (nextNormalsRotation != previousNormalLocalRotation)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Change Normal Local Rotation");
				currentSpline.UpdateNormalAngularOffset(normalIndex, nextNormalsRotation);
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}

			previousNormalLocalRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawNormalsGlobalRotationField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			previousNormalsGlobalRotation = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.GlobalNormalsRotation : previousNormalsGlobalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorGlobalRotationContent, previousNormalsGlobalRotation);
			if (nextNormalsRotation != previousNormalsGlobalRotation)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Change Normals Global Rotation");
				SplineEditor.CurrentSpline.GlobalNormalsRotation = nextNormalsRotation;

				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}

			previousNormalsGlobalRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

	}

	#endregion

}
