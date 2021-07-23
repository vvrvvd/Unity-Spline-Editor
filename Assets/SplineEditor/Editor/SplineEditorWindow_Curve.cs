// <copyright file="SplineEditorWindow_Curve.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor window to SplineEditor.
	/// Partial class providing Curve options GUI.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private void DrawCurveOptions()
		{
			var prevEnabled = GUI.enabled;
			var isGroupEnabled = IsCurveEditorEnabled;

			EditorWindowState.IsCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(EditorWindowState.IsCurveSectionFolded, BezierGroupTitle);
			GUI.enabled = isGroupEnabled;
			if (EditorWindowState.IsCurveSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawCurveParametersSection();
				GUILayout.Space(10);
				DrawCurveButtonsSection();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawCurveButtonsSection()
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			DrawAddCurveButton();
			DrawRemoveCurveButton();
			DrawSplitCurveButton();
			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		private void DrawAddCurveButton()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= EditorState.CanNewCurveBeAdded;
			if (GUILayout.Button(AddCurveButtonContent, buttonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleAddCurve(EditorWindowState.AddCurveLength);
				repaintScene = true;
			}

			GUI.enabled = prevEnabled;
		}

		private void DrawRemoveCurveButton()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= EditorState.CanSelectedCurveBeRemoved;
			if (GUILayout.Button(RemoveCurveButtonContent, buttonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleRemoveSelectedCurve();
				repaintScene = true;
			}

			GUI.enabled = prevEnabled;
		}

		private void DrawSplitCurveButton()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= EditorState.IsAnyPointSelected;
			if (GUILayout.Button(SplitCurveButtonContent, buttonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleSplitCurve(EditorWindowState.SplitCurveValue);
				repaintScene = true;
			}

			GUI.enabled = prevEnabled;
		}

		private void DrawCurveParametersSection()
		{
			DrawAddCurveLengthField();
			DrawSplitCurveSlider();
		}

		private void DrawAddCurveLengthField()
		{
			GUILayout.BeginHorizontal();
			var prevState = EditorWindowState.AddCurveLength;
			var nextState = EditorGUILayout.FloatField(AddCurveLengthFieldContent, EditorWindowState.AddCurveLength);
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorWindowState, "Change Add Curve Length");
				EditorWindowState.AddCurveLength = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSplitCurveSlider()
		{
			GUILayout.BeginHorizontal();
			var prevState = EditorWindowState.SplitCurveValue;
			var nextState = EditorGUILayout.Slider(SplitPointSliderContent, EditorWindowState.SplitCurveValue, 0.001f, 0.999f);
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorWindowState, "Change Split Curve Point");
				EditorWindowState.SplitCurveValue = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}
	}
}
