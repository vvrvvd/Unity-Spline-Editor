using UnityEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	[CustomEditor(typeof(SplineMesh))]
	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private SplineMesh splineMesh;

		private void OnEnable()
		{
			splineMesh = target as SplineMesh;
		}

		
	}

}
