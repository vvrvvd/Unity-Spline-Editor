using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
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

			if(state)
			{
				Tools.current = Tool.Rotate;
				ToggleDrawCurveMode(false);
			}

			EditorState.IsNormalsEditorMode = state;
			EditorState.wasSplineModified = true;
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
				if (!EditorState.isRotating)
				{
					EditorState.lastRotation = baseHandleRotation;
					EditorState.isRotating = true;
				}

				Undo.RecordObject(EditorState.CurrentSpline, "Rotate Normal Vector");

				var normalAngleDiff = QuaternionUtils.GetSignedAngle(EditorState.lastRotation, rotation, tangentWorldVector);
				EditorState.CurrentSpline.SetNormalAngularOffset(normalIndex, normalAngularOffset + normalAngleDiff);
				EditorState.lastRotation = rotation;
				EditorState.wasSplineModified = true;
			}
			else if ((EditorState.isRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				EditorState.lastRotation = baseHandleRotation;
				EditorState.isRotating = false;
				EditorState.wasSplineModified = true;
			}
		}

		

	}

}
