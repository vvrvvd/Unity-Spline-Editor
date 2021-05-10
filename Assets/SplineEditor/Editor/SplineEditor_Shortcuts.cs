using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEditor;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region ShortcutManager Callbacks

		[ShortcutAttribute("Spline Editor/Add Curve", KeyCode.Home, ShortcutModifiers.Action)]
		private static void AddCurveShortcut()
		{
			ScheduleAddCurve(addCurveLength);
		}

		[ShortcutAttribute("Spline Editor/Remove Curve", KeyCode.End, ShortcutModifiers.Action)]
		private static void RemoveSelectedCurveShortcut()
		{
			ScheduleRemoveSelectedCurve();
		}

		[ShortcutAttribute("Spline Editor/Split Curve", KeyCode.M, ShortcutModifiers.Action)]
		private static void SplitCurveShortcut()
		{
			ScheduleSplitCurve(splitCurveValue);
		}

		[ShortcutAttribute("Spline Editor/Cast Spline", KeyCode.U, ShortcutModifiers.Action)]
		private static void CastSplineShortcut()
		{
			if(currentSpline==null)
			{
				return;
			}

			var customDirection = -currentSpline.transform.up;
			ScheduleCastSpline(customDirection);
		}

		[ShortcutAttribute("Spline Editor/Cast Spline To Camera View", KeyCode.T, ShortcutModifiers.Action)]
		private static void CastSplineToCameraShortcut()
		{
			if (currentSpline == null)
			{
				return;
			}

			ScheduleCastSplineToCameraView();
		}

		[ShortcutAttribute("Spline Editor/Factor Spline", KeyCode.G, ShortcutModifiers.Action)]
		private static void FactorSplineShortcut()
		{
			ScheduleFactorSpline();
		}

		[ShortcutAttribute("Spline Editor/Simplify Spline", KeyCode.H, ShortcutModifiers.Action)]
		private static void SimplifySplineShortcut()
		{
			ScheduleSimplifySpline();
		}

		[ShortcutAttribute("Spline Editor/Toggle Drawer Tool", KeyCode.Slash, ShortcutModifiers.Action)]
		private static void ToggleDrawSplineModeShortcut()
		{
			ToggleDrawSplineMode();
		}

		[ClutchShortcut("Spline Editor/Cast Selected Point To Mouse Position", KeyCode.U, ShortcutModifiers.None)]
		private static void TryCastSelectedPointShortcut()
		{
			ScheduleCastSelectedPoint();
		}

		#endregion

	}

}
