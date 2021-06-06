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

			if (splineMesh.BezierSpline == null || splineMesh.splinePath == null)
			{
				return;
			}

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;

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
