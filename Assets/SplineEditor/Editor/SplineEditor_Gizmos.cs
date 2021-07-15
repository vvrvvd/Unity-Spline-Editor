using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			if (!editorState.AlwaysDrawSplineOnScene || !editorState.DrawSpline || editorState.CurrentSpline == curve)
			{
				return;
			}

			DrawSpline(curve);
		}

	}

}
