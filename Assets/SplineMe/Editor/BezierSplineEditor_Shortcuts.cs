using UnityEngine;
using UnityEditor.ShortcutManagement;

namespace SplineMe.Editor
{
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Static Fields

		private static bool addCurveFlag;
		private static bool addMidCurveFlag;
		private static bool removeSelectedCurveFlag;
		private static bool castCurveFlag;
		private static bool factorCurveFlag;
		private static bool simplifyCurveFlag;
		private static bool drawCurveModeFlag;
		private static bool castSelectedPointFlag;
		private static bool castSelectedPointShortcutFlag;

		#endregion

		#region ShortcutManager Callbacks

		[ShortcutAttribute("Spline Editor/Add Curve", KeyCode.Home, ShortcutModifiers.Action)]
		private static void AddCurveShortcut()
		{
			addCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Add Mid Curve", KeyCode.M, ShortcutModifiers.Action)]
		private static void AddMidCurveShortcut()
		{
			addMidCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Remove Curve", KeyCode.End, ShortcutModifiers.Action)]
		private static void RemoveSelectedCurveShortcut()
		{
			removeSelectedCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Cast Curve Points", KeyCode.U, ShortcutModifiers.Action)]
		private static void CastCurvePointsShortcut()
		{
			castCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Factor Curve", KeyCode.G, ShortcutModifiers.Action)]
		private static void FactorCurvesShortcut()
		{
			factorCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Simplify Curve", KeyCode.H, ShortcutModifiers.Action)]
		private static void SimplifyCurveShortcut()
		{
			simplifyCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Toggle Draw Curve Mode", KeyCode.Slash, ShortcutModifiers.Action)]
		private static void ToggleDrawCurveModeShortcut()
		{
			drawCurveModeFlag = !drawCurveModeFlag;
		}

		[ClutchShortcut("Spline Editor/Cast Selected Point To Mouse Position", KeyCode.U, ShortcutModifiers.None)]
		private static void TryCastSelectedPointShortcut()
		{
			castSelectedPointFlag = !castSelectedPointShortcutFlag;
			castSelectedPointShortcutFlag = castSelectedPointFlag;
		}

		#endregion

		#region Shortcuts Logic

		private void InitializeShortcuts()
		{
			factorCurveFlag = false;
			simplifyCurveFlag = false;
			addCurveFlag = false;
			removeSelectedCurveFlag = false;
			drawCurveModeFlag = false;
			castCurveFlag = false;
			castSelectedPointFlag = false;
		}

		private void ApplyShortcuts()
		{
			if (drawCurveModeFlag)
			{
				ToggleDrawCurveMode(!isCurveDrawerMode);
				drawCurveModeFlag = false;
			}

			if(addMidCurveFlag)
			{
				AddMidCurve();
				addMidCurveFlag = false;
			}	

			if (factorCurveFlag)
			{
				FactorCurve();
				factorCurveFlag = false;
			}

			if (simplifyCurveFlag)
			{
				SimplifyCurve();
				simplifyCurveFlag = false;
			}

			if (castCurveFlag)
			{
				CastCurve();
				castCurveFlag = false;
			}

			if (!isCurveDrawerMode)
			{
				if (addCurveFlag)
				{
					AddCurve();
					addCurveFlag = false;
				}

				if (removeSelectedCurveFlag)
				{
					RemoveSelectedCurve();
					removeSelectedCurveFlag = false;
				}
			}
		}

		#endregion

	}

}
