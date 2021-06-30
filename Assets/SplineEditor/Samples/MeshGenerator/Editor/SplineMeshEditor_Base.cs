using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMesh splineMesh;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
			initializeStyles = true;

			if(SplineMeshEditorState.instance.IsAnyDebugModeViewVisible() && !SplineMeshEditorState.instance.IsDebugModeView(splineMesh))
			{
				SplineMeshEditorState.instance.RestoreSavedDebugMaterial();
			}

		}

	}

}
