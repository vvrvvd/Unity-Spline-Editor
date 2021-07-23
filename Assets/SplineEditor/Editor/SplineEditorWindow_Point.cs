// <copyright file="SplineEditorWindow_Point.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor window to SplineEditor.
	/// Partial class providing Points options GUI.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private void DrawPointGroup()
		{
			var prevEnabled = GUI.enabled;
			var prevColor = GUI.color;

			EditorWindowState.IsPointSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(EditorWindowState.IsPointSectionFolded, PointGroupTitle);
			GUI.enabled = EditorState.IsAnyPointSelected;
			if (EditorWindowState.IsPointSectionFolded)
			{
				DrawSelectedSplineInspector();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();

			GUI.color = prevColor;
			GUI.enabled = prevEnabled;
		}

		private void DrawSelectedSplineInspector()
		{
			var isGroupEnabled = GUI.enabled;
			GUILayout.BeginVertical(groupsStyle);
			GUILayout.Space(10);
			EditorGUI.indentLevel++;

			DrawSelectedPointInspector();

			EditorGUI.indentLevel--;
			GUILayout.Space(10);
			GUILayout.EndVertical();
			GUI.enabled = isGroupEnabled;
		}

		private void DrawSelectedPointInspector()
		{
			var prevEnabled = GUI.enabled;

			var currentSpline = EditorState.CurrentSpline;
			var isPointSelected = currentSpline != null && EditorState.IsAnyPointSelected;
			GUI.enabled &= isPointSelected;

			DrawPositionField();
			DrawPointsScaleField();
			GUILayout.Space(10);
			DrawModePopupField();

			GUI.enabled = prevEnabled;
		}

		private void DrawPositionField()
		{
			var currentSpline = EditorState.CurrentSpline;
			var selectedPointIndex = EditorState.SelectedPointIndex;
			var isPointSelected = currentSpline != null && EditorState.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			var pointPosition = isPointSelected ? currentSpline.Points[selectedPointIndex].Position : EditorWindowState.PreviousPointPosition;
			var point = EditorGUILayout.Vector3Field(PointPositionContent, pointPosition);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Move Point");
				currentSpline.SetPoint(selectedPointIndex, point);
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			EditorWindowState.PreviousPointPosition = point;
		}

		private void DrawPointsScaleField()
		{
			var prevEnabled = GUI.enabled;
			var currentSpline = EditorState.CurrentSpline;
			var isScaleFieldActive = GUI.enabled & EditorState.SelectedPointIndex % 3 == 0;
			GUI.enabled = isScaleFieldActive;

			GUILayout.BeginHorizontal();

			var pointIndex = EditorState.SelectedPointIndex / 3;
			var currentPointScale = isScaleFieldActive ? currentSpline.PointsScales[pointIndex] : EditorWindowState.PreviousPointScale;
			EditorGUI.BeginChangeCheck();
			var nextPointScale = EditorGUILayout.Vector3Field(PointScaleContent, currentPointScale);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Scale Point");
				currentSpline.SetPointsScale(pointIndex, nextPointScale);
			}

			EditorWindowState.PreviousPointScale = currentPointScale;

			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawModePopupField()
		{
			var currentSpline = EditorState.CurrentSpline;
			var selectedPointIndex = EditorState.SelectedPointIndex;
			var isPointSelected = currentSpline != null && EditorState.IsAnyPointSelected;

			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			var currentMode = isPointSelected ? currentSpline.GetControlPointMode(selectedPointIndex) : EditorWindowState.PreviousPointMode;
			var mode = (BezierControlPointMode)EditorGUILayout.EnumPopup(PointModeContent, currentMode);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Change Point Mode");
				currentSpline.SetControlPointMode(selectedPointIndex, mode);
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			if (GUILayout.Button(ApplyToAllPoinstButtonContent, buttonStyle, ToolsButtonsHeight))
			{
				Undo.RecordObject(currentSpline, "Change All Points Mode");
				currentSpline.SetAllControlPointsMode(mode);
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();

			EditorWindowState.PreviousPointMode = mode;
		}
	}
}
