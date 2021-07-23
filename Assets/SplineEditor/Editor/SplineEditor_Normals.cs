// <copyright file="SplineEditor_Normals.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// Partial class providing Normals Editor feature implementation.
	/// </summary>
	public partial class SplineEditor : UnityEditor.Editor
	{
		private void InitializeNormalsEditorMode()
		{
			if (!EditorState.IsNormalsEditorMode)
			{
				return;
			}

			ToggleNormalsEditorMode(true);
		}

		private void ToggleNormalsEditorMode(bool state)
		{
			if (EditorState.IsNormalsEditorMode != state)
			{
				Undo.RecordObject(EditorState, "Toggle Normals Editor Mode");
			}

			if (state)
			{
				Tools.current = Tool.Rotate;
				ToggleDrawCurveMode(false);
			}

			EditorState.IsNormalsEditorMode = state;
			EditorState.WasSplineModified = true;
		}

		private void RotateNormals(int index, Vector3 worldPoint)
		{
			var normalIndex = index / 3;
			var normalVector = EditorState.CurrentSpline.GetNormal(normalIndex);
			var tangentVector = EditorState.CurrentSpline.Tangents[normalIndex];
			var normalAngularOffset = EditorState.CurrentSpline.NormalsAngularOffsets[normalIndex];

			var normalWorldVector = EditorState.CurrentSpline.transform.TransformDirection(normalVector).normalized;
			var tangentWorldVector = EditorState.CurrentSpline.transform.TransformDirection(tangentVector).normalized;
			var baseHandleRotation = Quaternion.LookRotation(normalWorldVector, tangentWorldVector);

			EditorGUI.BeginChangeCheck();
			var rotation = Handles.DoRotationHandle(baseHandleRotation, worldPoint);
			if (EditorGUI.EndChangeCheck())
			{
				if (!EditorState.IsRotating)
				{
					EditorState.LastRotation = baseHandleRotation;
					EditorState.IsRotating = true;
				}

				Undo.RecordObject(EditorState.CurrentSpline, "Rotate Normal Vector");

				var normalAngleDiff = QuaternionUtils.GetSignedAngle(EditorState.LastRotation, rotation, tangentWorldVector);
				EditorState.CurrentSpline.SetNormalAngularOffset(normalIndex, normalAngularOffset + normalAngleDiff);
				EditorState.LastRotation = rotation;
				EditorState.WasSplineModified = true;
			}
			else if ((EditorState.IsRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				EditorState.LastRotation = baseHandleRotation;
				EditorState.IsRotating = false;
				EditorState.WasSplineModified = true;
			}
		}
	}
}
