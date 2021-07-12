using UnityEditor;
using UnityEngine;

namespace SplineEditor
{

	public class LineRendererSpline_MenuItem : MonoBehaviour
	{
		[MenuItem("GameObject/Spline Editor/Line Renderer Spline", false, 1)]
		static void CreateCustomSplineMesh(MenuCommand menuCommand) {

			var go = new GameObject("Line Renderer Spline");
			var lineRendererSpline = go.AddComponent<LineRendererSpline>();

			lineRendererSpline.LineRenderer.material = new Material(Shader.Find("Diffuse"));

			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			// Register the creation in the undo system
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;

		}
	}

}
