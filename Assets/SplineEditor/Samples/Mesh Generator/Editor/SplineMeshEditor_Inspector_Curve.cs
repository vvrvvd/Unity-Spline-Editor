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

				DrawWidthCurvesFields();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawWidthCurvesFields()
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();


			if(!splineMesh.UseAsymetricWidthCurve)
			{
				DrawAsymetricCurveToggle();
			}
			else
			{
				DrawRightSideCurveField();
				GUILayout.Space(10);
				DrawLeftSideCurveField();
			}

			GUILayout.Space(10);
			DrawAsymetrictWidthCurvesToggle();

			GUILayout.EndVertical();
		}

		private void DrawAsymetrictWidthCurvesToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(CurveOptionsAsymetricWidthCurveToggleContent);
			GUILayout.Space(20);
			var toggleState = GUILayout.Toggle(splineMesh.UseAsymetricWidthCurve, string.Empty);
			if (toggleState != splineMesh.UseAsymetricWidthCurve)
			{
				Undo.RecordObject(splineMesh, "Toggle asymetric spline width curve");
				splineMesh.UseAsymetricWidthCurve = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawRightSideCurveField()
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(CurveOptionsRightWidthCurveContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.CurveField(string.Empty, splineMesh.RightSideCurve.animationCurve, WidthCurveMaxWidth);
			if (splineMesh.RightSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = splineMesh.RightSideCurve;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		private void DrawLeftSideCurveField()
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(CurveOptionsLeftWidthCurveContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.CurveField(string.Empty, splineMesh.LeftSideCurve.animationCurve, WidthCurveMaxWidth);
			if (splineMesh.LeftSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.LeftSideCurve = splineMesh.LeftSideCurve;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		private void DrawAsymetricCurveToggle()
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(CurveOptionsWidthCurveContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.CurveField(string.Empty, splineMesh.RightSideCurve.animationCurve, WidthCurveMaxWidth);
			if (splineMesh.RightSideCurve.CheckWasCurveModified())
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = splineMesh.RightSideCurve;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

	}

}
