// <copyright file="SplineEditor_Flags.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// Partial class providing static flags system feature implementation.
	/// </summary>
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

		/// <summary>
		/// Sets flag for adding new curve in the next possible frame.
		/// </summary>
		/// <param name="curveLength">Length of the added curve.</param>
		internal static void ScheduleAddCurve(float curveLength)
		{
			addCurveFlag = true;
			addCurveLength = curveLength;
		}

		/// <summary>
		/// Sets flag for removing selected curve in the next possible frame.
		/// </summary>
		internal static void ScheduleRemoveSelectedCurve()
		{
			removeSelectedCurveFlag = true;
		}

		/// <summary>
		/// Sets flag for toggling close loop operation on curve in the next possible frame.
		/// </summary>
		internal static void ScheduleToggleCloseLoop()
		{
			closeLoopFlag = true;
		}

		/// <summary>
		/// Sets flag for factoring curve in the next possible frame.
		/// </summary>
		internal static void ScheduleFactorSpline()
		{
			factorSplineFlag = true;
		}

		/// <summary>
		/// Sets flag for simplifying curve in the next possible frame.
		/// </summary>
		internal static void ScheduleSimplifySpline()
		{
			simplifySplineFlag = true;
		}

		/// <summary>
		/// Sets flag for casting currently selected point in the next possible frame.
		/// </summary>
		internal static void ScheduleCastSelectedPoint()
		{
			castSelectedPointFlag = !castSelectedPointShortcutFlag;
			castSelectedPointShortcutFlag = castSelectedPointFlag;
		}

		/// <summary>
		/// Sets flag for splitting curve in the next possible frame.
		/// </summary>
		/// <param name="splitPointValue">Value of t parameter to get split point from bezier curve (normalized from 0 to 1).</param>
		internal static void ScheduleSplitCurve(float splitPointValue)
		{
			splitCurveValue = splitPointValue;
			splitCurveFlag = true;
		}

		/// <summary>
		/// Sets flag for casting all spline points in regard to to camera view in the next possible frame.
		/// </summary>
		internal static void ScheduleCastSplineToCameraView()
		{
			castSplineToCameraFlag = true;
		}

		/// <summary>
		/// Sets flag for casting all spline points in regard to given direction in the next possible frame.
		/// </summary>
		/// <param name="direction">Direction to cast spline to.</param>
		internal static void ScheduleCastSpline(Vector3 direction)
		{
			castSplineDirection = direction;
			castSplineFlag = true;
		}

		/// <summary>
		/// Sets flag for toggling draw spline mode in the next possible frame.
		/// </summary>
		internal static void ToggleDrawSplineMode()
		{
			drawSplineModeFlag = !drawSplineModeFlag;
		}

		/// <summary>
		/// Sets flag for toggling normals editor mode in the next possible frame.
		/// </summary>
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
				ToggleNormalsEditorMode(!EditorState.IsNormalsEditorMode);
				normalsEditorModeFlag = false;
			}

			if (drawSplineModeFlag)
			{
				ToggleDrawCurveMode(!EditorState.IsDrawerMode);
				drawSplineModeFlag = false;
			}

			if (splitCurveFlag)
			{
				SplitCurve(splitCurveValue);
				splitCurveFlag = false;
			}

			if (closeLoopFlag)
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
				if (sceneCamera != null)
				{
					castSplineDirection = sceneCamera.transform.forward;
					CastSpline(castSplineDirection);
				}

				castSplineToCameraFlag = false;
			}

			if (!EditorState.IsDrawerMode)
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
