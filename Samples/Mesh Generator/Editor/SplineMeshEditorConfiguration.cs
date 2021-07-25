// <copyright file="SplineMeshEditorConfiguration.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor.MeshGenerator
{
	/// <summary>
	/// Scriptable object containing SplineMesh editor configuration.
	/// </summary>
	[CreateAssetMenu(fileName = "SplineMeshEditorConfiguration", menuName = "Spline Editor/Mesh Generator/Mesh Generator Configuration", order = 1)]
	public class SplineMeshEditorConfiguration : ScriptableObject
	{
		private static SplineMeshEditorConfiguration instance;

		[Header("General")]
		[SerializeField]
		private Material uvMaterial = default;
		[SerializeField]
		private GUISkin guiSkin = default;

		[Header("Scene GUI")]
		[SerializeField]
		private Color pointsColor = Color.blue;
		[SerializeField]
		private Color normalsColor = Color.green;
		[SerializeField]
		private float normalVectorLength = 2.5f;

		/// <summary>
		/// Gets the first SplineMesh editor configuration object found in the resources.
		/// Caches the result so it's loaded from resources only once.
		/// </summary>
		public static SplineMeshEditorConfiguration Instance
		{
			get
			{
				if (instance == null)
				{
					var loadedInstances = Resources.LoadAll<SplineMeshEditorConfiguration>(string.Empty);

					if (loadedInstances.Length == 0)
					{
						Debug.LogError("[SplineEditor] There is a missing editor settings scriptable for SplineMesh in Resources. Create new settings through \"Spline Editor / Mesh Generator / Mesh Generator Configuration\" and put them in Resources folder.");
						return null;
					}

					instance = loadedInstances[0];
				}

				return instance;
			}
		}

		/// <summary>
		/// Gets or sets material used for visualizing mesh UV in editor.
		/// Is serialized.
		/// </summary>
		public Material UvMaterial { get => uvMaterial; set => uvMaterial = value; }

		/// <summary>
		/// Gets or sets skin used for custom editor GUI.
		/// Is serialized.
		/// </summary>
		public GUISkin GuiSkin { get => guiSkin; set => guiSkin = value; }

		/// <summary>
		/// Gets or sets color of points drawn in the scene view.
		/// Is serialized.
		/// </summary>
		public Color PointsColor { get => pointsColor; set => pointsColor = value; }

		/// <summary>
		/// Gets or sets color of normal vectors drawn in the scene view.
		/// Is serialized.
		/// </summary>
		public Color NormalsColor { get => normalsColor; set => normalsColor = value; }

		/// <summary>
		/// Gets or sets length of normal vectors length drawn in the scene view.
		/// Is serialized.
		/// </summary>
		public float NormalVectorLength { get => normalVectorLength; set => normalVectorLength = value; }
	}
}