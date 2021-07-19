using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private static string WindowTitle { get; } = "Spline Editor";
		private static GUILayoutOption ToolsSettingsButtonWidth { get; } = GUILayout.Width(30);
		private static GUILayoutOption ToolsSettingsButtonHeight { get; } = GUILayout.Height(30);
		private static GUILayoutOption ToolsHeaderToolbarWidth { get; } = GUILayout.Width(128);
		private static GUILayoutOption ToolsHeaderToolbarHeight { get; } = GUILayout.Height(18);
		private static GUILayoutOption ToolsButtonsMinWidth { get; } = GUILayout.MinWidth(92);
		private static GUILayoutOption ToolsButtonsHeight { get; } = GUILayout.Height(40);


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

		

		private const string PointGroupTitle = "Point";
		private const string PointPositionLabel = "Position";
		private const string PointScaleLabel = "Scale";
		private const string PointModeLabel = "Mode";
		private const string ApplyToAllPointsLabel = "Apply Mode To All";
		private const string ApplyToAllPointsTooltip = "Apply currently selected mode to all control points.";

		private static GUIContent PointPositionContent = new GUIContent(PointPositionLabel);
		private static GUIContent PointScaleContent = new GUIContent(PointScaleLabel);
		private static GUIContent PointModeContent = new GUIContent(PointModeLabel);
		private static GUIContent ApplyToAllPoinstButtonContent = new GUIContent(ApplyToAllPointsLabel);

		private const string BezierGroupTitle = "Curve";
		private const string AddCurveButtonTitle = "Add";
		private const string AddCurveLengthTitle = "Add Curve Length";
		private const string AddCurveButtonTooltip = "Add curve at the beginning or the end of the spline.";
		private const string RemoveCurveButtonTitle = "Remove";
		private const string RemoveCurveButtonTooltip = "Remove selected curve.";
		private const string SplitCurveButtonTitle = "Split";
		private const string SplitCurveButtonTooltip = "Split curve by adding mid point.";
		private const string SplitPointSliderLabel = "Split Point";

		private static GUIContent AddCurveButtonContent = new GUIContent();
		private static GUIContent AddCurveLengthFieldContent = new GUIContent(AddCurveLengthTitle);
		private static GUIContent RemoveCurveButtonContent = new GUIContent();
		private static GUIContent SplitCurveButtonContent = new GUIContent();
		private static GUIContent SplitPointSliderContent = new GUIContent(SplitPointSliderLabel);
		
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

		private GUIStyle buttonStyle;
		private GUIStyle groupsStyle;
		private GUIStyle headerLabelStyle;
		private GUIStyle toggleButtonStyle;
		private GUIStyle settingsButtonStyle;

		
		private void InitializeStyles()
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
			headerLabelStyle = editorSettings.guiSkin.FindStyle("Header");
			toggleButtonStyle = editorSettings.guiSkin.FindStyle("ToggleButton");
			settingsButtonStyle = editorSettings.guiSkin.FindStyle("SettingsButton");
			buttonsLayoutIndex = editorWindowState.UseText && editorWindowState.UseImages ? 2 : editorWindowState.UseImages ? 1 : 0;
		}

		private void UpdateStyles()
		{
			buttonStyle = editorSettings.guiSkin.FindStyle("button");
			headerLabelStyle = editorSettings.guiSkin.FindStyle("Header");
			toggleButtonStyle = editorSettings.guiSkin.FindStyle("ToggleButton");
			settingsButtonStyle = editorSettings.guiSkin.FindStyle("SettingsButton");

			editorWindowState.UseText = buttonsLayoutIndex == 0 || buttonsLayoutIndex == 2;
			editorWindowState.UseImages = buttonsLayoutIndex == 1 || buttonsLayoutIndex == 2;

			//header
			layoutsButtonsContent[1].image = editorSettings.imageLayoutIcon;
			layoutsButtonsContent[2].image = editorSettings.imageLayoutIcon;

			//points
			ApplyToAllPoinstButtonContent.text = editorWindowState.UseText ? ApplyToAllPointsLabel : string.Empty;
			ApplyToAllPoinstButtonContent.image = editorWindowState.UseImages ? editorSettings.applyToAllPointsIcon : null;
			ApplyToAllPoinstButtonContent.tooltip = editorWindowState.UseText ? ApplyToAllPointsTooltip : ApplyToAllPointsLabel;

			//curve
			AddCurveButtonContent.text = editorWindowState.UseText ? AddCurveButtonTitle : string.Empty;
			AddCurveButtonContent.image = editorWindowState.UseImages ? editorSettings.addCurveIcon : null;
			AddCurveButtonContent.tooltip = editorWindowState.UseText ? AddCurveButtonTooltip : AddCurveButtonTitle;

			RemoveCurveButtonContent.text = editorWindowState.UseText ? RemoveCurveButtonTitle : string.Empty;
			RemoveCurveButtonContent.image = editorWindowState.UseImages ? editorSettings.removeCurveIcon : null;
			RemoveCurveButtonContent.tooltip = editorWindowState.UseText ? RemoveCurveButtonTooltip : RemoveCurveButtonTitle;

			SplitCurveButtonContent.text = editorWindowState.UseText ? SplitCurveButtonTitle : string.Empty;
			SplitCurveButtonContent.image = editorWindowState.UseImages ? editorSettings.splitCurveIcon : null;
			SplitCurveButtonContent.tooltip = editorWindowState.UseText ? SplitCurveButtonTooltip : SplitCurveButtonTitle;

			//spline
			CloseLoopButtonContent.text = editorWindowState.UseText ? CloseLoopButtonTitle : string.Empty;
			CloseLoopButtonContent.image = editorWindowState.UseImages ? editorSettings.closeLoopIcon : null;
			CloseLoopButtonContent.tooltip = editorWindowState.UseText ? CloseLoopButtonTooltip : CloseLoopButtonTitle;

			if (editorState.CurrentSpline!=null && editorState.CurrentSpline.IsLoop)
			{
				CloseLoopButtonContent.text = editorWindowState.UseText ? OpenLoopButtonTitle : string.Empty;
				CloseLoopButtonContent.image = editorWindowState.UseImages ? editorSettings.openLoopIcon : null;
				CloseLoopButtonContent.tooltip = editorWindowState.UseText ? OpenLoopButtonTooltip: OpenLoopButtonTitle;
			}

			FactorSplineButtonContent.text = editorWindowState.UseText ? FactorSplineButtonTitle : string.Empty;
			FactorSplineButtonContent.image = editorWindowState.UseImages ? editorSettings.factorSplineIcon : null;
			FactorSplineButtonContent.tooltip = editorWindowState.UseText ? FactorSplineButtonTooltip : FactorSplineButtonTitle;

			SimplifyButtonContent.text = editorWindowState.UseText ? SimplifySplineButtonTitle : string.Empty;
			SimplifyButtonContent.image = editorWindowState.UseImages ? editorSettings.simplifySplineIcon : null;
			SimplifyButtonContent.tooltip = editorWindowState.UseText ? SimplifySplineButtonTooltip : SimplifySplineButtonTitle;

			CastSplineContent.text = editorWindowState.UseText ? CastSplineButtonTitle : string.Empty;
			CastSplineContent.image = editorWindowState.UseImages ? editorSettings.castSplineIcon : null;
			CastSplineContent.tooltip = editorWindowState.UseText ? CastSplineButtonTooltip : CastSplineButtonTitle;

			CastSplineToCameraContent.text = editorWindowState.UseText ? CastSplineToCameraButtonTitle : string.Empty;
			CastSplineToCameraContent.image = editorWindowState.UseImages ? editorSettings.castToCameraSplineIcon : null;
			CastSplineToCameraContent.tooltip = editorWindowState.UseText ? CastSplineToCameraButtonTooltip : CastSplineToCameraButtonTitle;

			//normals
			NormalsEditorButtonContent.text = editorWindowState.UseText ? RotateNormalsToolLabel : string.Empty;
			NormalsEditorButtonContent.image = editorWindowState.UseImages ? editorSettings.normalsToolIcon : null;
			NormalsEditorButtonContent.tooltip = editorWindowState.UseText ? RotateNormalsToolTooltip : RotateNormalsToolLabel;

			//drawer
			DrawCurveButtonContent.text = editorWindowState.UseText ? DrawCurveButtonTitle : string.Empty;
			DrawCurveButtonContent.image = editorWindowState.UseImages ? editorSettings.drawerToolIcon : null;
			DrawCurveButtonContent.tooltip = editorWindowState.UseText ? DrawCurveButtonTooltip : DrawCurveButtonTitle;
		}

		

	}
}
