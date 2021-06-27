using UnityEngine;

namespace SplineEditor.MeshGenerator
{
	[CreateAssetMenu(fileName = "SplineMeshEditorConfiguration", menuName = "Spline Editor/Mesh Generator Settings", order = 1)]
	public class SplineMeshEditorSettings : ScriptableObject
	{

		private const string AssetName = "SplineMeshEditorConfiguration";

		private static SplineMeshEditorSettings _instance;
		public static SplineMeshEditorSettings instance
		{
			get
			{
				if(_instance==null)
				{
					_instance = Resources.Load<SplineMeshEditorSettings>(AssetName);
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
