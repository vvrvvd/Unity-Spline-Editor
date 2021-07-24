// <copyright file="SplineEditorWindow_Drawer.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor window to SplineEditor.
	/// Partial class providing Draw Curve Mode options GUI.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private void DrawDrawerToolOptions()
		{
			var prevColor = GUI.color;
			var prevEnabled = GUI.enabled;

			EditorWindowState.IsDrawerSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(EditorWindowState.IsDrawerSectionFolded, DrawerGroupTitle);
			GUI.enabled = IsSplineEditorEnabled;

			if (EditorWindowState.IsDrawerSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawSmoothAnglesToggle();
				DrawSegmentLengthField();
				DrawFirstPointHookSlider();
				DrawSecondPointHookSlider();
				DrawDrawerToolButton();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawDrawerToolButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			GUI.enabled = IsSplineEditorEnabled && !EditorState.CurrentSpline.IsLoop;
			var toggleState = IsSplineEditorEnabled && EditorState.IsDrawerMode;
			if (GUILayout.Toggle(toggleState, DrawCurveButtonContent, toggleButtonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ToggleDrawSplineMode();
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSmoothAnglesToggle()
		{
			GUILayout.BeginHorizontal();
			var prevState = EditorState.DrawCurveSmoothAcuteAngles;
			var nextState = EditorGUILayout.Toggle(DrawCurveSmoothAnglesContent, prevState);
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorState, "Toggle Draw Smooth Angles");
				EditorState.DrawCurveSmoothAcuteAngles = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSecondPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			var prevState = EditorState.DrawCurveSecondPointHook;
			var nextState = EditorGUILayout.Slider(DrawCurveSecondHookContent, EditorState.DrawCurveSecondPointHook, EditorState.DrawCurveFirstPointHook, 0.999f);
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorState, "Change Second Point Hook");
				EditorState.DrawCurveSecondPointHook = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawFirstPointHookSlider()
		{
			GUILayout.BeginHorizontal();
			var prevState = EditorState.DrawCurveFirstPointHook;
			var nextState = EditorGUILayout.Slider(DrawCurveFirstHookContent, EditorState.DrawCurveFirstPointHook, 0.001f, EditorState.DrawCurveSecondPointHook);
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorState, "Change First Point Hook");
				EditorState.DrawCurveFirstPointHook = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}

		private void DrawSegmentLengthField()
		{
			GUILayout.BeginHorizontal();
			var prevState = EditorState.DrawCurveSegmentLength;
			var nextState = EditorGUILayout.FloatField(DrawCurveSegmentLengthContent, EditorState.DrawCurveSegmentLength);
			if (nextState != prevState)
			{
				Undo.RecordObject(EditorState, "Change Draw Curve Segment Length");
				EditorState.DrawCurveSegmentLength = nextState;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
		}
	}
}
