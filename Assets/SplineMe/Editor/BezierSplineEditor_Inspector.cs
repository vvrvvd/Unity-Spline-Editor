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
			EditorGUILayout.FloatField("Length", currentSpline.Length);

			GUI.enabled = HasMoreThanOneCurve;
			EditorGUI.BeginChangeCheck();
			bool loop = EditorGUILayout.Toggle("Loop", currentSpline.IsLoop);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Toggle Loop");
				currentSpline.IsLoop = loop;

				if (currentSpline.IsLoop)
				{
					ToggleDrawCurveMode(false);
					EditorUtility.SetDirty(currentSpline);
				}
			}

			GUI.enabled = prevEnabled;

			if (selectedPointIndex >= currentSpline.PointsCount)
			{
				SelectIndex(currentSpline.PointsCount - 1);
			}

			if (IsAnyPointSelected)
			{
				DrawSelectedPointInspector();
			}

			if (GUILayout.Button("Cast Curve Points"))
			{
				CastCurve();
				EditorUtility.SetDirty(currentSpline);
			}

			GUI.enabled = IsAnyPointSelected;
			if (GUILayout.Button("Add Mid Curve"))
			{
				AddMidCurve();
				EditorUtility.SetDirty(currentSpline);
			}
			GUI.enabled = prevEnabled;

			if (GUILayout.Button("Factor Curve"))
			{
				FactorCurve();
				EditorUtility.SetDirty(currentSpline);
			}

			GUI.enabled = CanBeSimplified;
			if (GUILayout.Button("Simplify Curve"))
			{
				SimplifyCurve();
				EditorUtility.SetDirty(currentSpline);
			}

			GUI.enabled = prevEnabled;
		}

		private void DrawSelectedPointInspector()
		{
			GUILayout.Label("Selected Point");
			EditorGUI.BeginChangeCheck();
			Vector3 point = EditorGUILayout.Vector3Field("Position", currentSpline.Points[selectedPointIndex].position);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Move Point");
				currentSpline.UpdatePoint(selectedPointIndex, point);
				EditorUtility.SetDirty(currentSpline);
			}

			EditorGUI.BeginChangeCheck();
			BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", currentSpline.GetControlPointMode(selectedPointIndex));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(currentSpline, "Change Point Mode");
				currentSpline.SetControlPointMode(selectedPointIndex, mode);
				EditorUtility.SetDirty(currentSpline);
			}
		}

		#endregion

	}

}
