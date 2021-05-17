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
		private static GUILayoutOption ToolsButtonsWidth { get; } = GUILayout.Width(90);
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
		private const string PointModeLabel = "Mode";
		private const string ApplyToAllPointsLabel = "Apply To All";
		private const string ApplyToAllPointsTooltip = "Apply currently selected mode to all control points.";

		private static GUIContent PointPositionContent = new GUIContent(PointPositionLabel);
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

		#endregion

		#region Private Fields

		private bool useText = false;
		private bool useImages = false;

		private GUIStyle buttonStyle;
		private GUIStyle groupsStyle;
		private GUIStyle headerLabelStyle;
		private GUIStyle drawerButtonStyle;
		private GUIStyle settingsButtonStyle;

		#endregion

		#region Private Methods

		private void InitializeStyles(SplineEditorConfiguration editorSettings)
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
			headerLabelStyle = editorSettings.guiSkin.FindStyle("Header");
			drawerButtonStyle = editorSettings.guiSkin.FindStyle("DrawerButton");
			settingsButtonStyle = editorSettings.guiSkin.FindStyle("SettingsButton");
		}

		private void UpdateStyles(SplineEditorConfiguration editorSettings)
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			headerLabelStyle = editorSettings.guiSkin.FindStyle("Header");
			drawerButtonStyle = editorSettings.guiSkin.FindStyle("DrawerButton");
			settingsButtonStyle = editorSettings.guiSkin.FindStyle("SettingsButton");

			useText = buttonsLayoutIndex == 0 || buttonsLayoutIndex == 2;
			useImages = buttonsLayoutIndex == 1 || buttonsLayoutIndex == 2;

			//header
			layoutsButtonsContent[1].image = editorSettings.imageLayoutIcon;
			layoutsButtonsContent[2].image = editorSettings.imageLayoutIcon;

			//points
			ApplyToAllPoinstButtonContent.text = useText ? ApplyToAllPointsLabel : string.Empty;
			ApplyToAllPoinstButtonContent.image = useImages ? editorSettings.applyToAllPointsIcon : null;
			ApplyToAllPoinstButtonContent.tooltip = useText ? ApplyToAllPointsTooltip : ApplyToAllPointsLabel;

			//curve
			AddCurveButtonContent.text = useText ? AddCurveButtonTitle : string.Empty;
			AddCurveButtonContent.image = useImages ? editorSettings.addCurveIcon : null;
			AddCurveButtonContent.tooltip = useText ? AddCurveButtonTooltip : AddCurveButtonTitle;

			RemoveCurveButtonContent.text = useText ? RemoveCurveButtonTitle : string.Empty;
			RemoveCurveButtonContent.image = useImages ? editorSettings.removeCurveIcon : null;
			RemoveCurveButtonContent.tooltip = useText ? RemoveCurveButtonTooltip : RemoveCurveButtonTitle;

			SplitCurveButtonContent.text = useText ? SplitCurveButtonTitle : string.Empty;
			SplitCurveButtonContent.image = useImages ? editorSettings.splitCurveIcon : null;
			SplitCurveButtonContent.tooltip = useText ? SplitCurveButtonTooltip : SplitCurveButtonTitle;

			//spline
			CloseLoopButtonContent.text = useText ? CloseLoopButtonTitle : string.Empty;
			CloseLoopButtonContent.image = useImages ? editorSettings.closeLoopIcon : null;
			CloseLoopButtonContent.tooltip = useText ? CloseLoopButtonTooltip : CloseLoopButtonTitle;

			if (SplineEditor.CurrentSpline!=null && SplineEditor.CurrentSpline.IsLoop)
			{
				CloseLoopButtonContent.text = useText ? OpenLoopButtonTitle : string.Empty;
			CloseLoopButtonContent.image = useImages ? editorSettings.openLoopIcon : null;
				CloseLoopButtonContent.tooltip = useText ? OpenLoopButtonTooltip: OpenLoopButtonTitle;
			}

			FactorSplineButtonContent.text = useText ? FactorSplineButtonTitle : string.Empty;
			FactorSplineButtonContent.image = useImages ? editorSettings.factorSplineIcon : null;
			FactorSplineButtonContent.tooltip = useText ? FactorSplineButtonTooltip : FactorSplineButtonTitle;

			SimplifyButtonContent.text = useText ? SimplifySplineButtonTitle : string.Empty;
			SimplifyButtonContent.image = useImages ? editorSettings.simplifySplineIcon : null;
			SimplifyButtonContent.tooltip = useText ? SimplifySplineButtonTooltip : SimplifySplineButtonTitle;

			CastSplineContent.text = useText ? CastSplineButtonTitle : string.Empty;
			CastSplineContent.image = useImages ? editorSettings.castSplineIcon : null;
			CastSplineContent.tooltip = useText ? CastSplineButtonTooltip : CastSplineButtonTitle;

			CastSplineToCameraContent.text = useText ? CastSplineToCameraButtonTitle : string.Empty;
			CastSplineToCameraContent.image = useImages ? editorSettings.castToCameraSplineIcon : null;
			CastSplineToCameraContent.tooltip = useText ? CastSplineToCameraButtonTooltip : CastSplineToCameraButtonTitle;

			//drawer
			DrawCurveButtonContent.text = useText ? DrawCurveButtonTitle : string.Empty;
			DrawCurveButtonContent.image = useImages ? editorSettings.drawerToolIcon : null;
			DrawCurveButtonContent.tooltip = useText ? DrawCurveButtonTooltip : DrawCurveButtonTitle;

		}

		#endregion

	}
}
