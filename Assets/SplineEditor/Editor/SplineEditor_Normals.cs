using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		/// <summary>
		/// I don't know why all the handles for axis are rotated by this angle but it doesn't look neat when the handle is offseted by it so we just take it into account.
		/// </summary>
		// TODO: FIX THIS
		private const float MagicAngleOffset = 0f;

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

		private void RotateNormals(int index, Vector3 point)
		{
			var normalIndex = index / 3;
			EditorGUI.BeginChangeCheck();
			var normalAngularOffset = editorState.CurrentSpline.NormalsAngularOffsets[normalIndex];
			var globalRotation = Quaternion.AngleAxis(editorState.CurrentSpline.GlobalNormalsRotation, editorState.CurrentSpline.Tangents[normalIndex]);
			var normalRotation = globalRotation * Quaternion.AngleAxis(normalAngularOffset + MagicAngleOffset, editorState.CurrentSpline.Tangents[normalIndex]);
			var normalHandleRotation = normalRotation * Quaternion.LookRotation(editorState.CurrentSpline.Tangents[normalIndex]);
			var baseHandleRotation = handleTransform.rotation * normalHandleRotation;
			var rotation = Handles.DoRotationHandle(baseHandleRotation, point);
			if (EditorGUI.EndChangeCheck())
			{
				if (!editorState.isRotating)
				{
					editorState.lastRotation = baseHandleRotation;
					editorState.isRotating = true;
				}

				Undo.RecordObject(editorState.CurrentSpline, "Rotate Normal Vector");

				var normalAngleDiff = Vector3.SignedAngle(rotation * editorState.CurrentSpline.Normals[normalIndex], editorState.lastRotation * editorState.CurrentSpline.Normals[normalIndex], handleTransform.rotation * (-editorState.CurrentSpline.Tangents[normalIndex]));
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
