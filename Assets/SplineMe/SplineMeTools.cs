using System;
using UnityEditor;
using UnityEngine;

namespace SplineMe
{
	public struct SplineEditorState
	{
		public string title;

		public Action AddCurveAction;
		public Action RemoveCurveAction;

		public bool isAnyCurveSelected;
		public bool isMoreThanOneCurve;
		public bool showMainTransformGizmo;
		public bool showPointsHandles;
		public bool showSegmentsPoints;
		public bool showDirectionsLines;
	}

	public class SplineMeTools
	{

		public const int CurveStepsCount = 10;
		public const float MaxRaycastDistance = 100;

		public static Color LineStartPointColor => Color.green;
		public static Color LineMidPointColor => Color.white;
		public static Color LineEndPointColor => Color.red;
		public static Color LineColor => Color.white;
		public static Color TangentLineColor => Color.grey;
		public static Color DirectionLineColor => Color.green;
		public static Color SegmentsColor => Color.blue;

		public static Color[] ModeColors = {
			Color.white,	//Free
			Color.yellow,	//Aligned
			Color.cyan		//Mirrored
		};

		public const float LineWidth = 2f;
		public const float HandlePointSize = 0.04f;
		public const float HandleSegmentSize = 0.03f;
		public const float PickPointSize = 0.06f;
		public const float DirectionScale = 0.5f;

		private const string ShowMainHandleKey = "ShowMainHandle";
		private const string ShowPointsHandlesKey = "ShowPointsHandles";
		private const string ShowSegmentsHandleKey = "ShowSegmentsHandle";
		private const string ShowDirectionsHandleKey = "ShowDirectionsHandle";
		
		private static Tool savedTool = Tool.None;

		#region GUI

		public static void InitializeGUI(ref SplineEditorState state)
		{
			state.showMainTransformGizmo = EditorPrefs.GetBool(ShowMainHandleKey, true);
			state.showPointsHandles = EditorPrefs.GetBool(ShowPointsHandlesKey, true);
			state.showSegmentsPoints = EditorPrefs.GetBool(ShowSegmentsHandleKey, true);
			state.showDirectionsLines = EditorPrefs.GetBool(ShowDirectionsHandleKey, true);

			if (state.showMainTransformGizmo)
			{
				ShowTools();
			} else
			{
				HideTools();
			}

		}

		public static void ReleaseGUI(ref SplineEditorState state)
		{
			state.AddCurveAction = null;
			state.RemoveCurveAction = null;

			if (state.showMainTransformGizmo)
			{
				ShowTools();
			}
			else
			{
				HideTools();
			}

		}

		public static void DrawGUI(ref SplineEditorState state)
		{
			Handles.BeginGUI();

			GUILayout.BeginArea(new Rect(20, 20, 180, 150));
			var rect = EditorGUILayout.BeginVertical();
			GUI.color = Color.yellow;
			GUI.Box(rect, GUIContent.none);

			GUI.color = Color.white;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(state.title);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			DrawToggles(ref state);
			DrawButtons(ref state);

			EditorGUILayout.EndVertical();
			GUILayout.EndArea();
			Handles.EndGUI();
		}

		private static void DrawToggles(ref SplineEditorState state)
		{
			GUILayout.BeginVertical();
			GUI.backgroundColor = Color.white;

			var prevValue = state.showMainTransformGizmo;
			var newValue = GUILayout.Toggle(prevValue, "Show Main Handle");
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

			prevValue = state.showSegmentsPoints;
			newValue = GUILayout.Toggle(prevValue, "Show Segments");
			if (newValue != prevValue)
			{
				state.showSegmentsPoints = newValue;
				EditorPrefs.SetBool(ShowSegmentsHandleKey, newValue);
			}

			prevValue = state.showPointsHandles;
			newValue = GUILayout.Toggle(prevValue, "Show Points");
			if (newValue != prevValue)
			{
				state.showPointsHandles = newValue;
				EditorPrefs.SetBool(ShowPointsHandlesKey, newValue);
			}

			prevValue = state.showDirectionsLines;
			newValue = GUILayout.Toggle(prevValue, "Show Directions");
			if (newValue != prevValue)
			{
				state.showDirectionsLines = newValue;
				EditorPrefs.SetBool(ShowDirectionsHandleKey, newValue);
			}


			GUILayout.EndVertical();
		}

		private static void DrawButtons(ref SplineEditorState state)
		{
			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.red;

			if (GUILayout.Button("Add Curve"))
			{
				state.AddCurveAction();
			}

			var prevEnabled = GUI.enabled;
			GUI.enabled = state.isMoreThanOneCurve;
			if (GUILayout.Button("Remove Curve"))
			{
				state.RemoveCurveAction();
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
