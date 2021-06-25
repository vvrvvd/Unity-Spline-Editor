using UnityEditor;
using UnityEngine;
using BezierSplineEditor = SplineEditor.Editor.SplineEditor;

namespace SplineEditor.MeshGenerator.Editor
{

	public partial class SplineMeshEditor : UnityEditor.Editor
	{

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

		private static GUIContent UvOptionsUvModeDropdownContent = new GUIContent(UvOptionsUvModeTitle);
		private static GUIContent UvOptionsMirrorUvToggleContent = new GUIContent(UvOptionsMirrorUvTitle);

		private static GUILayoutOption MaxDropdownWidth = GUILayout.MaxWidth(100);
		
		#endregion

		#region Mesh Options Styles

		private const string MeshOptionsGroupTitle = "Mesh";
		private const string MeshOptionsMeshWidthTitle = "Mesh Width";
		private const string MeshOptionsMeshSpacingTitle = "Mesh Spacing";
		private const string MeshOptionsMirrorCurveTitle = "Symetric Width Curve";
		private const string MeshOptionsWidthCurveTitle = "Width Curve";
		private const string MeshOptionsRightWidthCurveTitle = "Right Width Curve";
		private const string MeshOptionsLeftWidthCurveTitle = "Left Width Curve";

		private static GUIContent MeshOptionsMeshWidthFieldContent = new GUIContent(MeshOptionsMeshWidthTitle);
		private static GUIContent MeshOptionsMeshSpacingFieldContent = new GUIContent(MeshOptionsMeshSpacingTitle);
		private static GUIContent MeshOptionsMirrorCurveToggleContent = new GUIContent(MeshOptionsMirrorCurveTitle);
		private static GUIContent MeshOptionsWidthCurveContent = new GUIContent(MeshOptionsWidthCurveTitle);
		private static GUIContent MeshOptionsRightWidthCurveContent = new GUIContent(MeshOptionsRightWidthCurveTitle);
		private static GUIContent MeshOptionsLeftWidthCurveContent = new GUIContent(MeshOptionsLeftWidthCurveTitle);

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
