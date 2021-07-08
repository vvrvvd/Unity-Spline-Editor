using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private void OnSceneGUI()
		{

			if (splineMesh.BezierSpline == null)
			{
				return;
			}

			var prevColor = Handles.color;
			var prevEnabled = GUI.enabled;
			GUI.enabled = false;
			DrawPoints();
			GUI.enabled = prevEnabled;
			Handles.color = prevColor;
		}

		private void DrawPoints()
		{
			for (var i = 0; i < splineMesh.Points.Length; i++)
			{
				DrawPoint(i);
			}
		}

		private void DrawPoint(int index)
		{
			var point = splineMesh.transform.TransformPoint(splineMesh.Points[index]);
			var size = HandleUtility.GetHandleSize(point);

			if (meshEditorState.DrawPoints)
			{
				var handleSize = 0.025f;
				Handles.color = SplineMeshEditorConfiguration.instance.pointsColor;
				Handles.Button(point, Quaternion.identity, size * handleSize, size * handleSize, Handles.DotHandleCap);
			}

			if (meshEditorState.DrawNormals)
			{
				DrawNormal(point, index);
			}
		}

		private void DrawNormal(Vector3 point, int index)
		{
			var normal = splineMesh.Normals[index];
			var normalLength = SplineMeshEditorConfiguration.instance.normalVectorLength;
			Handles.color = SplineMeshEditorConfiguration.instance.normalsColor;
			Handles.DrawLine(point, point + splineMesh.transform.TransformDirection(normal * normalLength));
		}
	}

}
