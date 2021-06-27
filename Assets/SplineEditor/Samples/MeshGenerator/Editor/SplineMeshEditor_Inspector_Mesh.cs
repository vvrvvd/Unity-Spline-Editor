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

				DrawWidthCurvesFields();

				GUILayout.Space(10);
				DrawUsePointsScaleToggle();
				DrawMeshWidthField();
				DrawMeshSpacingField();

				GUILayout.Space(10);

				DrawBakeMeshButton();

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


			if(!splineMesh.useAsymetricWidthCurve)
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

			GUILayout.Label(MeshOptionsAsymetricWidthCurveToggleContent);
			GUILayout.Space(20);
			var toggleState = GUILayout.Toggle(splineMesh.useAsymetricWidthCurve, string.Empty);
			if (toggleState != splineMesh.useAsymetricWidthCurve)
			{
				Undo.RecordObject(splineMesh, "Toggle asymetric spline width curve");
				splineMesh.useAsymetricWidthCurve = toggleState;
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
			GUILayout.Label(MeshOptionsRightWidthCurveContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var nextCurveState = EditorGUILayout.CurveField(string.Empty, splineMesh.rightSideCurve, WidthCurveMaxWidth);
			if (nextCurveState != splineMesh.rightSideCurve)
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.rightSideCurve = nextCurveState;
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
			GUILayout.Label(MeshOptionsLeftWidthCurveContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var nextCurveState = EditorGUILayout.CurveField(string.Empty, splineMesh.leftSideCurve, WidthCurveMaxWidth);
			if (nextCurveState != splineMesh.leftSideCurve)
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.leftSideCurve = nextCurveState;
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
			GUILayout.Label(MeshOptionsWidthCurveContent);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var nextCurveState = EditorGUILayout.CurveField(string.Empty, splineMesh.rightSideCurve, WidthCurveMaxWidth);
			if (nextCurveState != splineMesh.rightSideCurve)
			{
				Undo.RecordObject(splineMesh, "Change mirrored width curve");
				splineMesh.rightSideCurve = nextCurveState;
				EditorUtility.SetDirty(splineMesh);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		private void DrawUsePointsScaleToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var toggleState = EditorGUILayout.Toggle(MeshOptionsUsePointsScaleToggleContent, splineMesh.usePointsScale);
			if (toggleState != splineMesh.usePointsScale)
			{
				Undo.RecordObject(splineMesh, "Toggle use points scale");
				splineMesh.usePointsScale = toggleState;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawMeshSpacingField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var nextMeshSpacing = EditorGUILayout.FloatField(MeshOptionsMeshSpacingFieldContent, splineMesh.spacing);
			if (nextMeshSpacing != splineMesh.spacing)
			{
				Undo.RecordObject(splineMesh, "Change spline mesh spacing");
				splineMesh.spacing = nextMeshSpacing;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawMeshWidthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var nextMeshWidth = EditorGUILayout.FloatField(MeshOptionsMeshWidthFieldContent, splineMesh.width);
			if (nextMeshWidth != splineMesh.width)
			{
				Undo.RecordObject(splineMesh, "Change spline mesh width");
				splineMesh.width = nextMeshWidth;
				EditorUtility.SetDirty(splineMesh);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawBakeMeshButton()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(MeshOptionsBakeMeshButtonContent, ButtonMaxWidth, ButtonHeight))
			{
				splineMesh.ConstructMesh();
				var path = EditorUtility.SaveFilePanel(MeshOptionsBakeMeshWindowTitle, MeshOptionsBakeMeshWindowFolderPath, MeshOptionsBakeMeshWindowFileName, MeshOptionsBakeMeshWindowFileExtension);
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
