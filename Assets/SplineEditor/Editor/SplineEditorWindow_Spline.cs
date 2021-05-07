using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private float previousLoopLength = 0f;
		private bool previousLoopState = false;
		private Transform customTransform = null;

		private bool isSplineSectionFolded = true;

		private void DrawSplineGroup()
		{
			var prevEnabled = GUI.enabled;
			var prevColor = GUI.color;

			isSplineSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isSplineSectionFolded, SplineOptionsTitle);
			GUI.enabled = isSplineEditorEnabled;
			if (isSplineSectionFolded)
			{
				DrawSplineStatsSection();
				DrawSplineButtons();
				DrawCastButtons();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.color = prevColor;
			GUI.enabled = prevEnabled;
		}


		private void DrawSplineStatsSection()
		{
			GUILayout.BeginHorizontal(groupsStyle);
			GUILayout.BeginVertical();

			GUILayout.Space(5);
			DrawLoopToggle();
			DrawLengthField();
			GUILayout.Space(5);

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

		}

		private void DrawLoopToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var prevLoopState = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.IsLoop : previousLoopState;
			GUILayout.Label(LoopToggleFieldContent);
			var nextLoopState = GUILayout.Toggle(prevLoopState, string.Empty);
			if (nextLoopState != prevLoopState)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Set Looped");
				SplineEditor.CurrentSpline.IsLoop = nextLoopState;
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			previousLoopState = nextLoopState;
		}

		private void DrawLengthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;
			var currentLength = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.Length : previousLoopLength;
			GUILayout.Label(LengthSplineFieldContent);
			GUILayout.Label(currentLength.ToString());

			previousLoopLength = currentLength;
			GUI.enabled = prevEnabled;

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawSplineButtons()
		{
			var isGroupEnabled = GUI.enabled;
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
