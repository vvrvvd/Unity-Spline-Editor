// <copyright file="SplineMeshEditorState.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing serializable state for custom SplineMesh editor.
	/// </summary>
	[FilePath("UserSettings/SplineEditor/MeshGenerator/SplineMeshEditorState.asset", FilePathAttribute.Location.ProjectFolder)]
	public class SplineMeshEditorState : ScriptableSingleton<SplineMeshEditorState>
	{
		private Material savedDebugViewMeshMaterial;
		private SplineMesh savedDebugViewSplineMesh;

		[SerializeField]
		private bool drawPoints = true;
		[SerializeField]
		private bool drawNormals = false;
		[SerializeField]
		private bool isUvSectionFolded = true;
		[SerializeField]
		private bool isCurveSectionFolded = true;
		[SerializeField]
		private bool isMeshSectionFolded = true;

		/// <summary>
		/// Gets or sets a value indicating whether points are drawn on scene GUI for SplineMesh components.
		/// Is serialized.
		/// </summary>
		public bool DrawPoints
		{
			get => drawPoints;
			set
			{
				if (drawPoints == value)
				{
					return;
				}

				drawPoints = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether normal vectors are drawn on scene GUI for SplineMesh components.
		/// Is serialized.
		/// </summary>
		public bool DrawNormals
		{
			get => drawNormals;
			set
			{
				if (drawNormals == value)
				{
					return;
				}

				drawNormals = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether UV options sections are folded on SplineMesh components.
		/// Is serialized.
		/// </summary>
		public bool IsUvSectionFolded
		{
			get => isUvSectionFolded;
			set
			{
				if (isUvSectionFolded == value)
				{
					return;
				}

				isUvSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether curve options sections are folded on SplineMesh components.
		/// Is serialized.
		/// </summary>
		public bool IsCurveSectionFolded
		{
			get => isCurveSectionFolded;
			set
			{
				if (IsCurveSectionFolded == value)
				{
					return;
				}

				isCurveSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether mesh options sections are folded on SplineMesh components.
		/// Is serialized.
		/// </summary>
		public bool IsMeshSectionFolded
		{
			get => isMeshSectionFolded;
			set
			{
				if (IsMeshSectionFolded == value)
				{
					return;
				}

				isMeshSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Returns whether debug view displaying UV on given SplineMesh component is enabled.
		/// </summary>
		/// <param name="splineMesh">SplineMesh component to have the debug mode checked.</param>
		/// <returns>True if debug view is enabled for given SplineMesh component and false otherwise.</returns>
		public bool IsDebugModeView(SplineMesh splineMesh)
		{
			return splineMesh == savedDebugViewSplineMesh;
		}

		/// <summary>
		/// Returns whether any debug view displaying UV on spline mesh is enabled.
		/// </summary>
		/// <returns>True if debug view is enabled for any SplineMesh component and false otherwise.</returns>
		public bool IsAnyDebugModeViewVisible()
		{
			return savedDebugViewSplineMesh != null;
		}

		/// <summary>
		/// Sets debug mode view displaying UV state for given SplineMesh component.
		/// </summary>
		/// <param name="splineMesh">SplineMesh component to set debug mode view.</param>
		/// <param name="state">State of debug mode to be set.</param>
		public void SetDebugModeView(SplineMesh splineMesh, bool state)
		{
			if ((state && splineMesh == savedDebugViewSplineMesh) || (!state && splineMesh != savedDebugViewSplineMesh))
			{
				return;
			}

			RestoreSavedDebugMaterial();

			if (state)
			{
				SetDebugModeMaterial(splineMesh);
			}
		}

		/// <summary>
		/// Sets debug mode displaying UV material to given SplineMesh component.
		/// </summary>
		/// <param name="splineMesh">SplineMesh component to have UV material set.</param>
		public void SetDebugModeMaterial(SplineMesh splineMesh)
		{
			var settingsScriptable = SplineMeshEditorConfiguration.Instance;

			var prevMaterial = splineMesh.MeshRenderer.sharedMaterial;
			var newMaterial = settingsScriptable.UvMaterial;

			splineMesh.MeshRenderer.sharedMaterial = newMaterial;

			savedDebugViewSplineMesh = splineMesh;
			savedDebugViewMeshMaterial = prevMaterial;
		}

		/// <summary>
		/// Restores previous material on SplineMesh component having debug mode enabled.
		/// </summary>
		public void RestoreSavedDebugMaterial()
		{
			if (savedDebugViewSplineMesh == null)
			{
				return;
			}

			savedDebugViewSplineMesh.MeshRenderer.sharedMaterial = savedDebugViewMeshMaterial;

			savedDebugViewSplineMesh = null;
			savedDebugViewMeshMaterial = null;
		}
	}
}
