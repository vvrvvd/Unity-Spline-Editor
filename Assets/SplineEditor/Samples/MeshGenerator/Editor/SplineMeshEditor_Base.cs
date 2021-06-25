using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMesh splineMesh;
		private SplineMeshConfiguration cachedSplineMeshConfiguration;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
			cachedSplineMeshConfiguration = Resources.Load<SplineMeshConfiguration>(SplineMesh.SplineMeshSettingsName);

			if (cachedSplineMeshConfiguration == null)
			{
				Debug.LogError("[SplineMeshEditor] There is missing SplineMeshConfiguration file in Resources folder!");
			}

			initializeStyles = true;
		}


	}

}
