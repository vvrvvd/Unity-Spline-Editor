using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Draw Gizmos

		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void RenderCustomGizmo(BezierSpline curve, GizmoType gizmoType)
		{
			if (CurrentSpline == curve || !curve.AlwaysDrawSplineOnScene)
			{
				return;
			}

			TryLoadEditorSettings();
			DrawSpline(curve);
		}

		#endregion

	}

}
