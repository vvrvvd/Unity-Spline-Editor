using UnityEngine;

namespace SplineEditor.MeshGenerator
{

	//[CreateAssetMenu(fileName = "SplineMeshSettings", menuName = "Spline Editor/Mesh Generator/Spline Mesh Settings", order = 1)]
	public class SplineMeshConfiguration : ScriptableObject
	{

		[Header("General")]
		public Material uvMaterial= default;

		[Header("Scene GUI")]
		public Color pointsColor = Color.blue;
		public Color normalsColor = Color.green;
		public float normalVectorLength = 2.5f;

	}

}
