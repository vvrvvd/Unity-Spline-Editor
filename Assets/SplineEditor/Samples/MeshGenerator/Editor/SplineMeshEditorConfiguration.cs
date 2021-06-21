using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	//[CreateAssetMenu(fileName = "SplineMeshEditorSettings", menuName = "Spline Editor/Mesh Generator/Spline Mesh Settings", order = 1)]
	public class SplineMeshEditorConfiguration : ScriptableObject
	{

		[Header("General")]
		public Shader uvShader = default;

	}

}
