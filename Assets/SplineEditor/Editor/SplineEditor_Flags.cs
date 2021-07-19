using UnityEngine;
using UnityEditor;

namespace SplineEditor.Editor
{

	public partial class SplineEditor : UnityEditor.Editor
	{

		private static bool addCurveFlag;
		private static bool splitCurveFlag;
		private static bool removeSelectedCurveFlag;
		private static bool castSplineFlag;
		private static bool castSplineToCameraFlag;
		private static bool closeLoopFlag;
		private static bool factorSplineFlag;
		private static bool simplifySplineFlag;
		private static bool normalsEditorModeFlag;
		private static bool drawSplineModeFlag;
		private static bool castSelectedPointFlag;
		private static bool castSelectedPointShortcutFlag;

		private static float addCurveLength = 1f;
		private static float splitCurveValue = 0.5f;
		private static Vector3 castSplineDirection;

		internal static void ScheduleAddCurve(float curveLength)
		{
			addCurveFlag = true;
			addCurveLength = curveLength;
		}

		internal static void ScheduleRemoveSelectedCurve()
		{
			removeSelectedCurveFlag = true;
		}

		internal static void ScheduleToggleCloseLoop()
		{
			closeLoopFlag = true;
		}

		internal static void ScheduleFactorSpline()
		{
			factorSplineFlag = true;
		}

		internal static void ScheduleSimplifySpline()
		{
			simplifySplineFlag = true;
		}

		private static void ScheduleCastSelectedPoint()
		{
			castSelectedPointFlag = !castSelectedPointShortcutFlag;
			castSelectedPointShortcutFlag = castSelectedPointFlag;
		}

		internal static void ScheduleSplitCurve(float splitPointValue)
		{
			splitCurveValue = splitPointValue;
			splitCurveFlag = true;
		}

		internal static void ScheduleCastSplineToCameraView()
		{
			castSplineToCameraFlag = true;
		}

		internal static void ScheduleCastSpline(Vector3 direction)
		{
			castSplineDirection = direction;
			castSplineFlag = true;
		}

		internal static void ToggleDrawSplineMode()
		{
			drawSplineModeFlag = !drawSplineModeFlag;
		}
		
		internal static void ToggleNormalsEditorMode()
		{
			normalsEditorModeFlag = !normalsEditorModeFlag;
		}

		private void InitializeFlags()
		{
			closeLoopFlag = false;
			factorSplineFlag = false;
			simplifySplineFlag = false;
			addCurveFlag = false;
			removeSelectedCurveFlag = false;
			normalsEditorModeFlag = false;
			drawSplineModeFlag = false;
			castSplineFlag = false;
			castSplineToCameraFlag = false;
			castSelectedPointFlag = false;
		}

		private void InvokeScheduledActions()
		{
			if (normalsEditorModeFlag)
			{
				ToggleNormalsEditorMode(!editorState.IsNormalsEditorMode);
				normalsEditorModeFlag = false;
			}

			if (drawSplineModeFlag)
			{
				ToggleDrawCurveMode(!editorState.IsDrawerMode);
				drawSplineModeFlag = false;
			}

			if(splitCurveFlag)
			{
				SplitCurve(splitCurveValue);
				splitCurveFlag = false;
			}	

			if(closeLoopFlag)
			{
				ToggleCloseLoop();
				closeLoopFlag = false;
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

			if (!editorState.IsDrawerMode)
			{
				if (addCurveFlag)
				{
					AddCurve(addCurveLength);
					addCurveFlag = false;
				}

				if (removeSelectedCurveFlag)
				{
					RemoveSelectedCurve();
					removeSelectedCurveFlag = false;
				}
			}
		}

		

	}

}
