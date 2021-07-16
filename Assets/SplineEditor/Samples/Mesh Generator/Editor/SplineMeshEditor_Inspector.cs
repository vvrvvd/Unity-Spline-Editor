using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private bool initializeStyles = true;

		public override void OnInspectorGUI()
		{
			if (initializeStyles)
			{
				InitializeStyles();
				initializeStyles = false;
			}

			GUILayout.BeginVertical();
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((SplineMesh)target), typeof(SplineMesh), false);
			GUI.enabled = true;

			DrawMeshOptions();
			DrawWidthCurveOptions();
			DrawUvOptions();
			DrawGUIOptions();
			GUILayout.EndVertical();

			var prevEnabled = GUI.enabled;

			GUI.enabled = prevEnabled;


		}

	}

}
