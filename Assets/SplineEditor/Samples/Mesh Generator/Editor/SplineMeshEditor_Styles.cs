// <copyright file="SplineMeshEditor_Styles.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.MeshGenerator.Editor
{
	/// <summary>
	/// Class providing custom editor to SplineMesh component.
	/// Partial class providing titles, buttons and other GUI styles for custom editor.
	/// </summary>
	public partial class SplineMeshEditor : UnityEditor.Editor
	{
		private const string MeshOptionsGroupTitle = "Mesh";
		private const string MeshOptionsMeshWidthTitle = "Mesh Width";
		private const string MeshOptionsMeshSpacingTitle = "Mesh Spacing";
		private const string MeshOptionsUsePointsScaleTitle = "Use Points Scale";
		private const string MeshOptionsBakeMeshButtonTitle = "Bake Mesh";

		private const string MeshOptionsBakeMeshWindowTitle = "Save Mesh Asset";
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

		private static readonly GUIContent MeshOptionsMeshWidthFieldContent = new GUIContent(MeshOptionsMeshWidthTitle);
		private static readonly GUIContent MeshOptionsMeshSpacingFieldContent = new GUIContent(MeshOptionsMeshSpacingTitle);
		private static readonly GUIContent MeshOptionsUsePointsScaleToggleContent = new GUIContent(MeshOptionsUsePointsScaleTitle);
		private static readonly GUIContent MeshOptionsBakeMeshButtonContent = new GUIContent(MeshOptionsBakeMeshButtonTitle);

		private static readonly GUIContent CurveOptionsAsymetricWidthCurveToggleContent = new GUIContent(CurveOptionsAsymetrictWidthCurveTitle);
		private static readonly GUIContent CurveOptionsWidthCurveContent = new GUIContent(CurveOptionsWidthCurveTitle);
		private static readonly GUIContent CurveOptionsRightWidthCurveContent = new GUIContent(CurveOptionsRightWidthCurveTitle);
		private static readonly GUIContent CurveOptionsLeftWidthCurveContent = new GUIContent(CurveOptionsLeftWidthCurveTitle);

		private static readonly GUIContent GuiOptionsDrawPointsToggleContent = new GUIContent(GuiOptionsDrawPointsTitle);
		private static readonly GUIContent GuiOptionsDrawNormalToggleContent = new GUIContent(GuiOptionsDrawNormalsTitle);

		private static readonly GUIContent UvOptionsUvModeDropdownContent = new GUIContent(UvOptionsUvModeTitle);
		private static readonly GUIContent UvOptionsMirrorUvToggleContent = new GUIContent(UvOptionsMirrorUvTitle);
		private static readonly GUIContent UvOptionsHideDebugViewButtonContent = new GUIContent(UvOptionsHideDebugUvViewButtonTitle);
		private static readonly GUIContent UvOptionsShowDebugViewButtonContent = new GUIContent(UvOptionsShowDebugUvViewButtonTitle);

		private static readonly GUILayoutOption ButtonHeight = GUILayout.Height(30);

		private GUIStyle groupsStyle;
		private GUIStyle buttonStyle;

		private void InitializeStyles()
		{
			groupsStyle = new GUIStyle(EditorStyles.helpBox);
			buttonStyle = MeshEditorConfiguration.GuiSkin.FindStyle("button");
		}
	}
}
