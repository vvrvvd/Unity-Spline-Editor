using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

		private static GUILayoutOption FloatFieldWidth = GUILayout.Width(150);
		private static GUILayoutOption MaxDropdownWidth = GUILayout.MaxWidth(110);
		private static GUILayoutOption WidthCurveMaxWidth = GUILayout.MaxWidth(200);
		private static GUILayoutOption ButtonMaxWidth = GUILayout.MaxWidth(200);
		private static GUILayoutOption ButtonHeight = GUILayout.Height(30);

		#region GUI Options Styles

		private const string GuiOptionsGroupTitle = "GUI";
		private const string GuiOptionsDrawPointsTitle = "Draw Points";
		private const string GuiOptionsDrawNormalsTitle = "Draw Normals";

		private static GUIContent GuiOptionsDrawPointsToggleContent = new GUIContent(GuiOptionsDrawPointsTitle);
		private static GUIContent GuiOptionsDrawNormalToggleContent = new GUIContent(GuiOptionsDrawNormalsTitle);

		#endregion

		#region UV Options Styles

		private const string UvOptionsGroupTitle = "UV";
		private const string UvOptionsUvModeTitle = "UV Mode";
		private const string UvOptionsMirrorUvTitle = "Mirror UV";
		private const string UvOptionsShowDebugUvViewButtonTitle = "Show Debug UV";
		private const string UvOptionsHideDebugUvViewButtonTitle = "Hide Debug UV";

		private static GUIContent UvOptionsUvModeDropdownContent = new GUIContent(UvOptionsUvModeTitle);
		private static GUIContent UvOptionsMirrorUvToggleContent = new GUIContent(UvOptionsMirrorUvTitle);
		private static GUIContent UvOptionsHideDebugUvViewButtonContent = new GUIContent(UvOptionsHideDebugUvViewButtonTitle);
		private static GUIContent UvOptionsShowDebugUvViewButtonContent = new GUIContent(UvOptionsShowDebugUvViewButtonTitle);

		#endregion

		#region Mesh Options Styles

		private const string MeshOptionsGroupTitle = "Mesh";
		private const string MeshOptionsMeshWidthTitle = "Mesh Width";
		private const string MeshOptionsMeshSpacingTitle = "Mesh Spacing";
		private const string MeshOptionsMirrorCurveTitle = "Asymetric Width Curve";
		private const string MeshOptionsWidthCurveTitle = "Width Curve";
		private const string MeshOptionsRightWidthCurveTitle = "Right Width Curve";
		private const string MeshOptionsLeftWidthCurveTitle = "Left Width Curve";
		private const string MeshOptionsUsePointsScaleTitle = "Use Points Scale";
		private const string MeshOptionsBakeMeshButtonTitle = "Bake Mesh";

		private const string MeshOptionsBakeMeshWindowTitle = "Save Spline Mesh Asset";
		private const string MeshOptionsBakeMeshWindowFolderPath = "Assets/";
		private const string MeshOptionsBakeMeshWindowFileName = "bakedSplineMesh";
		private const string MeshOptionsBakeMeshWindowFileExtension = "asset";

		private static GUIContent MeshOptionsMeshWidthFieldContent = new GUIContent(MeshOptionsMeshWidthTitle);
		private static GUIContent MeshOptionsMeshSpacingFieldContent = new GUIContent(MeshOptionsMeshSpacingTitle);
		private static GUIContent MeshOptionsAsymetricWidthCurveToggleContent = new GUIContent(MeshOptionsMirrorCurveTitle);
		private static GUIContent MeshOptionsWidthCurveContent = new GUIContent(MeshOptionsWidthCurveTitle);
		private static GUIContent MeshOptionsRightWidthCurveContent = new GUIContent(MeshOptionsRightWidthCurveTitle);
		private static GUIContent MeshOptionsLeftWidthCurveContent = new GUIContent(MeshOptionsLeftWidthCurveTitle);
		private static GUIContent MeshOptionsUsePointsScaleToggleContent= new GUIContent(MeshOptionsUsePointsScaleTitle);
		private static GUIContent MeshOptionsBakeMeshButtonContent = new GUIContent(MeshOptionsBakeMeshButtonTitle);

		#endregion

		#region Private Fields

		private GUIStyle groupsStyle;

		#endregion

		private void InitializeStyles()
		{
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
		}

	}

}
