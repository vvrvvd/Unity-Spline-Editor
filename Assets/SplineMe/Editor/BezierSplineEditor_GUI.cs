using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Const Fields

		private const string ShowMainHandleKey = "ShowMainHandle";
		private const string ShowPointsHandlesKey = "ShowPointsHandles";
		private const string ShowSegmentsHandleKey = "ShowSegmentsHandle";
		private const string ShowDirectionsHandleKey = "ShowDirectionsHandle";
		private const string DrawCurveSmoothAcuteAngles = "DrawCurveSmoothAcuteAngles";

		#endregion

		#region Static Fields

		private static Tool savedTool = Tool.None;

		#endregion

		#region Private Fields

		private bool showMainTransformGizmo;
		private bool showPointsHandles;
		private bool showSegmentsPoints;
		private bool showDirectionsLines;
		private bool drawCurveSmoothAcuteAngles;

		#endregion

		#region Initialize GUI

		private void InitializeGUI()
		{
			showMainTransformGizmo = EditorPrefs.GetBool(ShowMainHandleKey, true);
			showPointsHandles = EditorPrefs.GetBool(ShowPointsHandlesKey, true);
			showSegmentsPoints = EditorPrefs.GetBool(ShowSegmentsHandleKey, false);
			showDirectionsLines = EditorPrefs.GetBool(ShowDirectionsHandleKey, false);
			drawCurveSmoothAcuteAngles = EditorPrefs.GetBool(DrawCurveSmoothAcuteAngles, false);

			if (showMainTransformGizmo)
			{
				ShowTools();
			}
			else
			{
				HideTools();
			}

		}

		private void ReleaseGUI()
		{
			if (showMainTransformGizmo)
			{
				ShowTools();
			}
			else
			{
				HideTools();
			}
		}

		#endregion

		#region Draw GUI

		private void DrawGUI()
		{
			if(currentEditor==null || currentSpline == null)
			{
				return;
			}

			if (showMainTransformGizmo)
			{
				savedTool = Tools.current;
			}
			else if (Tools.current != Tool.None)
			{
				HideTools();
			}

			Handles.BeginGUI();

			GUILayout.BeginArea(new Rect(20, 20, 180, 150));
			var rect = EditorGUILayout.BeginVertical();
			GUI.color = Color.yellow;
			GUI.Box(rect, GUIContent.none);

			GUI.color = Color.white;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Spline Editor");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			DrawToggles();
			DrawButtons();

			EditorGUILayout.EndVertical();
			GUILayout.EndArea();
			Handles.EndGUI();
		}

		private void DrawToggles()
		{
			GUILayout.BeginVertical();
			GUI.backgroundColor = Color.white;

			var prevValue = showMainTransformGizmo;
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

				showMainTransformGizmo = newValue;
				EditorPrefs.SetBool(ShowMainHandleKey, newValue);
			}

			prevValue = showSegmentsPoints;
			newValue = GUILayout.Toggle(prevValue, "Show Segments");
			if (newValue != prevValue)
			{
				showSegmentsPoints = newValue;
				EditorPrefs.SetBool(ShowSegmentsHandleKey, newValue);
			}

			prevValue = showPointsHandles;
			newValue = GUILayout.Toggle(prevValue, "Show Points");
			if (newValue != prevValue)
			{
				if (!newValue)
				{
					SelectIndex(-1);
				}

				showPointsHandles = newValue;
				EditorPrefs.SetBool(ShowPointsHandlesKey, newValue);
			}

			prevValue = showDirectionsLines;
			newValue = GUILayout.Toggle(prevValue, "Show Directions");
			if (newValue != prevValue)
			{
				showDirectionsLines = newValue;
				EditorPrefs.SetBool(ShowDirectionsHandleKey, newValue);
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Draw Curve");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			prevValue = drawCurveSmoothAcuteAngles;
			newValue = GUILayout.Toggle(prevValue, "Draw Curve Smooth Acute Angles");
			if (newValue != prevValue)
			{
				drawCurveSmoothAcuteAngles = newValue;
				EditorPrefs.SetBool(DrawCurveSmoothAcuteAngles, newValue);
			}

			GUILayout.EndVertical();
		}

		private void DrawButtons()
		{
			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.red;
			var prevEnabled = GUI.enabled;

			GUI.enabled = CanNewCurveBeAdded;
			if (GUILayout.Button("Add Curve"))
			{
				AddCurve();
			}

			GUI.enabled = CanSelectedCurveBeRemoved && !isCurveDrawerMode;
			if (GUILayout.Button("Remove Curve"))
			{
				RemoveSelectedCurve();
			}

			GUI.enabled = prevEnabled;

			GUILayout.EndHorizontal();
		}

		#endregion

		#region Static Methods

		private static void ShowTools()
		{
			if (savedTool == Tool.None)
			{
				savedTool = Tool.Move;
			}

			Tools.current = savedTool;
		}

		private static void HideTools()
		{
			savedTool = Tools.current;
			Tools.current = Tool.None;
		}

		#endregion

	}

}
