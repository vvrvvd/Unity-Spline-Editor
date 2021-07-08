using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private const string MeshOptionsGroupTitle = "Mesh";
		private const string MeshOptionsMeshWidthTitle = "Mesh Width";
		private const string MeshOptionsMeshSpacingTitle = "Mesh Spacing";
		private const string MeshOptionsUsePointsScaleTitle = "Use Points Scale";
		private const string MeshOptionsBakeMeshButtonTitle = "Bake Mesh";

		private const string MeshOptionsBakeMeshWindowTitle = "Save Spline Mesh Asset";
		private const string MeshOptionsBakeMeshWindowFolderPath = "Assets/";
		private const string MeshOptionsBakeMeshWindowFileName = "bakedSplineMesh";
		private const string MeshOptionsBakeMeshWindowFileExtension = "asset";

		private const string CurveOptionsGroupTitle = "Width Curve";
		private const string CurveOptionsAsymetrictWidthCurveTitle = "Asymetric Width Curve";
		private const string CurveOptionsWidthCurveTitle = "Width Curve";
		private const string CurveOptionsRightWidthCurveTitle = "Right Width Curve";
		private const string CurveOptionsLeftWidthCurveTitle = "Left Width Curve";

		private const string GuiOptionsGroupTitle = "GUI";
		private const string GuiOptionsDrawPointsTitle = "Draw Points";
		private const string GuiOptionsDrawNormalsTitle = "Draw Normals";

		private const string UvOptionsGroupTitle = "UV";
		private const string UvOptionsUvModeTitle = "UV Mode";
		private const string UvOptionsMirrorUvTitle = "Mirror UV";
		private const string UvOptionsShowDebugUvViewButtonTitle = "Show Debug UV";
		private const string UvOptionsHideDebugUvViewButtonTitle = "Hide Debug UV";

		private static GUIContent MeshOptionsMeshWidthFieldContent = new GUIContent(MeshOptionsMeshWidthTitle);
		private static GUIContent MeshOptionsMeshSpacingFieldContent = new GUIContent(MeshOptionsMeshSpacingTitle);
		private static GUIContent MeshOptionsUsePointsScaleToggleContent= new GUIContent(MeshOptionsUsePointsScaleTitle);
		private static GUIContent MeshOptionsBakeMeshButtonContent = new GUIContent(MeshOptionsBakeMeshButtonTitle);

		private static GUIContent CurveOptionsAsymetricWidthCurveToggleContent = new GUIContent(CurveOptionsAsymetrictWidthCurveTitle);
		private static GUIContent CurveOptionsWidthCurveContent = new GUIContent(CurveOptionsWidthCurveTitle);
		private static GUIContent CurveOptionsRightWidthCurveContent = new GUIContent(CurveOptionsRightWidthCurveTitle);
		private static GUIContent CurveOptionsLeftWidthCurveContent = new GUIContent(CurveOptionsLeftWidthCurveTitle);

		private static GUIContent GuiOptionsDrawPointsToggleContent = new GUIContent(GuiOptionsDrawPointsTitle);
		private static GUIContent GuiOptionsDrawNormalToggleContent = new GUIContent(GuiOptionsDrawNormalsTitle);

		private static GUIContent UvOptionsUvModeDropdownContent = new GUIContent(UvOptionsUvModeTitle);
		private static GUIContent UvOptionsMirrorUvToggleContent = new GUIContent(UvOptionsMirrorUvTitle);
		private static GUIContent UvOptionsHideDebugViewButtonContent = new GUIContent(UvOptionsHideDebugUvViewButtonTitle);
		private static GUIContent UvOptionsShowDebugViewButtonContent = new GUIContent(UvOptionsShowDebugUvViewButtonTitle);

		private static GUILayoutOption MaxDropdownWidth = GUILayout.MaxWidth(110);
		private static GUILayoutOption WidthCurveMaxWidth = GUILayout.MaxWidth(200);
		private static GUILayoutOption ButtonMaxWidth = GUILayout.MaxWidth(200);
		private static GUILayoutOption ButtonHeight = GUILayout.Height(30);

		private GUIStyle groupsStyle;
		private GUIStyle buttonStyle;

		private void InitializeStyles()
		{
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
			buttonStyle = meshEditorConfiguration.guiSkin.FindStyle("button");
		}

	}

}
