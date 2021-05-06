using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Inspector GUI

		private void DrawInspectorGUI()
		{
			var prevEnabled = GUI.enabled;

			GUI.enabled = false;
			EditorGUILayout.FloatField("Length", CurrentSpline.Length);

			GUI.enabled = prevEnabled;

			if (SelectedPointIndex >= CurrentSpline.PointsCount)
			{
				SelectIndex(CurrentSpline.PointsCount - 1);
			}

			if (IsAnyPointSelected)
			{
				DrawSelectedPointInspector();
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
				wasSplineModified = true;
			}

			EditorGUI.BeginChangeCheck();
			BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", CurrentSpline.GetControlPointMode(SelectedPointIndex));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(CurrentSpline, "Change Point Mode");
				CurrentSpline.SetControlPointMode(SelectedPointIndex, mode);
				EditorUtility.SetDirty(CurrentSpline);

				wasSplineModified = true;
			}
		}

		#endregion

	}

}
