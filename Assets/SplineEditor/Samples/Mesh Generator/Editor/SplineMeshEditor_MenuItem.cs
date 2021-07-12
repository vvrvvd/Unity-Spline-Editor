using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

        [MenuItem("GameObject/Spline Editor/Bezier Spline Mesh", false, 1)]
        static void CreateCustomSplineMesh(MenuCommand menuCommand) {

            var go = new GameObject("Bezier Spline Mesh");
            var splineMesh = go.AddComponent<SplineMesh>();

            splineMesh.MeshRenderer.material = new Material(Shader.Find("Diffuse"));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

        }

    }

}
