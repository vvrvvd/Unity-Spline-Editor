using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMesh splineMesh;
		private Material savedMaterial;
		private bool isVisualizingUV = false;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
			initializeStyles = true;
		}

		private void OnDisable()
		{
			ToggleUV(false);
		}

	}

}
