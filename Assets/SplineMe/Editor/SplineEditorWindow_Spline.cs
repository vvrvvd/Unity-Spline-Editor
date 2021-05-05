using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private Transform customTransform = null;

		private void DrawSplineGroup()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled = isSplineEditorEnabled;
			var prevColor = GUI.color;

			GUILayout.BeginVertical();
			DrawSplineButtons();
			DrawCastButtons();
			GUILayout.EndVertical();

			GUI.color = prevColor;
			GUI.enabled = prevEnabled;
		}

		private void DrawSplineButtons()
		{
			var isGroupEnabled = GUI.enabled;
			GUILayout.Label(SplineOptionsTitle);
			GUILayout.BeginVertical(groupsStyle);
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(FactorSplineButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleFactorSpline();
				repaintScene = true;
			}

			GUI.enabled = isGroupEnabled && SplineEditor.CanSplineBeSimplified;
			if (GUILayout.Button(SimplifyButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleSimplifySpline();
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.EndVertical();
			GUI.enabled = isGroupEnabled;
		}

		private void DrawCastButtons()
		{
			GUILayout.BeginVertical(groupsStyle);
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(CastSplineContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				var referenceTransform = customTransform == null ? SplineEditor.CurrentSpline.transform : customTransform;
				var castDirection = -referenceTransform.up;
				SplineEditor.ScheduleCastSpline(castDirection);
				repaintScene = true;
			}

			if (GUILayout.Button(CastSplineToCameraContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleCastSplineToCameraView();
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(CastTransformFieldContent);
			customTransform = EditorGUILayout.ObjectField(customTransform, typeof(Transform), true, ToolsCustomTransformFieldWidth) as Transform;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.EndVertical();
		}


	}
}
