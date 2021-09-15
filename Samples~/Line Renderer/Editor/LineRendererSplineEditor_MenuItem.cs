// <copyright file="LineRendererSplineEditor_MenuItem.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Partial class of LineRendererSplinEditor for context menu implementation on LineRendererSpline component.
	/// </summary>
	public partial class LineRendererSplineEditor : UnityEditor.Editor
	{
		[MenuItem("GameObject/Spline Editor/Line Renderer Spline", false, 1)]
		private static void CreateCustomSplineMesh(MenuCommand menuCommand)
		{
			var go = new GameObject("Line Renderer Spline");
			var lineRendererSpline = go.AddComponent<LineRendererSpline>();

			lineRendererSpline.LineRenderer.material = new Material(Shader.Find("Diffuse"));

			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

			// Register the creation in the undo system
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;
		}
	}
}
