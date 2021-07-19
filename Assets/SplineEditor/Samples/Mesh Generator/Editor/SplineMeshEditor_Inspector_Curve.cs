using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private void DrawWidthCurveOptions()
		{
			var prevEnabled = GUI.enabled;

			meshEditorState.IsCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(meshEditorState.IsCurveSectionFolded, CurveOptionsGroupTitle);
			GUI.enabled = true;
			if (meshEditorState.IsCurveSectionFolded)
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
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.EndHorizontal();
		}

		private void DrawRightSideCurveField()
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.CurveField(CurveOptionsRightWidthCurveContent, splineMesh.RightSideCurve.animationCurve);
			if (splineMesh.RightSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = splineMesh.RightSideCurve;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.EndHorizontal();
		}

		private void DrawLeftSideCurveField()
		{
			GUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.CurveField(CurveOptionsLeftWidthCurveContent, splineMesh.LeftSideCurve.animationCurve);
			if (splineMesh.LeftSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.LeftSideCurve = splineMesh.LeftSideCurve;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.EndHorizontal();
		}

		private void DrawAsymetricCurveToggle()
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.CurveField(CurveOptionsWidthCurveContent, splineMesh.RightSideCurve.animationCurve);
			if (splineMesh.RightSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = splineMesh.RightSideCurve;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.EndHorizontal();
		}

	}

}
