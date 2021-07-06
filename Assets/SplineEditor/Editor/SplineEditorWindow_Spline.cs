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

			editorWindowState.isSplineSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(editorWindowState.isSplineSectionFolded, SplineOptionsTitle);
			GUI.enabled = isSplineEditorEnabled;
			if (editorWindowState.isSplineSectionFolded)
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
			var previousDrawPoints = editorState.drawPoints;
			GUILayout.Label(DrawPointsFieldContent);
			var nextLoopState = GUILayout.Toggle(previousDrawPoints, string.Empty);
			if (nextLoopState != previousDrawPoints)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Draw Points");
				editorState.drawPoints = nextLoopState;
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawDrawSplineToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousDrawSpline = editorState.drawSpline;
			GUILayout.Label(DrawSplineFieldContent);
			var nextLoopState = GUILayout.Toggle(previousDrawSpline, string.Empty);
			if (nextLoopState != previousDrawSpline)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Draw Spline");
				editorState.drawSpline = nextLoopState;
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}


		private void DrawDrawNormalsToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousDrawNormals = editorState.drawNormals;
			GUILayout.Label(DrawNormalsToggleFieldContent);
			var nextDrawNormals = GUILayout.Toggle(previousDrawNormals, string.Empty);
			if (nextDrawNormals != previousDrawNormals)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Draw Normals");
				editorState.drawNormals = nextDrawNormals;
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawShowMainTransformHandleToggle()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousShowTransformHandle = editorState.showTransformHandle;
			GUILayout.Label(ShowTransformHandleFieldContent);
			var nextLoopState = GUILayout.Toggle(previousShowTransformHandle, string.Empty);
			if (nextLoopState != previousShowTransformHandle)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Show Transform Handle");
				editorState.showTransformHandle = nextLoopState;
				if (!nextLoopState)
				{
					SplineEditor.HideTools();
				}
				else
				{
					SplineEditor.ShowTools();
				}
				EditorUtility.SetDirty(editorState.CurrentSpline);
				repaintScene = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawAlwaysOnSceneToggle()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled &= editorState.drawSpline;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var previousAlwaysDrawOnScene = editorState.alwaysDrawSplineOnScene;
			GUILayout.Label(AlwaysDrawOnSceneFieldContent);
			var nextLoopState = GUILayout.Toggle(previousAlwaysDrawOnScene, string.Empty);
			if (nextLoopState != previousAlwaysDrawOnScene)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Always Draw On Scene");
				editorState.alwaysDrawSplineOnScene = nextLoopState;
				EditorUtility.SetDirty(editorState.CurrentSpline);
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
			var currentLength = editorState.CurrentSpline != null ? editorState.CurrentSpline.GetLinearLength(useWorldScale: true) : editorWindowState.previousSplineLength;
			GUILayout.Label(LengthSplineFieldContent);
			GUILayout.Label(currentLength.ToString());

			editorWindowState.previousSplineLength = currentLength;
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
				var referenceTransform = editorWindowState.customTransform == null ? editorState.CurrentSpline.transform : editorWindowState.customTransform;
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
			editorWindowState.customTransform = EditorGUILayout.ObjectField(editorWindowState.customTransform, typeof(Transform), true, ToolsCustomTransformFieldWidth) as Transform;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.EndVertical();
		}


	}
}
