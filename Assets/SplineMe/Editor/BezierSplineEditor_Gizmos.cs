using UnityEditor;

namespace SplineMe.Editor
{
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Draw Gizmos

		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			if (CurrentSpline == curve)
			{
				return;
			}

			DrawSpline(curve);
		}

		#endregion

	}

}
