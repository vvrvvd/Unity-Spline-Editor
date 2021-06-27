using UnityEngine;
using UnityEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	[FilePath("SplineEditor/MeshGenerator/SplineMeshEditorState.conf", FilePathAttribute.Location.ProjectFolder)]
	public class SplineMeshEditorState : ScriptableSingleton<SplineMeshEditorState>
	{
		public bool drawPoints = true;
		public bool drawNormals = false;
	}

}
