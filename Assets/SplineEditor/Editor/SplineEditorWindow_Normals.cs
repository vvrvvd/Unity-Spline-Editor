using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private bool isNormalsEditorMode = false;
        private bool isNormalsSectionFolded = true;
		private bool previousDrawNormals = false;
		private bool previousFlipNormals = false;
		private float previousNormalsRotation = 0f;

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

				DrawDrawNormalsToggle();
				DrawFlipNormalsToggle();
				DrawNormalsEditorButton();
				DrawNormalsGlobalRotationField();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
            GUI.enabled = prevEnabled;
        }

		private void DrawNormalsEditorButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUI.enabled = isSplineEditorEnabled;
			var toggleState = isSplineEditorEnabled && isNormalsEditorMode;
			if (GUILayout.Toggle(toggleState, NormalsEditorButtonContent, toggleButtonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				//SplineEditor.ToggleNormalsEditorMode();
				repaintScene = true;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			previousDrawNormals = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.drawNormals : previousDrawNormals;
			GUILayout.Label(DrawNormalsToggleFieldContent);
			var nextDrawNormals = GUILayout.Toggle(previousDrawNormals, string.Empty);
			if (nextDrawNormals != previousDrawNormals)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Toggle Draw Normals");
				SplineEditor.CurrentSpline.drawNormals = nextDrawNormals;
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}
			previousDrawNormals = nextDrawNormals;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawFlipNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			previousFlipNormals = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.FlipNormals : previousFlipNormals;
			GUILayout.Label(FlipNormalsToggleFieldContent);
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

		private void DrawNormalsGlobalRotationField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			previousNormalsRotation = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.GlobalNormalsRotation : previousNormalsRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorGlobalRotationContent, previousNormalsRotation);
			if (nextNormalsRotation != previousNormalsRotation)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Change Normals Global Rotation");
				SplineEditor.CurrentSpline.GlobalNormalsRotation = nextNormalsRotation;

				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}

			previousNormalsRotation = nextNormalsRotation;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}



	}

}
