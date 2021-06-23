using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private static float prevPointScale = 1f;

		public override void OnInspectorGUI()
		{
			//Add full editor override, get rid of this invoke
			base.OnInspectorGUI();

			var prevEnabled = GUI.enabled;

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

			GUI.enabled = prevEnabled;

			var uvButtonTitle = splineMesh.IsVisualizingUV ? "Hide UV" : "Show UV" ;
			if (GUILayout.Button(uvButtonTitle))
			{
				splineMesh.ToggleUV();
			}

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

		}

	}

}
