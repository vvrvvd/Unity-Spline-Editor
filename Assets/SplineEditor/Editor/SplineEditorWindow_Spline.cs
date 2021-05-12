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
			DrawSplineTogglesInspector();
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

		private bool previousDrawPointsState = true;
		private bool previousShowTransformHandleState = true;
		private bool previousAlwaysDrawOnSceneState = false;

		private void DrawSplineTogglesInspector()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousDrawPoints = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.drawPoints : previousDrawPointsState;
			GUILayout.Label(DrawPointsFieldContent);
			var nextLoopState = GUILayout.Toggle(previousDrawPoints, string.Empty);
			if (nextLoopState != previousDrawPoints)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Toggle Draw Points");
				SplineEditor.CurrentSpline.drawPoints = nextLoopState;
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}
			previousDrawPointsState = nextLoopState;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousShowTransformHandle = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.showTransformHandle : previousShowTransformHandleState;
			GUILayout.Label(ShowTransformHandleFieldContent);
			nextLoopState = GUILayout.Toggle(previousShowTransformHandle, string.Empty);
			if (nextLoopState != previousShowTransformHandle)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Toggle Show Transform Handle");
				SplineEditor.CurrentSpline.showTransformHandle = nextLoopState;
				if (!nextLoopState)
				{
					SplineEditor.HideTools();
				}
				else
				{
					SplineEditor.ShowTools();
				}
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}
			previousShowTransformHandleState = nextLoopState;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousAlwaysDrawOnScene = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.alwaysDrawSplineOnScene : previousAlwaysDrawOnSceneState;
			GUILayout.Label(AlwaysDrawOnSceneFieldContent);
			nextLoopState = GUILayout.Toggle(previousAlwaysDrawOnScene, string.Empty);
			if (nextLoopState != previousAlwaysDrawOnScene)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Toggle Always Draw On Scene");
				SplineEditor.CurrentSpline.alwaysDrawSplineOnScene = nextLoopState;
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}

			previousAlwaysDrawOnSceneState = nextLoopState;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
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
