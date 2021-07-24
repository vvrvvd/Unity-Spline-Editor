// <copyright file="SplineEditor_Inspector.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// Partial class providing custom inspector for BezierSpline.
	/// </summary>
	public partial class SplineEditor : UnityEditor.Editor
	{
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

			EditorGUILayout.FloatField("Length", EditorState.CurrentSpline.GetLinearLength(precision: 0.001f, useWorldScale: true));

			GUI.enabled = prevEnabled;
		}
	}
}
