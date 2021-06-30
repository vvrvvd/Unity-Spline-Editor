using UnityEngine;
using UnityEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	[FilePath("SplineEditor/MeshGenerator/SplineMeshEditorState.conf", FilePathAttribute.Location.ProjectFolder)]
	public class SplineMeshEditorState : ScriptableSingleton<SplineMeshEditorState>
	{

		public bool drawPoints = true;
		public bool drawNormals = false;

		[SerializeField]
		private Material savedDebugViewMeshMaterial;
		[SerializeField]
		private SplineMesh savedDebugViewSplineMesh;

		public bool IsDebugModeView(SplineMesh splineMesh)
		{
			return splineMesh == savedDebugViewSplineMesh;
		}

		public bool IsAnyDebugModeViewVisible()
		{
			return savedDebugViewSplineMesh != null;
		}

		public void SetDebugModeView(SplineMesh splineMesh, bool state)
		{
			if((state && splineMesh == savedDebugViewSplineMesh) || (!state && splineMesh != savedDebugViewSplineMesh))
			{
				return;
			}

			RestoreSavedDebugMaterial();

			if(state)
			{
				SetDebugModeMaterial(splineMesh);
			}

		}

		public void SetDebugModeMaterial(SplineMesh splineMesh)
		{
			var settingsScriptable = SplineMeshEditorConfiguration.instance;

			var prevMaterial = splineMesh.MeshRenderer.sharedMaterial;
			var newMaterial = settingsScriptable.uvMaterial;

			splineMesh.MeshRenderer.sharedMaterial = newMaterial;

			savedDebugViewSplineMesh = splineMesh;
			savedDebugViewMeshMaterial = prevMaterial;
		}

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
