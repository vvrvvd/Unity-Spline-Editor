using UnityEditor;
using UnityEngine;
using static SplineMe.BezierSpline;

namespace SplineMe.Editor
{
    public partial class BezierSplineEditor : UnityEditor.Editor
    {

		#region Inspector GUI

		private void DrawInspectorGUI()
		{
			var prevEnabled = GUI.enabled;

			GUI.enabled = false;
			EditorGUILayout.FloatField("Length", CurrentSpline.Length);

			GUI.enabled = HasMoreThanOneCurve;
			EditorGUI.BeginChangeCheck();
			bool loop = EditorGUILayout.Toggle("Loop", CurrentSpline.IsLoop);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(CurrentSpline, "Toggle Loop");
				CurrentSpline.IsLoop = loop;

				if (CurrentSpline.IsLoop)
				{
					ToggleDrawCurveMode(false);
					EditorUtility.SetDirty(CurrentSpline);
				}
			}

			GUI.enabled = prevEnabled;

			if (SelectedPointIndex >= CurrentSpline.PointsCount)
			{
				SelectIndex(CurrentSpline.PointsCount - 1);
			}

			if (IsAnyPointSelected)
			{
				DrawSelectedPointInspector();
			}

			if (GUILayout.Button("Cast Curve Points"))
			{
				CastCurve();
				EditorUtility.SetDirty(CurrentSpline);
			}

			GUI.enabled = IsAnyPointSelected;
			if (GUILayout.Button("Add Mid Curve"))
			{
				AddMidCurve();
				EditorUtility.SetDirty(CurrentSpline);
			}
			GUI.enabled = prevEnabled;

			if (GUILayout.Button("Factor Curve"))
			{
				FactorCurve();
				EditorUtility.SetDirty(CurrentSpline);
			}

			GUI.enabled = CanBeSimplified;
			if (GUILayout.Button("Simplify Curve"))
			{
				SimplifyCurve();
				EditorUtility.SetDirty(CurrentSpline);
			}

			GUI.enabled = prevEnabled;
		}

		private void DrawSelectedPointInspector()
		{
			GUILayout.Label("Selected Point");
			EditorGUI.BeginChangeCheck();
			Vector3 point = EditorGUILayout.Vector3Field("Position", CurrentSpline.Points[SelectedPointIndex].position);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(CurrentSpline, "Move Point");
				CurrentSpline.UpdatePoint(SelectedPointIndex, point);
				EditorUtility.SetDirty(CurrentSpline);
			}

			EditorGUI.BeginChangeCheck();
			BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", CurrentSpline.GetControlPointMode(SelectedPointIndex));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(CurrentSpline, "Change Point Mode");
				CurrentSpline.SetControlPointMode(SelectedPointIndex, mode);
				EditorUtility.SetDirty(CurrentSpline);
			}
		}

		#endregion

	}

}
