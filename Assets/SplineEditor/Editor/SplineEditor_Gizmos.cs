using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Draw Gizmos

		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			if (!editorState.AlwaysDrawSplineOnScene || !editorState.DrawSpline)
			{
				return;
			}

			DrawSpline(curve);
		}

		#endregion

	}

}
