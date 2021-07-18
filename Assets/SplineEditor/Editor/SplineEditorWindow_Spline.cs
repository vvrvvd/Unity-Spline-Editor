using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{

	public partial class SplineEditorWindow : EditorWindow
	{

		private void DrawSplineGroup()
		{
			var prevEnabled = GUI.enabled;
			var prevColor = GUI.color;

			editorWindowState.IsSplineSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.IsSplineSectionFolded, SplineOptionsTitle);
			GUI.enabled = IsSplineEditorEnabled;
			if (editorWindowState.IsSplineSectionFolded)
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


		private void DrawSplineTogglesInspector()
		{
			DrawDrawPointsToggle();
			DrawDrawNormalsToggle();
			DrawDrawSplineToggle();
			DrawAlwaysOnSceneToggle();
			DrawShowMainTransformHandleToggle();
		}


		private void DrawDrawPointsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousDrawPoints = editorState.DrawPoints;
			GUILayout.Label(DrawPointsFieldContent);
			var nextLoopState = GUILayout.Toggle(previousDrawPoints, string.Empty);
			if (nextLoopState != previousDrawPoints)
			{
				Undo.RecordObject(editorState, "Toggle Draw Points");
				editorState.DrawPoints = nextLoopState;
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawDrawSplineToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousDrawSpline = editorState.DrawSpline;
			GUILayout.Label(DrawSplineFieldContent);
			var nextLoopState = GUILayout.Toggle(previousDrawSpline, string.Empty);
			if (nextLoopState != previousDrawSpline)
			{
				Undo.RecordObject(editorState, "Toggle Draw Spline");
				editorState.DrawSpline = nextLoopState;
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}


		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousDrawNormals = editorState.DrawNormals;
			GUILayout.Label(DrawNormalsToggleFieldContent);
			var nextDrawNormals = GUILayout.Toggle(previousDrawNormals, string.Empty);
			if (nextDrawNormals != previousDrawNormals)
			{
				Undo.RecordObject(editorState, "Toggle Draw Normals");
				editorState.DrawNormals = nextDrawNormals;
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawShowMainTransformHandleToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousShowTransformHandle = editorState.ShowTransformHandle;
			GUILayout.Label(ShowTransformHandleFieldContent);
			var nextLoopState = GUILayout.Toggle(previousShowTransformHandle, string.Empty);
			if (nextLoopState != previousShowTransformHandle)
			{
				Undo.RecordObject(editorState, "Toggle Show Transform Handle");
				editorState.ShowTransformHandle = nextLoopState;
				if (!nextLoopState)
				{
					SplineEditor.HideTools();
				}
				else
				{
					SplineEditor.ShowTools();
				}
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawAlwaysOnSceneToggle()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= editorState.DrawSpline;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousAlwaysDrawOnScene = editorState.AlwaysDrawSplineOnScene;
			GUILayout.Label(AlwaysDrawOnSceneFieldContent);
			var nextLoopState = GUILayout.Toggle(previousAlwaysDrawOnScene, string.Empty);
			if (nextLoopState != previousAlwaysDrawOnScene)
			{
				Undo.RecordObject(editorState, "Toggle Always Draw On Scene");
				editorState.AlwaysDrawSplineOnScene = nextLoopState;
				repaintScene = true;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawLengthField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;
			var currentLength = editorState.CurrentSpline != null ? editorState.CurrentSpline.GetLinearLength(useWorldScale: true) : editorWindowState.PreviousSplineLength;
			GUILayout.Label(LengthSplineFieldContent);
			GUILayout.Label(currentLength.ToString());

			editorWindowState.PreviousSplineLength = currentLength;
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

			GUI.enabled = isGroupEnabled && editorState.CanSplineBeSimplified;
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
				var referenceTransform = editorWindowState.CustomTransform == null ? editorState.CurrentSpline.transform : editorWindowState.CustomTransform;
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

			DrawCustomTransformField();

			GUILayout.Space(10);
			GUILayout.EndVertical();
		}

		private void DrawCustomTransformField()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(CastTransformFieldContent);

			var prevState = editorWindowState.CustomTransform;
			var nextState = EditorGUILayout.ObjectField(editorWindowState.CustomTransform, typeof(Transform), true, ToolsCustomTransformFieldWidth) as Transform;
			if (nextState != prevState)
			{
				Undo.RecordObject(editorWindowState, "Change Custom Cast Transform");
				editorWindowState.CustomTransform = nextState;
			}


			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

		}


	}
}
