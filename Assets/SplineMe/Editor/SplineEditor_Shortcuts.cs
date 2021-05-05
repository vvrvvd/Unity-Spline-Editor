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
			ScheduleAddCurve();
		}

		[ShortcutAttribute("Spline Editor/Split Curve", KeyCode.M, ShortcutModifiers.Action)]
		private static void SplitCurveShortcut()
		{
			ScheduleSplitCurve(splitCurveValue);
		}

		[ShortcutAttribute("Spline Editor/Remove Curve", KeyCode.End, ShortcutModifiers.Action)]
		private static void RemoveSelectedCurveShortcut()
		{
			ScheduleRemoveSelectedCurve();
		}

		[ShortcutAttribute("Spline Editor/Cast Curve Points", KeyCode.U, ShortcutModifiers.Action)]
		private static void CastCurvePointsShortcut()
		{
			if(currentSpline==null)
			{
				return;
			}

			var customDirection = -currentSpline.transform.up;
			ScheduleCastSpline(customDirection);
		}

		[ShortcutAttribute("Spline Editor/Factor Curve", KeyCode.G, ShortcutModifiers.Action)]
		private static void FactorSplineShortcut()
		{
			ScheduleFactorSpline();
		}

		[ShortcutAttribute("Spline Editor/Simplify Curve", KeyCode.H, ShortcutModifiers.Action)]
		private static void SimplifySplineShortcut()
		{
			ScheduleSimplifySpline();
		}

		[ShortcutAttribute("Spline Editor/Toggle Draw Spline Mode", KeyCode.Slash, ShortcutModifiers.Action)]
		private static void ToggleDrawSplineModeShortcut()
		{
			ToggleDrawSplineMode();
		}

		[ClutchShortcut("Spline Editor/Snap Spline End Points", KeyCode.S, ShortcutModifiers.None)]
		private static void SnapSplineEndPointsShortcut()
		{
			ToggleSnapCurvePointMode();
		}

		[ClutchShortcut("Spline Editor/Cast Selected Point To Mouse Position", KeyCode.U, ShortcutModifiers.None)]
		private static void TryCastSelectedPointShortcut()
		{
			ScheduleCastSelectedPoint();
		}

		#endregion

	}

}
