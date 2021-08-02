// <copyright file="SplineMeshEditor_Base.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// </summary>
	[CustomEditor(typeof(SplineMesh))]
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private SplineMesh splineMesh;

		private static SplineMeshEditorState MeshEditorState => SplineMeshEditorState.instance;

		private static SplineMeshConfiguration MeshEditorConfiguration => SplineMeshConfiguration.Instance;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
			initializeStyles = true;

			if (MeshEditorState.IsAnyDebugModeViewVisible() && !MeshEditorState.IsDebugModeView(splineMesh))
			{
				MeshEditorState.RestoreSavedDebugMaterial();
			}
		}
	}
}
