using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public class SplineMeshEditor : UnityEditor.Editor
	{

		public const string SplineMeshEditorSettingsName = "SplineMeshEditorSettings";

		private SplineMesh splineMesh;
		private static float prevPointScale = 1f;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
		}

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

			if (GUILayout.Button("Generate Mesh"))
			{
				splineMesh.ConstructMesh();
			}

		}

		private void OnSceneGUI()
		{

			if (splineMesh.BezierSpline == null || splineMesh.splinePath == null)
			{
				return;
			}

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;

			//TODO: Add color to settings
			Handles.color = Color.blue;
			for (var i = 0; i < splineMesh.splinePath.points.Length; i++)
			{
				DrawPoint(i);
			}

			GUI.enabled = prevEnabled;

		}

		private void DrawPoint(int index)
		{
			var point = splineMesh.transform.TransformPoint(splineMesh.splinePath.points[index]);
			var size = HandleUtility.GetHandleSize(point);

			if (splineMesh.drawPoints)
			{
				var handleSize = 0.025f;
				Handles.color = Color.blue;
				Handles.Button(point, Quaternion.identity, size * handleSize, size * handleSize, Handles.DotHandleCap);
			}

			if (splineMesh.drawNormals)
			{
				var normal = splineMesh.Normals[index];
				var normalLength = 2.5f;
				Handles.color = Color.green;
				Handles.DrawLine(point, point + splineMesh.transform.TransformDirection(normal * normalLength));
			}
		}
	}

}
