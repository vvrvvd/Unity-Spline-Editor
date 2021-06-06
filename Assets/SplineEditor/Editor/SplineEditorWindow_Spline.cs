using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private float previousSplineLength = 0f;
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

			DrawSplineTogglesInspector();

			GUILayout.Space(5);

			DrawLengthField();
			GUILayout.Space(5);

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

		}


		private bool previousDrawPointsState = true;
		private bool previousShowTransformHandleState = true;
		private bool previousAlwaysDrawOnSceneState = false;

		private void DrawSplineTogglesInspector()
		{
			DrawDrawPointsToggle();
			DrawDrawNormalsToggle();
			DrawShowMainTransformHandleToggle();
			DrawAlwaysOnSCeneToggle();
		}

		private void DrawDrawPointsToggle()
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
		}

		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			previousDrawNormals = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.drawNormals : previousDrawNormals;
			GUILayout.Label(DrawNormalsToggleFieldContent);
			var nextDrawNormals = GUILayout.Toggle(previousDrawNormals, string.Empty);
			if (nextDrawNormals != previousDrawNormals)
			{
				Undo.RecordObject(SplineEditor.CurrentSpline, "Toggle Draw Normals");
				SplineEditor.CurrentSpline.drawNormals = nextDrawNormals;
				EditorUtility.SetDirty(SplineEditor.CurrentSpline);
				repaintScene = true;
			}
			previousDrawNormals = nextDrawNormals;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawShowMainTransformHandleToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousShowTransformHandle = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.showTransformHandle : previousShowTransformHandleState;
			GUILayout.Label(ShowTransformHandleFieldContent);
			var nextLoopState = GUILayout.Toggle(previousShowTransformHandle, string.Empty);
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
		}

		private void DrawAlwaysOnSCeneToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousAlwaysDrawOnScene = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.alwaysDrawSplineOnScene : previousAlwaysDrawOnSceneState;
			GUILayout.Label(AlwaysDrawOnSceneFieldContent);
			var nextLoopState = GUILayout.Toggle(previousAlwaysDrawOnScene, string.Empty);
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
			var currentLength = SplineEditor.CurrentSpline != null ? SplineEditor.CurrentSpline.GetLinearLength(useWorldScale: true) : previousSplineLength;
			GUILayout.Label(LengthSplineFieldContent);
			GUILayout.Label(currentLength.ToString());

			previousSplineLength = currentLength;
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
			GUI.enabled = isGroupEnabled;

			if (GUILayout.Button(CloseLoopButtonContent, buttonStyle, ToolsButtonsWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleToggleCloseLoop();
				repaintScene = true;
			}

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
