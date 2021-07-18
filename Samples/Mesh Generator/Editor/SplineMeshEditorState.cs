using UnityEngine;
using UnityEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	[FilePath("SplineEditor/MeshGenerator/SplineMeshEditorState.conf", FilePathAttribute.Location.ProjectFolder)]
	public class SplineMeshEditorState : ScriptableSingleton<SplineMeshEditorState>
	{

		private Material savedDebugViewMeshMaterial;
		private SplineMesh savedDebugViewSplineMesh;

		[SerializeField]
		private bool drawPoints = true;
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

		[SerializeField]
		private bool drawNormals = false;
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

		[SerializeField]
		private bool isUvSectionFolded = true;
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

		[SerializeField]
		private bool isCurveSectionFolded = true;
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

		[SerializeField]
		private bool isMeshSectionFolded = true;
		public bool IsMeshSectionFolded
		{
			get => isMeshSectionFolded; 
			set
			{
				if(IsMeshSectionFolded == value)
				{
					return;
				}

				isMeshSectionFolded = value;
				Save(true);
			}
		}

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
