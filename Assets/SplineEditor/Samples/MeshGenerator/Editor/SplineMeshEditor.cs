using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMesh splineMesh;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Generate Mesh"))
			{
				splineMesh.ConstructMesh();
			}
		}

		private void OnSceneGUI()
		{
			if (splineMesh.BezierSpline == null)
			{
				return;
			}

			var segmentPoints = splineMesh.SplinePath.points;
			var normals = splineMesh.Normals;

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;
			Handles.color = Color.blue;
			for (var i = 0; i < segmentPoints.Length; i++)
			{
				var point = splineMesh.transform.TransformPoint(segmentPoints[i]);
				var size = HandleUtility.GetHandleSize(point);
				var normal = normals[i];
				var normalLength = 5f;
				var handleSize = 0.05f;
				Handles.color = Color.blue;
				Handles.Button(point, Quaternion.identity, size * handleSize, size * handleSize, Handles.DotHandleCap);
				Handles.color = Color.green;
				Handles.DrawLine(point, point + normal * normalLength);
			}
			GUI.enabled = prevEnabled;
		}
	}

}
