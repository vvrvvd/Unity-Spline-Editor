using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		#region Window Styles

		private const string WindowTitle = "Spline Editor";

		#endregion

		#region Tools Styles

		private static GUILayoutOption ToolsHeaderToolbarWidth { get; } = GUILayout.Width(128);
		private static GUILayoutOption ToolsHeaderToolbarHeight { get; } = GUILayout.Height(18);
		private static GUILayoutOption ToolsButtonsWidth { get; } = GUILayout.Width(110);
		private static GUILayoutOption ToolsButtonsHeight { get; } = GUILayout.Height(50);
		private static GUILayoutOption ToolsSliderWidth { get; } = GUILayout.Width(175);
		private static GUILayoutOption ToolsCustomTransformFieldWidth { get; } = GUILayout.Width(175);

		#region Header Styles


		private const string HeaderTitle = "Spline Editor";

		private const string LayoutTextTitle = "T";
		private const string LayoutTextTooltip = "Text Layout";
		private const string LayoutImageTitle = "";
		private const string LayoutImageTooltip = "Image Layout";
		private const string LayoutTextAndImageTitle = "+T";
		private const string LayoutTextAndImageTooltip = "Text & Image Layout";

		private GUIContent[] layoutsButtonsContent = new GUIContent[]
		{
				new GUIContent(LayoutTextTitle, LayoutTextTooltip),
				new GUIContent(LayoutImageTitle, LayoutImageTooltip),
				new GUIContent(LayoutTextAndImageTitle, LayoutTextAndImageTooltip),
		};

		#endregion

		#region Curve Styles

		private const string BezierGroupTitle = "Curve";
		private const string AddCurveButtonTitle = "Add Curve";
		private const string AddCurveButtonTooltip = "Add curve at the beginning or the end of the spline.";
		private const string RemoveCurveButtonTitle = "Remove Curve";
		private const string RemoveCurveButtonTooltip = "Remove selected curve.";
		private const string SplitCurveButtonTitle = "Split Curve";
		private const string SplitCurveButtonTooltip = "Split curve by adding mid point.";
		private const string SplitPointSliderLabel = "Split Point";

		private static GUIContent AddCurveButtonContent = new GUIContent();
		private static GUIContent RemoveCurveButtonContent = new GUIContent();
		private static GUIContent SplitCurveButtonContent = new GUIContent();
		private static GUIContent SplitPointSliderContent = new GUIContent(SplitPointSliderLabel);

		#endregion

		#region Spline Styles

		private const string SplineOptionsTitle = "Spline";
		private const string FactorSplineButtonTitle = "Factor Spline";
		private const string FactorSplineButtonTooltip = "Factor spline by adding mid points to every curve.";
		private const string SimplifySplineButtonTitle = "Simplify Spline";
		private const string SimplifySplineButtonTooltip = "Simplify spline by removing every second curve.";
		private const string CastSplineButtonTitle = "Cast Spline";
		private const string CastSplineButtonTooltip = "Cast spline regarding to cast transform or self (transform == null).";
		private const string CastSplineToCameraButtonTitle = "Cast Spline To Camera View";
		private const string CastSplineToCameraButtonTooltip = "Cast spline regarding to camera view.";
		private const string CastTransformFieldLabel = "Cast Transform";

		private static GUIContent FactorSplineButtonContent = new GUIContent();
		private static GUIContent SimplifyButtonContent = new GUIContent();
		private static GUIContent CastSplineContent = new GUIContent();
		private static GUIContent CastSplineToCameraContent = new GUIContent();
		private static GUIContent CastTransformFieldContent = new GUIContent(CastTransformFieldLabel);

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

		private bool useText = false;
		private bool useImages = false;

		private GUIStyle buttonStyle;
		private GUIStyle groupsStyle;

		private void InitializeStyles()
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
		}

		private void UpdateStyles()
		{

			useText = buttonsLayoutIndex == 0 || buttonsLayoutIndex == 2;
			useImages = buttonsLayoutIndex == 1 || buttonsLayoutIndex == 2;

			//header
			layoutsButtonsContent[1].image = editorSettings.imageLayoutIcon;
			layoutsButtonsContent[2].image = editorSettings.imageLayoutIcon;

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

	}
}
