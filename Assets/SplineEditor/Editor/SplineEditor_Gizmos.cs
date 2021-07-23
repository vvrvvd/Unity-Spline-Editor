using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			if (!EditorState.AlwaysDrawSplineOnScene || !EditorState.DrawSpline || EditorState.CurrentSpline == curve)
			{
				return;
			}

			DrawSpline(curve);
		}

	}

}
