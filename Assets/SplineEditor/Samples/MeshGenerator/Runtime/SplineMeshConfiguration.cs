using UnityEngine;

namespace SplineEditor.MeshGenerator
{

	//[CreateAssetMenu(fileName = "SplineMeshSettings", menuName = "Spline Editor/Mesh Generator/Spline Mesh Settings", order = 1)]
	public class SplineMeshConfiguration : ScriptableObject
	{

		[Header("General")]
		public Material uvMaterial= default;

	}

}
