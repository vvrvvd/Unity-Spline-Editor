// <copyright file="SplineMeshEditor_SceneGUI.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing scene GUI implementation.
	/// Draws evenly spaced mesh points and their corresponding normal vectors based on BezierSpline in the scene view.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private void OnSceneGUI()
		{
			if (splineMesh == null || splineMesh.BezierSpline == null)
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
			for (var i = 0; i < splineMesh.Points.Count; i++)
			{
				DrawPoint(i);
			}
		}

		private void DrawPoint(int index)
		{
			var point = splineMesh.transform.TransformPoint(splineMesh.Points[index]);
			var size = HandleUtility.GetHandleSize(point);

			if (MeshEditorState.DrawPoints)
			{
				var handleSize = 0.025f;
				Handles.color = SplineMeshEditorConfiguration.Instance.PointsColor;
				Handles.Button(point, Quaternion.identity, size * handleSize, size * handleSize, Handles.DotHandleCap);
			}

			if (MeshEditorState.DrawNormals)
			{
				DrawNormal(point, index);
			}
		}

		private void DrawNormal(Vector3 point, int index)
		{
			var normal = splineMesh.Normals[index];
			var normalLength = SplineMeshEditorConfiguration.Instance.NormalVectorLength;
			Handles.color = SplineMeshEditorConfiguration.Instance.NormalsColor;
			Handles.DrawLine(point, point + splineMesh.transform.TransformDirection(normal * normalLength));
		}
	}
}
