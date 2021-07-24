// <copyright file="SplineEditor_MenuItem.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// Partial class providing context menu items implementation.
	/// </summary>
	public partial class SplineEditor : UnityEditor.Editor
	{
		[MenuItem("GameObject/Spline Editor/Bezier Spline", false, 1)]
		private static void CreateCustomBezierSpline(MenuCommand menuCommand)
		{
			var go = new GameObject("Bezier Spline");
			go.AddComponent<BezierSpline>();

			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

			// Register the creation in the undo system
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;
		}
	}
}