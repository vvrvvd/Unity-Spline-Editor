using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

        [MenuItem("GameObject/Spline Editor/Bezier Spline", false, 1)]
        static void CreateCustomBezierSpline(MenuCommand menuCommand) {
            
            var go = new GameObject("Bezier Spline");
            go.AddComponent<BezierSpline>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;

        }

    }

}