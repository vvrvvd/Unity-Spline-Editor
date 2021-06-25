using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private bool isMeshSectionFolded = true;

		private void DrawMeshOptions()
		{
			var prevEnabled = GUI.enabled;

			isMeshSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isMeshSectionFolded, MeshOptionsGroupTitle);
			GUI.enabled = true;
			if (isUvSectionFolded)
			{
				GUILayout.BeginVertical(groupsStyle);
				GUILayout.Space(10);

				DrawMeshWidthField();
				DrawMeshSpacingField();

				GUILayout.Space(10);
				DrawWidthCurvesFields();

				GUILayout.Space(10);

				DrawUsePointsScaleToggle();
				DrawPointsScaleField();

				GUILayout.Space(10);

				DrawBakeMeshButton();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = prevEnabled;
		}

		private void DrawMeshWidthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(MeshOptionsMeshWidthFieldContent);
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawMeshSpacingField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(MeshOptionsMeshSpacingFieldContent);
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawWidthCurvesFields()
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			DrawMirrorCurvesToggle();

			if(splineMesh.useSymetricWidthCurve)
			{
				DrawMirroredCurveField();
			}
			else
			{
				DrawRightSideCurveField();
				DrawLeftSideCurveField();
			}

			GUILayout.EndVertical();
		}

		private void DrawMirrorCurvesToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(MeshOptionsMirrorCurveToggleContent);
			var toggleState = GUILayout.Toggle(splineMesh.useSymetricWidthCurve, string.Empty);
			if (toggleState != splineMesh.useSymetricWidthCurve)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.useSymetricWidthCurve = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawMirroredCurveField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(MeshOptionsWidthCurveContent);
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawRightSideCurveField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(MeshOptionsRightWidthCurveContent);
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawLeftSideCurveField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(MeshOptionsLeftWidthCurveContent);
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}


		private void DrawUsePointsScaleToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label(UvOptionsMirrorUvToggleContent);
			var toggleState = GUILayout.Toggle(splineMesh.mirrorUV, string.Empty);
			if (toggleState != splineMesh.mirrorUV)
			{
				Undo.RecordObject(splineMesh, "Toggle mirror mesh UV ");
				splineMesh.mirrorUV = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawPointsScaleField()
		{
			var prevEnabled = GUI.enabled;
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var isTargetSplineSelected = BezierSplineEditor.CurrentSpline == splineMesh.BezierSpline;
			var isMainPointSelected = BezierSplineEditor.IsAnyPointSelected && BezierSplineEditor.SelectedPointIndex % 3 == 0;
			var isScaleFieldActive = isTargetSplineSelected && isMainPointSelected;

			GUI.enabled = isScaleFieldActive;

			var pointIndex = BezierSplineEditor.SelectedPointIndex / 3;
			var currentPointScale = isScaleFieldActive ? splineMesh.BezierSpline.PointsScales[pointIndex] : prevPointScale;

			//TODO: Add styles and refactor with functions
			var nextPointScale = EditorGUILayout.FloatField("Point Scale", currentPointScale);
			if (nextPointScale != currentPointScale)
			{
				splineMesh.BezierSpline.UpdatePointsScale(pointIndex, nextPointScale);
			}
			prevPointScale = currentPointScale;

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawBakeMeshButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Save Mesh"))
			{
				splineMesh.ConstructMesh();
				var path = EditorUtility.SaveFilePanel("Save Spline Mesh Asset", "Assets/", "savedMesh", "asset");
				if (string.IsNullOrEmpty(path)) return;

				path = FileUtil.GetProjectRelativePath(path);
				var mesh = splineMesh.MeshFilter.sharedMesh;
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.SaveAssets();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}


	}

}
