// <copyright file="LineRendererSplineEditor_Inspector.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Partial class of LineRendererSplinEditor for LineRendererSpline custom inspector GUI.
	/// </summary>
	[CustomEditor(typeof(LineRendererSpline))]
	public partial class LineRendererSplineEditor : UnityEditor.Editor
	{
		private const string UseEvenlySpacedPointsToggleTitle = "Use Evenly Spaced Points";

		private LineRendererSpline lineRendererSpline;

		/// <summary>
		/// Draws custom inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			var prevEnabled = GUI.enabled;

			EditorGUILayout.BeginVertical();

			DrawStandardScriptReferenceField();
			DrawUseEvenlySpacedPointsToggle();

			GUI.enabled = lineRendererSpline.UseEvenlySpacedPoints;
			DrawSpacingField();
			GUI.enabled = !lineRendererSpline.UseEvenlySpacedPoints;
			DrawSegmentsCountField();

			EditorGUILayout.EndVertical();

			GUI.enabled = prevEnabled;
		}

		private void DrawStandardScriptReferenceField()
		{
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((LineRendererSpline)target), typeof(LineRendererSpline), false);
			GUI.enabled = true;
		}

		private void DrawUseEvenlySpacedPointsToggle()
		{
			EditorGUI.BeginChangeCheck();
			var toggleState = EditorGUILayout.Toggle(UseEvenlySpacedPointsToggleTitle, lineRendererSpline.UseEvenlySpacedPoints);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(lineRendererSpline, "Toggle Use Evenly Spaced Points Toggle");
				Undo.RecordObject(lineRendererSpline.LineRenderer, "Toggle Use Evenly Spaced Points Toggle");
				lineRendererSpline.UseEvenlySpacedPoints = toggleState;
			}
		}

		private void DrawSpacingField()
		{
			EditorGUI.BeginChangeCheck();
			var newSpacingValue = EditorGUILayout.FloatField("Points Spacing", lineRendererSpline.PointsSpacing);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(lineRendererSpline, "Change Points Spacing");
				Undo.RecordObject(lineRendererSpline.LineRenderer, "Change Points Spacing");
				lineRendererSpline.PointsSpacing = newSpacingValue;
			}
		}

		private void DrawSegmentsCountField()
		{
			EditorGUI.BeginChangeCheck();
			var newSegmentsCount = EditorGUILayout.IntField("Segments Count", lineRendererSpline.SegmentsCount);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(lineRendererSpline, "Change Segments Count");
				Undo.RecordObject(lineRendererSpline.LineRenderer, "Change Segments Count");
				lineRendererSpline.SegmentsCount = newSegmentsCount;
			}
		}

		private void OnEnable()
		{
			lineRendererSpline = target as LineRendererSpline;
		}
	}
}
