using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEditor;

namespace SplineMe.Editor
{
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Static Fields

		private static bool addCurveFlag;
		private static bool splitCurveFlag;
		private static bool removeSelectedCurveFlag;
		private static bool castSplineFlag;
		private static bool castSplineToCameraFlag;
		private static bool factorSplineFlag;
		private static bool simplifySplineFlag;
		private static bool drawSplineModeFlag;
		private static bool castSelectedPointFlag;
		private static bool castSelectedPointShortcutFlag;
		private static bool snapEndPointsFlag;

		private static float splitCurveValue = 0.5f;
		private static Vector3 castSplineDirection;

		#endregion

		#region ShortcutManager Callbacks

		[ShortcutAttribute("Spline Editor/Add Curve", KeyCode.Home, ShortcutModifiers.Action)]
		internal static void AddCurveShortcut()
		{
			addCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Split Curve", KeyCode.M, ShortcutModifiers.Action)]
		private static void SplitCurveShortcut()
		{
			SplitCurveByPoint(splitCurveValue);
		}

		internal static void SplitCurveByPoint(float splitPointValue)
		{
			splitCurveValue = splitPointValue;
			splitCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Remove Curve", KeyCode.End, ShortcutModifiers.Action)]
		internal static void RemoveSelectedCurveShortcut()
		{
			removeSelectedCurveFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Cast Curve Points", KeyCode.U, ShortcutModifiers.Action)]
		private static void CastCurvePointsShortcut()
		{
			if (currentSpline != null)
			{
				return;
			}

			var customRay = -currentSpline.transform.up;
			CastCurvePoints(customRay);
		}

		internal static void CastSplineToCameraView()
		{
			castSplineToCameraFlag = true;
		}

		internal static void CastCurvePoints(Vector3 direction)
		{
			castSplineDirection = direction;
			castSplineFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Factor Curve", KeyCode.G, ShortcutModifiers.Action)]
		internal static void FactorSplineShortcut()
		{
			factorSplineFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Simplify Curve", KeyCode.H, ShortcutModifiers.Action)]
		internal static void SimplifySplineShortcut()
		{
			simplifySplineFlag = true;
		}

		[ShortcutAttribute("Spline Editor/Toggle Draw Spline Mode", KeyCode.Slash, ShortcutModifiers.Action)]
		internal static void ToggleDrawSplineModeShortcut()
		{
			drawSplineModeFlag = !drawSplineModeFlag;
		}

		[ClutchShortcut("Spline Editor/Snap Spline End Points", KeyCode.S, ShortcutModifiers.None)]
		private static void SnapSplineEndPoints()
		{
			if(currentEditor!=null && CurrentSpline != null && snapEndPointsFlag && currentEditor.isSnapping)
			{
				Undo.RecordObject(CurrentSpline, "Snap Spline End Points");
				CurrentSpline.IsLoop = true;
			}

			snapEndPointsFlag = !snapEndPointsFlag;
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
			factorSplineFlag = false;
			simplifySplineFlag = false;
			addCurveFlag = false;
			removeSelectedCurveFlag = false;
			drawSplineModeFlag = false;
			castSplineFlag = false;
			castSplineToCameraFlag = false;
			castSelectedPointFlag = false;
			snapEndPointsFlag = false;
		}

		private void ApplyShortcuts()
		{
			if (drawSplineModeFlag)
			{
				ToggleDrawCurveMode(!isCurveDrawerMode);
				drawSplineModeFlag = false;
			}

			if(splitCurveFlag)
			{
				SplitCurve(splitCurveValue);
				splitCurveFlag = false;
			}	

			if (factorSplineFlag)
			{
				FactorCurve();
				factorSplineFlag = false;
			}

			if (simplifySplineFlag)
			{
				SimplifySpline();
				simplifySplineFlag = false;
			}

			if (castSplineFlag)
			{
				castSplineFlag = false;
				CastSpline(castSplineDirection);
			}

			if (castSplineToCameraFlag)
			{
				var sceneCamera = SceneView.lastActiveSceneView.camera;
				if(sceneCamera!=null)
				{
					castSplineDirection = sceneCamera.transform.forward;
					CastSpline(castSplineDirection);
				}

				castSplineToCameraFlag = false;
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
