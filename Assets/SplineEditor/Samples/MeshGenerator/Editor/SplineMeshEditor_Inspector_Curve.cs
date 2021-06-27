using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private bool isCurveSectionFolded = true;

		private void DrawWidthCurveOptions()
		{
			var prevEnabled = GUI.enabled;

			isCurveSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCurveSectionFolded, CurveOptionsGroupTitle);
			GUI.enabled = true;
			if (isCurveSectionFolded)
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
			var nextCurveState = EditorGUILayout.CurveField(string.Empty, splineMesh.RightSideCurve, WidthCurveMaxWidth);
			if (nextCurveState != splineMesh.RightSideCurve)
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = nextCurveState;
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
			var nextCurveState = EditorGUILayout.CurveField(string.Empty, splineMesh.LeftSideCurve, WidthCurveMaxWidth);
			if (nextCurveState != splineMesh.LeftSideCurve)
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.LeftSideCurve = nextCurveState;
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
			var nextCurveState = EditorGUILayout.CurveField(string.Empty, splineMesh.RightSideCurve, WidthCurveMaxWidth);
			if (nextCurveState != splineMesh.RightSideCurve)
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.RightSideCurve = nextCurveState;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

	}

}
