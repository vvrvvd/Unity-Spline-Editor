using UnityEngine;

namespace SplineEditor.MeshGenerator
{
	[CreateAssetMenu(fileName = "SplineMeshEditorConfiguration", menuName = "Spline Editor/Mesh Generator/Mesh Generator Configuration", order = 1)]
	public class SplineMeshEditorConfiguration : ScriptableObject
	{

		private static SplineMeshEditorConfiguration _instance;
		public static SplineMeshEditorConfiguration instance
		{
			get
			{
				if (_instance == null)
				{
					var loadedInstances = Resources.LoadAll<SplineMeshEditorConfiguration>("");

                    if (loadedInstances.Length == 0) 
					{
						Debug.LogError("[SplineEditor] There is a missing editor settings scriptable for Spline Mesh in Resources. Create new settings through \"Spline Editor / Mesh Generator / Mesh Generator Configuration\" and put them in Resources folder.");
						return null;
					}

					_instance = loadedInstances[0];
				}

				return _instance;
			}
		}

		[Header("General")]
		public Material uvMaterial= default;

		[Header("Scene GUI")]
		public Color pointsColor = Color.blue;
		public Color normalsColor = Color.green;
		public float normalVectorLength = 2.5f;

	}

}
