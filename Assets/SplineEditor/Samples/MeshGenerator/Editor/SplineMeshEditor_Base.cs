using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private static SplineMeshEditorState meshEditorState => SplineMeshEditorState.instance;

		private SplineMesh splineMesh;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
			initializeStyles = true;

			if(meshEditorState.IsAnyDebugModeViewVisible() && !meshEditorState.IsDebugModeView(splineMesh))
			{
				meshEditorState.RestoreSavedDebugMaterial();
			}

		}

	}

}
