using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		private void InitializeNormalsEditorMode()
		{
			if (!editorState.IsNormalsEditorMode)
			{
				return;
			}

			ToggleNormalsEditorMode(true);
		}

		private void ToggleNormalsEditorMode(bool state)
		{

			if (editorState.IsNormalsEditorMode != state)
			{
				Undo.RecordObject(editorState, "Toggle Normals Editor Mode");
			}

			if(state)
			{
				Tools.current = Tool.Rotate;
				ToggleDrawCurveMode(false);
			}

			editorState.IsNormalsEditorMode = state;
			editorState.wasSplineModified = true;
		}

		private void RotateNormals(int index, Vector3 worldPoint)
		{
			var normalIndex = index / 3;
			var normalVector = editorState.CurrentSpline.GetNormal(normalIndex);
			var tangentVector = editorState.CurrentSpline.Tangents[normalIndex];
			var normalAngularOffset = editorState.CurrentSpline.NormalsAngularOffsets[normalIndex];

			var normalWorldVector = editorState.CurrentSpline.transform.TransformDirection(normalVector).normalized;
			var tangentWorldVector = editorState.CurrentSpline.transform.TransformDirection(tangentVector).normalized;
			var baseHandleRotation = Quaternion.LookRotation(normalWorldVector, tangentWorldVector);

			EditorGUI.BeginChangeCheck();
			var rotation = Handles.DoRotationHandle(baseHandleRotation, worldPoint);
			if (EditorGUI.EndChangeCheck())
			{
				if (!editorState.isRotating)
				{
					editorState.lastRotation = baseHandleRotation;
					editorState.isRotating = true;
				}

				Undo.RecordObject(editorState.CurrentSpline, "Rotate Normal Vector");

				var normalAngleDiff = QuaternionUtils.GetSignedAngle(editorState.lastRotation, rotation, tangentWorldVector);
				editorState.CurrentSpline.UpdateNormalAngularOffset(normalIndex, normalAngularOffset + normalAngleDiff);
				editorState.lastRotation = rotation;
				editorState.wasSplineModified = true;
			}
			else if ((editorState.isRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				editorState.lastRotation = baseHandleRotation;
				editorState.isRotating = false;
				editorState.wasSplineModified = true;
			}
		}

		

	}

}
