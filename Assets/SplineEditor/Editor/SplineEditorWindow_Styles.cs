// <copyright file="SplineEditorWindow_Styles.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineEditor.
	/// Partial class providing titles, buttons and other GUI styles for custom editor.
	/// </summary>
	public partial class SplineEditorWindow : EditorWindow
	{
		private const string HeaderTitle = "Spline Editor";

		private const string SettingsTooltip = "Editor Settings";
		private const string LayoutTextTitle = "T";
		private const string LayoutTextTooltip = "Text Layout";
		private const string LayoutImageTitle = "";
		private const string LayoutImageTooltip = "Image Layout";
		private const string LayoutTextAndImageTitle = "+T";
		private const string LayoutTextAndImageTooltip = "Text & Image Layout";

		private const string PointGroupTitle = "Point";
		private const string PointPositionLabel = "Position";
		private const string PointScaleLabel = "Scale";
		private const string PointModeLabel = "Mode";
		private const string ApplyToAllPointsLabel = "Apply Mode To All";
		private const string ApplyToAllPointsTooltip = "Apply currently selected mode to all control points.";

		private const string BezierGroupTitle = "Curve";
		private const string AddCurveButtonTitle = "Add";
		private const string AddCurveLengthTitle = "Add Curve Length";
		private const string AddCurveButtonTooltip = "Add curve at the beginning or the end of the spline.";
		private const string RemoveCurveButtonTitle = "Remove";
		private const string RemoveCurveButtonTooltip = "Remove selected curve.";
		private const string SplitCurveButtonTitle = "Split";
		private const string SplitCurveButtonTooltip = "Split curve by adding mid point.";
		private const string SplitPointSliderLabel = "Split Point";

		private const string SplineOptionsTitle = "Spline";
		private const string CloseLoopButtonTitle = "Close";
		private const string CloseLoopButtonTooltip = "Loop spline by adding closing curve.";
		private const string OpenLoopButtonTitle = "Open";
		private const string OpenLoopButtonTooltip = "Open spline by removing last closing curve.";
		private const string FactorSplineButtonTitle = "Factor";
		private const string FactorSplineButtonTooltip = "Factor spline by adding mid points to every curve.";
		private const string SimplifySplineButtonTitle = "Simplify";
		private const string SimplifySplineButtonTooltip = "Simplify spline by removing every second curve.";
		private const string CastSplineButtonTitle = "Cast";
		private const string CastSplineButtonTooltip = "Cast spline regarding to cast transform or self (transform == null).";
		private const string CastSplineToCameraButtonTitle = "Camera Cast";
		private const string CastSplineToCameraButtonTooltip = "Cast spline regarding to camera view.";
		private const string CastTransformFieldLabel = "Cast Transform";
		private const string LengthSplineFieldLabel = "Length";

		private const string DrawPointsFieldLabel = "Draw Points";
		private const string DrawSplineFieldLabel = "Draw Spline";
		private const string DrawNormalsToggleFieldLabel = "Draw Normals";
		private const string ShowTransformHandleFieldLabel = "Show Transform Handle";
		private const string AlwaysDrawOnSceneFieldLabel = "Always Draw On Scene";

		private const string NormalsEditorGroupTitle = "Normals";
		private const string RotateNormalsToolLabel = "Rotate Tool";
		private const string RotateNormalsToolTooltip = "Rotate normal vectors using Unity handle.";
		private const string NormalsEditorGlobalRotationLabel = "Global Rotation";
		private const string NormalsEditorLocalRotationLabel = "Local Rotation";
		private const string FlipNormalsToggleFieldLabel = "Flip Normals";

		private const string DrawerGroupTitle = "Drawer Tool";
		private const string DrawCurveButtonTitle = "Drawer Tool";
		private const string DrawCurveButtonTooltip = "Draw spline using mouse.";
		private const string DrawCurveSegmentLengthLabel = "Segment length";
		private const string DrawCurveSmoothAnglesLabel = "Smooth angles";
		private const string DrawCurveFirstHookLabel = "1st point hook";
		private const string DrawCurveSecondHookLabel = "2nd point hook";

		private static readonly GUIContent PointPositionContent = new GUIContent(PointPositionLabel);
		private static readonly GUIContent PointScaleContent = new GUIContent(PointScaleLabel);
		private static readonly GUIContent PointModeContent = new GUIContent(PointModeLabel);
		private static readonly GUIContent ApplyToAllPoinstButtonContent = new GUIContent(ApplyToAllPointsLabel);

		private static readonly GUIContent AddCurveButtonContent = new GUIContent();
		private static readonly GUIContent AddCurveLengthFieldContent = new GUIContent(AddCurveLengthTitle);
		private static readonly GUIContent RemoveCurveButtonContent = new GUIContent();
		private static readonly GUIContent SplitCurveButtonContent = new GUIContent();
		private static readonly GUIContent SplitPointSliderContent = new GUIContent(SplitPointSliderLabel);

		private static readonly GUIContent CloseLoopButtonContent = new GUIContent();
		private static readonly GUIContent FactorSplineButtonContent = new GUIContent();
		private static readonly GUIContent SimplifyButtonContent = new GUIContent();
		private static readonly GUIContent CastSplineContent = new GUIContent();
		private static readonly GUIContent CastSplineToCameraContent = new GUIContent();
		private static readonly GUIContent CastTransformFieldContent = new GUIContent(CastTransformFieldLabel);
		private static readonly GUIContent LengthSplineFieldContent = new GUIContent(LengthSplineFieldLabel);
		private static readonly GUIContent DrawPointsFieldContent = new GUIContent(DrawPointsFieldLabel);
		private static readonly GUIContent DrawSplineFieldContent = new GUIContent(DrawSplineFieldLabel);
		private static readonly GUIContent DrawNormalsToggleFieldContent = new GUIContent(DrawNormalsToggleFieldLabel);
		private static readonly GUIContent ShowTransformHandleFieldContent = new GUIContent(ShowTransformHandleFieldLabel);
		private static readonly GUIContent AlwaysDrawOnSceneFieldContent = new GUIContent(AlwaysDrawOnSceneFieldLabel);

		private static readonly GUIContent DrawCurveButtonContent = new GUIContent();
		private static readonly GUIContent DrawCurveSmoothAnglesContent = new GUIContent(DrawCurveSmoothAnglesLabel);
		private static readonly GUIContent DrawCurveSegmentLengthContent = new GUIContent(DrawCurveSegmentLengthLabel);
		private static readonly GUIContent DrawCurveFirstHookContent = new GUIContent(DrawCurveFirstHookLabel);
		private static readonly GUIContent DrawCurveSecondHookContent = new GUIContent(DrawCurveSecondHookLabel);

		private static readonly GUIContent NormalsEditorButtonContent = new GUIContent();
		private static readonly GUIContent NormalsEditorGlobalRotationContent = new GUIContent(NormalsEditorGlobalRotationLabel);
		private static readonly GUIContent NormalsEditorLocalRotationContent = new GUIContent(NormalsEditorLocalRotationLabel);
		private static readonly GUIContent FlipNormalsToggleFieldContent = new GUIContent(FlipNormalsToggleFieldLabel);

		private GUIContent settingsButtonContent = new GUIContent(string.Empty, SettingsTooltip);

		private GUIContent[] layoutsButtonsContent = new GUIContent[]
		{
				new GUIContent(LayoutTextTitle, LayoutTextTooltip),
				new GUIContent(LayoutImageTitle, LayoutImageTooltip),
				new GUIContent(LayoutTextAndImageTitle, LayoutTextAndImageTooltip),
		};

		private GUIStyle buttonStyle;
		private GUIStyle groupsStyle;
		private GUIStyle headerLabelStyle;
		private GUIStyle toggleButtonStyle;
		private GUIStyle settingsButtonStyle;

		private static string WindowTitle { get; } = "Spline Editor";

		private static GUILayoutOption ToolsSettingsButtonWidth { get; } = GUILayout.Width(30);

		private static GUILayoutOption ToolsSettingsButtonHeight { get; } = GUILayout.Height(30);

		private static GUILayoutOption ToolsHeaderToolbarWidth { get; } = GUILayout.Width(128);

		private static GUILayoutOption ToolsHeaderToolbarHeight { get; } = GUILayout.Height(18);

		private static GUILayoutOption ToolsButtonsMinWidth { get; } = GUILayout.MinWidth(92);

		private static GUILayoutOption ToolsButtonsHeight { get; } = GUILayout.Height(40);

		private void InitializeStyles()
		{
			buttonStyle = EditorSettings.GuiSkin.FindStyle("button");
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
			headerLabelStyle = EditorSettings.GuiSkin.FindStyle("Header");
			toggleButtonStyle = EditorSettings.GuiSkin.FindStyle("ToggleButton");
			settingsButtonStyle = EditorSettings.GuiSkin.FindStyle("SettingsButton");
			buttonsLayoutIndex = EditorWindowState.UseText && EditorWindowState.UseImages ? 2 : EditorWindowState.UseImages ? 1 : 0;
		}

		private void UpdateStyles()
		{
			buttonStyle = EditorSettings.GuiSkin.FindStyle("button");
			headerLabelStyle = EditorSettings.GuiSkin.FindStyle("Header");
			toggleButtonStyle = EditorSettings.GuiSkin.FindStyle("ToggleButton");
			settingsButtonStyle = EditorSettings.GuiSkin.FindStyle("SettingsButton");

			EditorWindowState.UseText = buttonsLayoutIndex == 0 || buttonsLayoutIndex == 2;
			EditorWindowState.UseImages = buttonsLayoutIndex == 1 || buttonsLayoutIndex == 2;

			// Header
			layoutsButtonsContent[1].image = EditorSettings.ImageLayoutIcon;
			layoutsButtonsContent[2].image = EditorSettings.ImageLayoutIcon;

			// Points
			ApplyToAllPoinstButtonContent.text = EditorWindowState.UseText ? ApplyToAllPointsLabel : string.Empty;
			ApplyToAllPoinstButtonContent.image = EditorWindowState.UseImages ? EditorSettings.ApplyToAllPointsIcon : null;
			ApplyToAllPoinstButtonContent.tooltip = EditorWindowState.UseText ? ApplyToAllPointsTooltip : ApplyToAllPointsLabel;

			// Curve
			AddCurveButtonContent.text = EditorWindowState.UseText ? AddCurveButtonTitle : string.Empty;
			AddCurveButtonContent.image = EditorWindowState.UseImages ? EditorSettings.AddCurveIcon : null;
			AddCurveButtonContent.tooltip = EditorWindowState.UseText ? AddCurveButtonTooltip : AddCurveButtonTitle;

			RemoveCurveButtonContent.text = EditorWindowState.UseText ? RemoveCurveButtonTitle : string.Empty;
			RemoveCurveButtonContent.image = EditorWindowState.UseImages ? EditorSettings.RemoveCurveIcon : null;
			RemoveCurveButtonContent.tooltip = EditorWindowState.UseText ? RemoveCurveButtonTooltip : RemoveCurveButtonTitle;

			SplitCurveButtonContent.text = EditorWindowState.UseText ? SplitCurveButtonTitle : string.Empty;
			SplitCurveButtonContent.image = EditorWindowState.UseImages ? EditorSettings.SplitCurveIcon : null;
			SplitCurveButtonContent.tooltip = EditorWindowState.UseText ? SplitCurveButtonTooltip : SplitCurveButtonTitle;

			// Spline
			CloseLoopButtonContent.text = EditorWindowState.UseText ? CloseLoopButtonTitle : string.Empty;
			CloseLoopButtonContent.image = EditorWindowState.UseImages ? EditorSettings.CloseLoopIcon : null;
			CloseLoopButtonContent.tooltip = EditorWindowState.UseText ? CloseLoopButtonTooltip : CloseLoopButtonTitle;

			if (EditorState.CurrentSpline != null && EditorState.CurrentSpline.IsLoop)
			{
				CloseLoopButtonContent.text = EditorWindowState.UseText ? OpenLoopButtonTitle : string.Empty;
				CloseLoopButtonContent.image = EditorWindowState.UseImages ? EditorSettings.OpenLoopIcon : null;
				CloseLoopButtonContent.tooltip = EditorWindowState.UseText ? OpenLoopButtonTooltip : OpenLoopButtonTitle;
			}

			FactorSplineButtonContent.text = EditorWindowState.UseText ? FactorSplineButtonTitle : string.Empty;
			FactorSplineButtonContent.image = EditorWindowState.UseImages ? EditorSettings.FactorSplineIcon : null;
			FactorSplineButtonContent.tooltip = EditorWindowState.UseText ? FactorSplineButtonTooltip : FactorSplineButtonTitle;

			SimplifyButtonContent.text = EditorWindowState.UseText ? SimplifySplineButtonTitle : string.Empty;
			SimplifyButtonContent.image = EditorWindowState.UseImages ? EditorSettings.SimplifySplineIcon : null;
			SimplifyButtonContent.tooltip = EditorWindowState.UseText ? SimplifySplineButtonTooltip : SimplifySplineButtonTitle;

			CastSplineContent.text = EditorWindowState.UseText ? CastSplineButtonTitle : string.Empty;
			CastSplineContent.image = EditorWindowState.UseImages ? EditorSettings.CastSplineIcon : null;
			CastSplineContent.tooltip = EditorWindowState.UseText ? CastSplineButtonTooltip : CastSplineButtonTitle;

			CastSplineToCameraContent.text = EditorWindowState.UseText ? CastSplineToCameraButtonTitle : string.Empty;
			CastSplineToCameraContent.image = EditorWindowState.UseImages ? EditorSettings.CastToCameraSplineIcon : null;
			CastSplineToCameraContent.tooltip = EditorWindowState.UseText ? CastSplineToCameraButtonTooltip : CastSplineToCameraButtonTitle;

			// Normals Editor Mode
			NormalsEditorButtonContent.text = EditorWindowState.UseText ? RotateNormalsToolLabel : string.Empty;
			NormalsEditorButtonContent.image = EditorWindowState.UseImages ? EditorSettings.NormalsToolIcon : null;
			NormalsEditorButtonContent.tooltip = EditorWindowState.UseText ? RotateNormalsToolTooltip : RotateNormalsToolLabel;

			// Draw Curve Mode
			DrawCurveButtonContent.text = EditorWindowState.UseText ? DrawCurveButtonTitle : string.Empty;
			DrawCurveButtonContent.image = EditorWindowState.UseImages ? EditorSettings.DrawerToolIcon : null;
			DrawCurveButtonContent.tooltip = EditorWindowState.UseText ? DrawCurveButtonTooltip : DrawCurveButtonTitle;
		}
	}
}
