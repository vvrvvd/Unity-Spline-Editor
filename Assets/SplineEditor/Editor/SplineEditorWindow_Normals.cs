// <copyright file="SplineEditorWindow_Normals.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor window to SplineEditor .
	/// Partial class providing Normals Editor Mode options GUI.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private void DrawNormalsEditorOptions()
		{
			var prevColor = GUI.color;
			var prevEnabled = GUI.enabled;

			EditorWindowState.IsNormalsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(EditorWindowState.IsNormalsSectionFolded, NormalsEditorGroupTitle);
			GUI.enabled = IsSplineEditorEnabled;

			if (EditorWindowState.IsNormalsSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawFlipNormalsToggle();
				DrawNormalLocalRotationField();
				DrawNormalsGlobalRotationField();
				DrawRotateNormalsButton();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawRotateNormalsButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			GUI.enabled = IsSplineEditorEnabled;
			var toggleState = IsSplineEditorEnabled && EditorState.IsNormalsEditorMode;
			if (GUILayout.Toggle(toggleState, NormalsEditorButtonContent, toggleButtonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ToggleNormalsEditorMode();
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawFlipNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			EditorWindowState.PreviousFlipNormals = EditorState.CurrentSpline != null ? EditorState.CurrentSpline.FlipNormals : EditorWindowState.PreviousFlipNormals;
			var nextFlipNormals = EditorGUILayout.Toggle(FlipNormalsToggleFieldContent, EditorWindowState.PreviousFlipNormals);
			if (nextFlipNormals != EditorWindowState.PreviousFlipNormals)
			{
				Undo.RecordObject(EditorState.CurrentSpline, "Toggle Flip Normals");
				EditorState.CurrentSpline.FlipNormals = nextFlipNormals;
				repaintScene = true;
			}

			EditorWindowState.PreviousFlipNormals = nextFlipNormals;
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawNormalLocalRotationField()
		{
			var prevEnabled = GUI.enabled;
			var currentSpline = EditorState.CurrentSpline;
			var isNormalsEditorEnabled = currentSpline != null && EditorState.IsAnyPointSelected && EditorState.SelectedPointIndex % 3 == 0;
			var normalIndex = EditorState.SelectedPointIndex / 3;

			GUILayout.BeginHorizontal();
			GUI.enabled = isNormalsEditorEnabled;
			EditorWindowState.PreviousNormalLocalRotation = isNormalsEditorEnabled ? currentSpline.NormalsAngularOffsets[normalIndex] : EditorWindowState.PreviousNormalLocalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorLocalRotationContent, EditorWindowState.PreviousNormalLocalRotation);
			if (nextNormalsRotation != EditorWindowState.PreviousNormalLocalRotation)
			{
				Undo.RecordObject(EditorState.CurrentSpline, "Change Normal Local Rotation");
				currentSpline.SetNormalAngularOffset(normalIndex, nextNormalsRotation);
				repaintScene = true;
			}

			EditorWindowState.PreviousNormalLocalRotation = nextNormalsRotation;
			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawNormalsGlobalRotationField()
		{
			GUILayout.BeginHorizontal();
			EditorWindowState.PreviousNormalsGlobalRotation = EditorState.CurrentSpline != null ? EditorState.CurrentSpline.GlobalNormalsRotation : EditorWindowState.PreviousNormalsGlobalRotation;
			var nextNormalsRotation = EditorGUILayout.FloatField(NormalsEditorGlobalRotationContent, EditorWindowState.PreviousNormalsGlobalRotation);
			if (nextNormalsRotation != EditorWindowState.PreviousNormalsGlobalRotation)
			{
				Undo.RecordObject(EditorState.CurrentSpline, "Change Normals Global Rotation");
				EditorState.CurrentSpline.GlobalNormalsRotation = nextNormalsRotation;
				repaintScene = true;
			}

			EditorWindowState.PreviousNormalsGlobalRotation = nextNormalsRotation;
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}
	}
}
