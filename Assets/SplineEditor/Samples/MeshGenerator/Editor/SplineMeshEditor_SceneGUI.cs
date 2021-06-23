using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMeshConfiguration cachedSplineMeshConfiguration;

		private void OnSceneGUI()
		{

			if (splineMesh.BezierSpline == null || splineMesh.splinePath == null)
			{
				return;
			}

			if(cachedSplineMeshConfiguration==null)
			{
				cachedSplineMeshConfiguration = Resources.Load<SplineMeshConfiguration>(SplineMesh.SplineMeshSettingsName);
			}

			if(cachedSplineMeshConfiguration==null)
			{
				Debug.LogError("[SplineMeshEditor] There is missing SplineMeshConfiguration file in Resources folder!");
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
			for (var i = 0; i < splineMesh.splinePath.points.Length; i++)
			{
				DrawPoint(i);
			}
		}

		private void DrawPoint(int index)
		{
			var point = splineMesh.transform.TransformPoint(splineMesh.splinePath.points[index]);
			var size = HandleUtility.GetHandleSize(point);

			if (splineMesh.drawPoints)
			{
				var handleSize = 0.025f;
				Handles.color = cachedSplineMeshConfiguration.pointsColor;
				Handles.Button(point, Quaternion.identity, size * handleSize, size * handleSize, Handles.DotHandleCap);
			}

			if (splineMesh.drawNormals)
			{
				DrawNormal(point, index);
			}
		}

		private void DrawNormal(Vector3 point, int index)
		{
			var normal = splineMesh.Normals[index];
			var normalLength = cachedSplineMeshConfiguration.normalVectorLength;
			Handles.color = cachedSplineMeshConfiguration.normalsColor;
			Handles.DrawLine(point, point + splineMesh.transform.TransformDirection(normal * normalLength));
		}
	}

}
