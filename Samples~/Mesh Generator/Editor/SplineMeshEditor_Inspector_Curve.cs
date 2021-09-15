// <copyright file="SplineMeshEditor_Inspector_Curve.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing Curve options for custom inspector to SplineMesh component.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private void DrawWidthCurveOptions()
		{
			var prevEnabled = GUI.enabled;

			MeshEditorState.IsCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(MeshEditorState.IsCurveSectionFolded, CurveOptionsGroupTitle);
			GUI.enabled = true;
			if (MeshEditorState.IsCurveSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);
				EditorGUI.indentLevel++;

				DrawWidthCurvesFields();

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawWidthCurvesFields()
		{
			GUILayout.BeginVertical();

			DrawAsymetrictWidthCurvesToggle();

			if (!splineMesh.UseAsymetricWidthCurve)
			{
				DrawAsymetricCurveToggle();
			}
			else
			{
				DrawRightSideCurveField();
				DrawLeftSideCurveField();
			}

			GUILayout.EndVertical();
		}

		private void DrawAsymetrictWidthCurvesToggle()
		{
			GUILayout.BeginHorizontal();

			var toggleState = EditorGUILayout.Toggle(CurveOptionsAsymetricWidthCurveToggleContent, splineMesh.UseAsymetricWidthCurve);
			if (toggleState != splineMesh.UseAsymetricWidthCurve)
			{
				Undo.RecordObject(splineMesh, "Toggle asymetric spline width curve");
				splineMesh.UseAsymetricWidthCurve = toggleState;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawRightSideCurveField()
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.CurveField(CurveOptionsRightWidthCurveContent, splineMesh.RightSideCurve.AnimationCurve);
			if (splineMesh.RightSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = splineMesh.RightSideCurve;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawLeftSideCurveField()
		{
			GUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.CurveField(CurveOptionsLeftWidthCurveContent, splineMesh.LeftSideCurve.AnimationCurve);
			if (splineMesh.LeftSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.LeftSideCurve = splineMesh.LeftSideCurve;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawAsymetricCurveToggle()
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.CurveField(CurveOptionsWidthCurveContent, splineMesh.RightSideCurve.AnimationCurve);
			if (splineMesh.RightSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = splineMesh.RightSideCurve;
			}

			GUILayout.EndHorizontal();
		}
	}
}
