using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private bool initializeStyles = true;
		private static float prevPointScale = 1f;

		public override void OnInspectorGUI()
		{
			if (initializeStyles)
			{
				InitializeStyles();
				initializeStyles = false;
			}

			GUILayout.BeginVertical();
			DrawMeshOptions();
			DrawUvOptions();
			DrawGUIOptions();
			GUILayout.EndVertical();

			var prevEnabled = GUI.enabled;

			GUI.enabled = prevEnabled;


		}

	}

}
