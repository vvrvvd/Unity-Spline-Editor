using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Const Fields

		/// <summary>
		/// I don't know why all the handles for axis are rotated by this angle but it doesn't look neat when the handle is offseted by it so we just take it into account.
		/// </summary>
		// TODO: FIX THIS
		private const float MagicAngleOffset = 0f;

		#endregion

		#region Initialize DrawCurveMode

		private void InitializeNormalsEditorMode()
		{
			if (!IsNormalsEditorMode)
			{
				return;
			}

			ToggleNormalsEditorMode(true);
		}

		private void ToggleNormalsEditorMode(bool state)
		{

			if (IsNormalsEditorMode != state)
			{
				Undo.RecordObject(CurrentSpline, "Toggle Normals Editor Mode");
			}

			if(state)
			{
				Tools.current = Tool.Rotate;
				ToggleDrawCurveMode(false);
			}

			IsNormalsEditorMode = state;
			wasSplineModified = true;
		}

		#endregion

		#region Draw Scene GUI

		private void RotateNormals(int index, Vector3 point)
		{
			var normalIndex = index / 3;
			EditorGUI.BeginChangeCheck();
			var normalAngularOffset = currentSpline.NormalsAngularOffsets[normalIndex];
			var globalRotation = Quaternion.AngleAxis(currentSpline.GlobalNormalsRotation, currentSpline.Tangents[normalIndex]);
			var normalRotation = globalRotation * Quaternion.AngleAxis(normalAngularOffset + MagicAngleOffset, currentSpline.Tangents[normalIndex]);
			var normalHandleRotation = normalRotation * Quaternion.LookRotation(currentSpline.Tangents[normalIndex]);
			var baseHandleRotation = handleTransform.rotation * normalHandleRotation;
			var rotation = Handles.DoRotationHandle(baseHandleRotation, point);
			if (EditorGUI.EndChangeCheck())
			{
				if (!isRotating)
				{
					lastRotation = baseHandleRotation;
					isRotating = true;
				}

				Undo.RecordObject(CurrentSpline, "Rotate Normal Vector");

				var normalAngleDiff = Vector3.SignedAngle(rotation * currentSpline.Normals[normalIndex], lastRotation * currentSpline.Normals[normalIndex], handleTransform.rotation * (-currentSpline.Tangents[normalIndex]));
				currentSpline.UpdateNormalAngularOffset(normalIndex, normalAngularOffset + normalAngleDiff);
				lastRotation = rotation;
				wasSplineModified = true;
			}
			else if ((isRotating && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
			{
				lastRotation = baseHandleRotation;
				isRotating = false;
				wasSplineModified = true;
			}
		}

		#endregion

	}

}
