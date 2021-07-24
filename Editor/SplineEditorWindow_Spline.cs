// <copyright file="SplineEditorWindow_Spline.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor window to SplineEditor.
	/// Partial class providing Spline options GUI.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private void DrawSplineGroup()
		{
			var prevEnabled = GUI.enabled;
			var prevColor = GUI.color;

			EditorWindowState.IsSplineSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(EditorWindowState.IsSplineSectionFolded, SplineOptionsTitle);
			GUI.enabled = IsSplineEditorEnabled;
			EditorGUI.indentLevel++;

			if (EditorWindowState.IsSplineSectionFolded)
			{
				DrawSplineStatsSection();
				DrawSplineButtons();
				DrawCastButtons();
			}

			EditorGUI.indentLevel--;
			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.color = prevColor;
			GUI.enabled = prevEnabled;
		}

		private void DrawSplineStatsSection()
		{
			GUILayout.BeginHorizontal(groupsStyle);
			GUILayout.BeginVertical();

			GUILayout.Space(5);

			DrawSplineTogglesInspector();

			GUILayout.Space(5);

			DrawLengthField();
			GUILayout.Space(5);

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		private void DrawSplineTogglesInspector()
		{
			DrawDrawPointsToggle();
			DrawDrawNormalsToggle();
			DrawDrawSplineToggle();
			DrawAlwaysOnSceneToggle();
			DrawShowMainTransformHandleToggle();
		}

		private void DrawDrawPointsToggle()
		{
			GUILayout.BeginHorizontal();
			var previousDrawPoints = EditorState.DrawPoints;
			var nextLoopState = EditorGUILayout.Toggle(DrawPointsFieldContent, previousDrawPoints);
			if (nextLoopState != previousDrawPoints)
			{
				Undo.RecordObject(EditorState, "Toggle Draw Points");
				EditorState.DrawPoints = nextLoopState;
				repaintScene = true;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawDrawSplineToggle()
		{
			GUILayout.BeginHorizontal();
			var previousDrawSpline = EditorState.DrawSpline;
			var nextLoopState = EditorGUILayout.Toggle(DrawSplineFieldContent, previousDrawSpline);
			if (nextLoopState != previousDrawSpline)
			{
				Undo.RecordObject(EditorState, "Toggle Draw Spline");
				EditorState.DrawSpline = nextLoopState;
				repaintScene = true;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			var previousDrawNormals = EditorState.DrawNormals;
			var nextDrawNormals = EditorGUILayout.Toggle(DrawNormalsToggleFieldContent, previousDrawNormals);
			if (nextDrawNormals != previousDrawNormals)
			{
				Undo.RecordObject(EditorState, "Toggle Draw Normals");
				EditorState.DrawNormals = nextDrawNormals;
				repaintScene = true;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawShowMainTransformHandleToggle()
		{
			GUILayout.BeginHorizontal();
			var previousShowTransformHandle = EditorState.ShowTransformHandle;
			var nextLoopState = EditorGUILayout.Toggle(ShowTransformHandleFieldContent, previousShowTransformHandle);
			if (nextLoopState != previousShowTransformHandle)
			{
				Undo.RecordObject(EditorState, "Toggle Show Transform Handle");
				EditorState.ShowTransformHandle = nextLoopState;
				if (!nextLoopState)
				{
					SplineEditor.HideTools();
				}
				else
				{
					SplineEditor.ShowTools();
				}

				repaintScene = true;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawAlwaysOnSceneToggle()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= EditorState.DrawSpline;

			GUILayout.BeginHorizontal();
			var previousAlwaysDrawOnScene = EditorState.AlwaysDrawSplineOnScene;
			var nextLoopState = EditorGUILayout.Toggle(AlwaysDrawOnSceneFieldContent, previousAlwaysDrawOnScene);
			if (nextLoopState != previousAlwaysDrawOnScene)
			{
				Undo.RecordObject(EditorState, "Toggle Always Draw On Scene");
				EditorState.AlwaysDrawSplineOnScene = nextLoopState;
				repaintScene = true;
			}

			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawLengthField()
		{
			GUILayout.BeginHorizontal();

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;
			var currentLength = EditorState.CurrentSpline != null ? EditorState.CurrentSpline.GetLinearLength(useWorldScale: true) : EditorWindowState.PreviousSplineLength;
			EditorGUILayout.FloatField(LengthSplineFieldContent, currentLength);

			EditorWindowState.PreviousSplineLength = currentLength;
			GUI.enabled = prevEnabled;

			GUILayout.EndHorizontal();
		}

		private void DrawSplineButtons()
		{
			var isGroupEnabled = GUI.enabled;
			GUILayout.BeginVertical(groupsStyle);
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			GUI.enabled = isGroupEnabled;

			if (GUILayout.Button(CloseLoopButtonContent, buttonStyle, ToolsButtonsMinWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleToggleCloseLoop();
				repaintScene = true;
			}

			if (GUILayout.Button(FactorSplineButtonContent, buttonStyle, ToolsButtonsMinWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleFactorSpline();
				repaintScene = true;
			}

			GUI.enabled = isGroupEnabled && EditorState.CanSplineBeSimplified;
			if (GUILayout.Button(SimplifyButtonContent, buttonStyle, ToolsButtonsMinWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleSimplifySpline();
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.EndVertical();
			GUI.enabled = isGroupEnabled;
		}

		private void DrawCastButtons()
		{
			GUILayout.BeginVertical(groupsStyle);

			GUILayout.Space(10);

			DrawCustomTransformField();

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			if (GUILayout.Button(CastSplineContent, buttonStyle, ToolsButtonsHeight))
			{
				var referenceTransform = EditorWindowState.CustomTransform == null ? EditorState.CurrentSpline.transform : EditorWindowState.CustomTransform;
				var castDirection = -referenceTransform.up;
				SplineEditor.ScheduleCastSpline(castDirection);
				repaintScene = true;
			}

			if (GUILayout.Button(CastSplineToCameraContent, buttonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleCastSplineToCameraView();
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			GUILayout.EndVertical();
		}

		private void DrawCustomTransformField()
		{
			GUILayout.BeginHorizontal();

			var prevState = EditorWindowState.CustomTransform;
			var nextState = EditorGUILayout.ObjectField(CastTransformFieldContent, EditorWindowState.CustomTransform, typeof(Transform), true) as Transform;
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorWindowState, "Change Custom Cast Transform");
				EditorWindowState.CustomTransform = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}
	}
}
