using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		#region Static Fields

		private static string WindowTitle { get; } = "Spline Editor";
		private static GUILayoutOption ToolsSettingsButtonWidth { get; } = GUILayout.Width(30);
		private static GUILayoutOption ToolsSettingsButtonHeight { get; } = GUILayout.Height(30);
		private static GUILayoutOption ToolsHeaderToolbarWidth { get; } = GUILayout.Width(128);
		private static GUILayoutOption ToolsHeaderToolbarHeight { get; } = GUILayout.Height(18);
		private static GUILayoutOption ToolsButtonsWidth { get; } = GUILayout.Width(92);
		private static GUILayoutOption ToolsButtonsHeight { get; } = GUILayout.Height(40);
		private static GUILayoutOption ToolsPointPositionWidth { get; } = GUILayout.Width(275);
		private static GUILayoutOption ToolsPointPopupLabelWidth { get; } = GUILayout.Width(50);
		private static GUILayoutOption ToolsPointPopupWidth { get; } = GUILayout.Width(80);
		private static GUILayoutOption ToolsSliderWidth { get; } = GUILayout.Width(175);
		private static GUILayoutOption ToolsCustomTransformFieldWidth { get; } = GUILayout.Width(175);

		#region Header Styles


		private const string HeaderTitle = "Spline Editor";

		private const string SettingsTooltip = "Editor Settings";
		private const string LayoutTextTitle = "T";
		private const string LayoutTextTooltip = "Text Layout";
		private const string LayoutImageTitle = "";
		private const string LayoutImageTooltip = "Image Layout";
		private const string LayoutTextAndImageTitle = "+T";
		private const string LayoutTextAndImageTooltip = "Text & Image Layout";

		private GUIContent settingsButtonContent = new GUIContent(string.Empty, SettingsTooltip);

		private GUIContent[] layoutsButtonsContent = new GUIContent[]
		{
				new GUIContent(LayoutTextTitle, LayoutTextTooltip),
				new GUIContent(LayoutImageTitle, LayoutImageTooltip),
				new GUIContent(LayoutTextAndImageTitle, LayoutTextAndImageTooltip),
		};

		#endregion

		#region Point Styles

		private const string PointGroupTitle = "Point";
		private const string PointPositionLabel = "Position";
		private const string PointScaleLabel = "Scale";
		private const string PointModeLabel = "Mode";
		private const string ApplyToAllPointsLabel = "Apply To All";
		private const string ApplyToAllPointsTooltip = "Apply currently selected mode to all control points.";

		private static GUIContent PointPositionContent = new GUIContent(PointPositionLabel);
		private static GUIContent PointScaleContent = new GUIContent(PointScaleLabel);
		private static GUIContent PointModeContent = new GUIContent(PointModeLabel);
		private static GUIContent ApplyToAllPoinstButtonContent = new GUIContent(ApplyToAllPointsLabel);

		#endregion

		#region Curve Styles

		private const string BezierGroupTitle = "Curve";
		private const string AddCurveButtonTitle = "Add Curve";
		private const string AddCurveLengthTitle = "Add Curve Length";
		private const string AddCurveButtonTooltip = "Add curve at the beginning or the end of the spline.";
		private const string RemoveCurveButtonTitle = "Remove Curve";
		private const string RemoveCurveButtonTooltip = "Remove selected curve.";
		private const string SplitCurveButtonTitle = "Split Curve";
		private const string SplitCurveButtonTooltip = "Split curve by adding mid point.";
		private const string SplitPointSliderLabel = "Split Point";

		private static GUIContent AddCurveButtonContent = new GUIContent();
		private static GUIContent AddCurveLengthFieldContent = new GUIContent(AddCurveLengthTitle);
		private static GUIContent RemoveCurveButtonContent = new GUIContent();
		private static GUIContent SplitCurveButtonContent = new GUIContent();
		private static GUIContent SplitPointSliderContent = new GUIContent(SplitPointSliderLabel);

		#endregion

		#region Spline Styles

		private const string SplineOptionsTitle = "Spline";
		private const string CloseLoopButtonTitle = "Close Loop";
		private const string CloseLoopButtonTooltip = "Loop spline by adding closing curve.";
		private const string OpenLoopButtonTitle = "Open Loop";
		private const string OpenLoopButtonTooltip = "Open spline by removing last closing curve.";
		private const string FactorSplineButtonTitle = "Factor Spline";
		private const string FactorSplineButtonTooltip = "Factor spline by adding mid points to every curve.";
		private const string SimplifySplineButtonTitle = "Simplify Spline";
		private const string SimplifySplineButtonTooltip = "Simplify spline by removing every second curve.";
		private const string CastSplineButtonTitle = "Cast Spline";
		private const string CastSplineButtonTooltip = "Cast spline regarding to cast transform or self (transform == null).";
		private const string CastSplineToCameraButtonTitle = "Cast To Camera";
		private const string CastSplineToCameraButtonTooltip = "Cast spline regarding to camera view.";
		private const string CastTransformFieldLabel = "Cast Transform";
		private const string LengthSplineFieldLabel = "Length";

		private const string DrawPointsFieldLabel = "Draw Points";
		private const string DrawSplineFieldLabel = "Draw Spline";
		private const string DrawNormalsToggleFieldLabel = "Draw Normals";
		private const string ShowTransformHandleFieldLabel = "Show Transform Handle";
		private const string AlwaysDrawOnSceneFieldLabel = "Always Draw On Scene";

		private static GUIContent CloseLoopButtonContent= new GUIContent();
		private static GUIContent FactorSplineButtonContent = new GUIContent();
		private static GUIContent SimplifyButtonContent = new GUIContent();
		private static GUIContent CastSplineContent = new GUIContent();
		private static GUIContent CastSplineToCameraContent = new GUIContent();
		private static GUIContent CastTransformFieldContent = new GUIContent(CastTransformFieldLabel);
		private static GUIContent LengthSplineFieldContent = new GUIContent(LengthSplineFieldLabel);
		private static GUIContent DrawPointsFieldContent = new GUIContent(DrawPointsFieldLabel);
		private static GUIContent DrawSplineFieldContent = new GUIContent(DrawSplineFieldLabel);
		private static GUIContent DrawNormalsToggleFieldContent = new GUIContent(DrawNormalsToggleFieldLabel);
		private static GUIContent ShowTransformHandleFieldContent = new GUIContent(ShowTransformHandleFieldLabel);
		private static GUIContent AlwaysDrawOnSceneFieldContent = new GUIContent(AlwaysDrawOnSceneFieldLabel);

		#endregion

		#region Drawer Styles

		private const string DrawerGroupTitle = "Drawer Tool";
		private const string DrawCurveButtonTitle = "Drawer Tool";
		private const string DrawCurveButtonTooltip = "Draw spline using mouse.";
		private const string DrawCurveSegmentLengthLabel = "Segment length";
		private const string DrawCurveSmoothAnglesLabel = "Smooth angles";
		private const string DrawCurveFirstHookLabel = "1st point hook";
		private const string DrawCurveSecondHookLabel = "2nd point hook";

		private static GUIContent DrawCurveButtonContent = new GUIContent();
		private static GUIContent DrawCurveSmoothAnglesContent = new GUIContent(DrawCurveSmoothAnglesLabel);
		private static GUIContent DrawCurveSegmentLengthContent = new GUIContent(DrawCurveSegmentLengthLabel);
		private static GUIContent DrawCurveFirstHookContent = new GUIContent(DrawCurveFirstHookLabel);
		private static GUIContent DrawCurveSecondHookContent = new GUIContent(DrawCurveSecondHookLabel);

		#endregion

		#region Normals Editor Styles

		private const string NormalsEditorGroupTitle = "Normals";
		private const string RotateNormalsToolLabel = "Rotate Tool";
		private const string RotateNormalsToolTooltip = "Rotate normal vectors using Unity handle.";
		private const string NormalsEditorGlobalRotationLabel = "Global Rotation";
		private const string NormalsEditorLocalRotationLabel = "Local Rotation";
		private const string FlipNormalsToggleFieldLabel = "Flip Normals";

		private static GUIContent NormalsEditorButtonContent = new GUIContent();
		private static GUIContent NormalsEditorGlobalRotationContent = new GUIContent(NormalsEditorGlobalRotationLabel);
		private static GUIContent NormalsEditorLocalRotationContent = new GUIContent(NormalsEditorLocalRotationLabel);
		private static GUIContent FlipNormalsToggleFieldContent = new GUIContent(FlipNormalsToggleFieldLabel);

		#endregion

		#endregion

		#region Private Fields

		private GUIStyle buttonStyle;
		private GUIStyle groupsStyle;
		private GUIStyle headerLabelStyle;
		private GUIStyle toggleButtonStyle;
		private GUIStyle settingsButtonStyle;

		#endregion

		#region Private Methods

		private void InitializeStyles()
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
			headerLabelStyle = editorSettings.guiSkin.FindStyle("Header");
			toggleButtonStyle = editorSettings.guiSkin.FindStyle("ToggleButton");
			settingsButtonStyle = editorSettings.guiSkin.FindStyle("SettingsButton");
		}

		private void UpdateStyles()
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			headerLabelStyle = editorSettings.guiSkin.FindStyle("Header");
			toggleButtonStyle = editorSettings.guiSkin.FindStyle("ToggleButton");
			settingsButtonStyle = editorSettings.guiSkin.FindStyle("SettingsButton");

			editorWindowState.useText = buttonsLayoutIndex == 0 || buttonsLayoutIndex == 2;
			editorWindowState.useImages = buttonsLayoutIndex == 1 || buttonsLayoutIndex == 2;

			//header
			layoutsButtonsContent[1].image = editorSettings.imageLayoutIcon;
			layoutsButtonsContent[2].image = editorSettings.imageLayoutIcon;

			//points
			ApplyToAllPoinstButtonContent.text = editorWindowState.useText ? ApplyToAllPointsLabel : string.Empty;
			ApplyToAllPoinstButtonContent.image = editorWindowState.useImages ? editorSettings.applyToAllPointsIcon : null;
			ApplyToAllPoinstButtonContent.tooltip = editorWindowState.useText ? ApplyToAllPointsTooltip : ApplyToAllPointsLabel;

			//curve
			AddCurveButtonContent.text = editorWindowState.useText ? AddCurveButtonTitle : string.Empty;
			AddCurveButtonContent.image = editorWindowState.useImages ? editorSettings.addCurveIcon : null;
			AddCurveButtonContent.tooltip = editorWindowState.useText ? AddCurveButtonTooltip : AddCurveButtonTitle;

			RemoveCurveButtonContent.text = editorWindowState.useText ? RemoveCurveButtonTitle : string.Empty;
			RemoveCurveButtonContent.image = editorWindowState.useImages ? editorSettings.removeCurveIcon : null;
			RemoveCurveButtonContent.tooltip = editorWindowState.useText ? RemoveCurveButtonTooltip : RemoveCurveButtonTitle;

			SplitCurveButtonContent.text = editorWindowState.useText ? SplitCurveButtonTitle : string.Empty;
			SplitCurveButtonContent.image = editorWindowState.useImages ? editorSettings.splitCurveIcon : null;
			SplitCurveButtonContent.tooltip = editorWindowState.useText ? SplitCurveButtonTooltip : SplitCurveButtonTitle;

			//spline
			CloseLoopButtonContent.text = editorWindowState.useText ? CloseLoopButtonTitle : string.Empty;
			CloseLoopButtonContent.image = editorWindowState.useImages ? editorSettings.closeLoopIcon : null;
			CloseLoopButtonContent.tooltip = editorWindowState.useText ? CloseLoopButtonTooltip : CloseLoopButtonTitle;

			if (editorState.CurrentSpline!=null && editorState.CurrentSpline.IsLoop)
			{
				CloseLoopButtonContent.text = editorWindowState.useText ? OpenLoopButtonTitle : string.Empty;
				CloseLoopButtonContent.image = editorWindowState.useImages ? editorSettings.openLoopIcon : null;
				CloseLoopButtonContent.tooltip = editorWindowState.useText ? OpenLoopButtonTooltip: OpenLoopButtonTitle;
			}

			FactorSplineButtonContent.text = editorWindowState.useText ? FactorSplineButtonTitle : string.Empty;
			FactorSplineButtonContent.image = editorWindowState.useImages ? editorSettings.factorSplineIcon : null;
			FactorSplineButtonContent.tooltip = editorWindowState.useText ? FactorSplineButtonTooltip : FactorSplineButtonTitle;

			SimplifyButtonContent.text = editorWindowState.useText ? SimplifySplineButtonTitle : string.Empty;
			SimplifyButtonContent.image = editorWindowState.useImages ? editorSettings.simplifySplineIcon : null;
			SimplifyButtonContent.tooltip = editorWindowState.useText ? SimplifySplineButtonTooltip : SimplifySplineButtonTitle;

			CastSplineContent.text = editorWindowState.useText ? CastSplineButtonTitle : string.Empty;
			CastSplineContent.image = editorWindowState.useImages ? editorSettings.castSplineIcon : null;
			CastSplineContent.tooltip = editorWindowState.useText ? CastSplineButtonTooltip : CastSplineButtonTitle;

			CastSplineToCameraContent.text = editorWindowState.useText ? CastSplineToCameraButtonTitle : string.Empty;
			CastSplineToCameraContent.image = editorWindowState.useImages ? editorSettings.castToCameraSplineIcon : null;
			CastSplineToCameraContent.tooltip = editorWindowState.useText ? CastSplineToCameraButtonTooltip : CastSplineToCameraButtonTitle;

			//normals
			NormalsEditorButtonContent.text = editorWindowState.useText ? RotateNormalsToolLabel : string.Empty;
			NormalsEditorButtonContent.image = editorWindowState.useImages ? editorSettings.normalsToolIcon : null;
			NormalsEditorButtonContent.tooltip = editorWindowState.useText ? RotateNormalsToolTooltip : RotateNormalsToolLabel;

			//drawer
			DrawCurveButtonContent.text = editorWindowState.useText ? DrawCurveButtonTitle : string.Empty;
			DrawCurveButtonContent.image = editorWindowState.useImages ? editorSettings.drawerToolIcon : null;
			DrawCurveButtonContent.tooltip = editorWindowState.useText ? DrawCurveButtonTooltip : DrawCurveButtonTitle;
		}

		#endregion

	}
}
