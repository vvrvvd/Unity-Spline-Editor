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
			DrawStandardScriptReferenceField();
			DrawLengthField();
		}

		private void DrawStandardScriptReferenceField()
		{
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((BezierSpline)target), typeof(BezierSpline), false);
			GUI.enabled = true;
		}

		private void DrawLengthField()
		{
			var prevEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.FloatField("Length", CurrentSpline.GetLinearLength(precision: 0.001f, useWorldScale: true));

			GUI.enabled = prevEnabled;
		}

		#endregion

	}

}
