using System;
using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{

	public struct LineEditorState
	{
		public string title;

		public Action AddPointAction;
		public Action RemovePointAction;

		public bool isAnyPointSelected;
		public bool showMainTransformGizmo;
	}

	public class SplineMeTools
	{

		public static Color LineStartPointColor => Color.green;
		public static Color LineMidPointColor => Color.white;
		public static Color LineEndPointColor => Color.red;
		public static Color LineColor => Color.white;


		public const float HandlePointSize = 0.04f;
		public const float PickPointSize = 0.06f;
		public const float DirectionScale = 0.5f;

		private const string ShowMainHandleKey = "ShowMainHandle";
		
		private static Tool savedTool = Tool.None;

		#region GUI

		public static void InitializeGUI(ref LineEditorState state)
		{
			state.showMainTransformGizmo = EditorPrefs.GetBool(ShowMainHandleKey, true);

			if(state.showMainTransformGizmo)
			{
				ShowTools();
			} else
			{
				HideTools();
			}

		}

		public static void ReleaseGUI(ref LineEditorState state)
		{
			state.AddPointAction = null;
			state.RemovePointAction = null;

			if (state.showMainTransformGizmo)
			{
				ShowTools();
			}
			else
			{
				HideTools();
			}

		}

		public static void DrawGUI(ref LineEditorState state)
		{
			Handles.BeginGUI();

			GUILayout.BeginArea(new Rect(20, 20, 180, 80));
			var rect = EditorGUILayout.BeginVertical();
			GUI.color = Color.yellow;
			GUI.Box(rect, GUIContent.none);

			GUI.color = Color.white;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Spline Me");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			DrawToggles(ref state);
			DrawButtons(ref state);

			EditorGUILayout.EndVertical();
			GUILayout.EndArea();
			Handles.EndGUI();
		}

		private static void DrawToggles(ref LineEditorState state)
		{
			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.white;

			var prevValue = state.showMainTransformGizmo;
			var newValue = GUILayout.Toggle(prevValue, "Show main handle");
			if (newValue != prevValue)
			{
				if (newValue)
				{
					ShowTools();
				}
				else
				{
					HideTools();
				}

				state.showMainTransformGizmo = newValue;
				EditorPrefs.SetBool(ShowMainHandleKey, newValue);
			}

			GUILayout.EndHorizontal();
		}

		private static void DrawButtons(ref LineEditorState state)
		{
			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.red;

			if (GUILayout.Button("Add Point"))
			{
				state.AddPointAction();
			}

			var prevEnabled = GUI.enabled;
			GUI.enabled = state.isAnyPointSelected;
			if (GUILayout.Button("Remove Point"))
			{
				state.RemovePointAction();
			}

			GUI.enabled = prevEnabled;

			GUILayout.EndHorizontal();
		}

		#endregion

		#region Show/Hide Tools

		public static void ShowTools()
		{
			if (savedTool == Tool.None)
			{
				savedTool = Tool.Move;
			}

			Tools.current = savedTool;
		}

		public static void HideTools()
		{
			savedTool = Tools.current;
			Tools.current = Tool.None;
		}

		#endregion

	}

}
