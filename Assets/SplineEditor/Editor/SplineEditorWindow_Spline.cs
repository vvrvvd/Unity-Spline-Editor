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
			EditorGUI.indentLevel++;

			if (editorWindowState.IsSplineSectionFolded)
			{
				DrawSplineStatsSection();
				DrawSplineButtons();
				DrawCastButtons();
			}

			EditorGUI.indentLevel--;
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
			var previousDrawPoints = editorState.DrawPoints;
			var nextLoopState = EditorGUILayout.Toggle(DrawPointsFieldContent, previousDrawPoints);
			if (nextLoopState != previousDrawPoints)
			{
				Undo.RecordObject(editorState, "Toggle Draw Points");
				editorState.DrawPoints = nextLoopState;
				repaintScene = true;
			}
			GUILayout.EndHorizontal();
		}

		private void DrawDrawSplineToggle()
		{
			GUILayout.BeginHorizontal();
			var previousDrawSpline = editorState.DrawSpline;
			var nextLoopState = EditorGUILayout.Toggle(DrawSplineFieldContent, previousDrawSpline);
			if (nextLoopState != previousDrawSpline)
			{
				Undo.RecordObject(editorState, "Toggle Draw Spline");
				editorState.DrawSpline = nextLoopState;
				repaintScene = true;
			}
			GUILayout.EndHorizontal();
		}


		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			var previousDrawNormals = editorState.DrawNormals;
			var nextDrawNormals = EditorGUILayout.Toggle(DrawNormalsToggleFieldContent, previousDrawNormals);
			if (nextDrawNormals != previousDrawNormals)
			{
				Undo.RecordObject(editorState, "Toggle Draw Normals");
				editorState.DrawNormals = nextDrawNormals;
				repaintScene = true;
			}
			GUILayout.EndHorizontal();
		}

		private void DrawShowMainTransformHandleToggle()
		{
			GUILayout.BeginHorizontal();
			var previousShowTransformHandle = editorState.ShowTransformHandle;
			var nextLoopState = EditorGUILayout.Toggle(ShowTransformHandleFieldContent, previousShowTransformHandle);
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
			GUILayout.EndHorizontal();
		}

		private void DrawAlwaysOnSceneToggle()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= editorState.DrawSpline;

			GUILayout.BeginHorizontal();
			var previousAlwaysDrawOnScene = editorState.AlwaysDrawSplineOnScene;
			var nextLoopState = EditorGUILayout.Toggle(AlwaysDrawOnSceneFieldContent, previousAlwaysDrawOnScene);
			if (nextLoopState != previousAlwaysDrawOnScene)
			{
				Undo.RecordObject(editorState, "Toggle Always Draw On Scene");
				editorState.AlwaysDrawSplineOnScene = nextLoopState;
				repaintScene = true;
			}

			GUILayout.EndHorizontal();

			GUI.enabled = prevEnabled;
		}

		private void DrawLengthField()
		{
			GUILayout.BeginHorizontal();

			var prevEnabled = GUI.enabled;
			GUI.enabled = false;
			var currentLength = editorState.CurrentSpline != null ? editorState.CurrentSpline.GetLinearLength(useWorldScale: true) : editorWindowState.PreviousSplineLength;
			EditorGUILayout.FloatField(LengthSplineFieldContent, currentLength);

			editorWindowState.PreviousSplineLength = currentLength;
			GUI.enabled = prevEnabled;

			GUILayout.EndHorizontal();
		}

		private void DrawSplineButtons()
		{
			var isGroupEnabled = GUI.enabled;
			GUILayout.BeginVertical(groupsStyle);
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			GUI.enabled = isGroupEnabled;

			if (GUILayout.Button(CloseLoopButtonContent, buttonStyle, ToolsButtonsMinWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleToggleCloseLoop();
				repaintScene = true;
			}

			if (GUILayout.Button(FactorSplineButtonContent, buttonStyle, ToolsButtonsMinWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleFactorSpline();
				repaintScene = true;
			}

			GUI.enabled = isGroupEnabled && editorState.CanSplineBeSimplified;
			if (GUILayout.Button(SimplifyButtonContent, buttonStyle, ToolsButtonsMinWidth, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleSimplifySpline();
				repaintScene = true;
			}
			GUILayout.Space(15);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.EndVertical();
			GUI.enabled = isGroupEnabled;
		}

		private void DrawCastButtons()
		{
			GUILayout.BeginVertical(groupsStyle);

			GUILayout.Space(10);

			DrawCustomTransformField();

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);

			if (GUILayout.Button(CastSplineContent, buttonStyle, ToolsButtonsHeight))
			{
				var referenceTransform = editorWindowState.CustomTransform == null ? editorState.CurrentSpline.transform : editorWindowState.CustomTransform;
				var castDirection = -referenceTransform.up;
				SplineEditor.ScheduleCastSpline(castDirection);
				repaintScene = true;
			}

			if (GUILayout.Button(CastSplineToCameraContent, buttonStyle, ToolsButtonsHeight))
			{
				SplineEditor.ScheduleCastSplineToCameraView();
				repaintScene = true;
			}

			GUILayout.Space(15);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);

			GUILayout.EndVertical();
		}

		private void DrawCustomTransformField()
		{
			GUILayout.BeginHorizontal();

			var prevState = editorWindowState.CustomTransform;
			var nextState = EditorGUILayout.ObjectField(CastTransformFieldContent, editorWindowState.CustomTransform, typeof(Transform), true) as Transform;
			if (nextState != prevState)
			{
				Undo.RecordObject(editorWindowState, "Change Custom Cast Transform");
				editorWindowState.CustomTransform = nextState;
			}

            GUILayout.Space(15);
			GUILayout.EndHorizontal();

		}


	}
}
