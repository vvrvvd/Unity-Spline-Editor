// <copyright file="SplineMeshEditor_MenuItem.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing context menu items.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		[MenuItem("GameObject/Spline Editor/Bezier Spline Mesh", false, 1)]
		private static void CreateCustomSplineMesh(MenuCommand menuCommand)
		{
			var go = new GameObject("Bezier Spline Mesh");
			var splineMesh = go.AddComponent<SplineMesh>();

			splineMesh.MeshRenderer.material = new Material(Shader.Find("Diffuse"));

			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

			// Register the creation in the undo system
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;
		}
	}
}
