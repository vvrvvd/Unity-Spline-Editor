using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Const Fields

		private const float DrawCurveMinLengthToVisualize = 0.1f;

		#endregion

		#region Private Fields

		private bool isDraggingNewCurve;
		private bool firstControlPointSet;
		private bool secondControlPointSet;

		private Vector3 curveDrawerPosition = Vector3.zero;
		private Vector3[] newCurvePoints = new Vector3[4];

		#endregion

		#region Initialize DrawCurveMode

		private void InitializeDrawCurveMode()
		{
			if (!editorState.IsDrawerMode)
			{
				return;
			}

			ToggleDrawCurveMode(true);
		}

		public void ToggleDrawCurveMode(bool state)
		{
			if ((state && editorState.CurrentSpline.IsLoop))
			{
				return;
			}


			if (editorState.IsDrawerMode != state)
			{
				Undo.RecordObject(editorState.CurrentSpline, "Toggle Draw Curve Mode");
			}

			editorState.IsDrawerMode = state;

			if (state)
			{
				ToggleNormalsEditorMode(false);
				var lastPoint = editorState.CurrentSpline.Points[editorState.CurrentSpline.PointsCount - 1];
				StartDrawCurveMode(lastPoint.position);
				SelectIndex(-1);
			}

			editorState.wasSplineModified = true;
		}

		private void StartDrawCurveMode(Vector3 startPoint)
		{
			for (var i = 0; i < newCurvePoints.Length; i++)
			{
				newCurvePoints[i] = startPoint;
			}

			curveDrawerPosition = startPoint;
			firstControlPointSet = false;
			secondControlPointSet = false;
		}


		#endregion

		#region Draw Scene GUI

		private void DrawCurveModeSceneGUI()
		{
			var curveDrawerPointLocal = curveDrawerPosition;
			var curveDrawerPointWorld = editorState.CurrentSpline.transform.TransformPoint(curveDrawerPointLocal);
			var size = editorSettings.ScaleDrawerHandleOnScreen ? HandleUtility.GetHandleSize(curveDrawerPointWorld) : 1f;

			if (isDraggingNewCurve)
			{
				VisualizeDrawCurveModeCurve();
			}

			Handles.color = editorSettings.DrawerModeHandleColor;
			Handles.Button(curveDrawerPointWorld, handleRotation, size * editorSettings.DrawerModeHandleSize, size * editorSettings.DrawerModeHandleSize, Handles.SphereHandleCap);

			if (castSelectedPointFlag)
			{
				isDraggingNewCurve = true;
				if (TryCastMousePoint(out var castedPosition))
				{
					Undo.RecordObject(editorState.CurrentSpline, "Cast Drawer Point");
					curveDrawerPosition = editorState.CurrentSpline.transform.InverseTransformPoint(castedPosition);
					UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				var newEndPositionGlobal = Handles.DoPositionHandle(curveDrawerPointWorld, handleRotation);
				var wasChanged = EditorGUI.EndChangeCheck();
				if (wasChanged)
				{
					isDraggingNewCurve = true;
					Undo.RecordObject(editorState.CurrentSpline, "Move Drawer Point");
					curveDrawerPosition = editorState.CurrentSpline.transform.InverseTransformPoint(newEndPositionGlobal);
					UpdateNewDrawCurvePainterPosition(curveDrawerPosition);
				}
				else if ((isDraggingNewCurve && Event.current.type == EventType.Used) || Event.current.type == EventType.ValidateCommand)
				{
					if (firstControlPointSet && secondControlPointSet)
					{
						SpawnDrawCurveModeCurve(editorState.DrawCurveSmoothAcuteAngles);
					}

					var defaultDrawerPosition = editorState.CurrentSpline.Points[editorState.CurrentSpline.PointsCount - 1].position;
					StartDrawCurveMode(defaultDrawerPosition);
					isDraggingNewCurve = false;
					castSelectedPointFlag = false;
				}
			}
		}

		private void VisualizeDrawCurveModeCurve()
		{
			var p0 = newCurvePoints[0];
			var p3 = newCurvePoints[3];

			if (Vector3.Distance(p0, p3) < editorState.DrawCurveSegmentLength * DrawCurveMinLengthToVisualize)
			{
				return;
			}

			BezierUtils.GetInverseControlPoints(p0, p3, newCurvePoints[1], newCurvePoints[2], editorState.DrawCurveFirstPointHook, editorState.DrawCurveSecondPointHook, out var p1, out var p2);

			p0 = handleTransform.TransformPoint(p0);
			p1 = handleTransform.TransformPoint(p1);
			p2 = handleTransform.TransformPoint(p2);
			p3 = handleTransform.TransformPoint(p3);

			Handles.DrawBezier(p0, p3, p1, p2, editorSettings.DrawerModeCurveColor, null, editorSettings.SplineWidth * 1.5f);

			if (editorState.drawPoints)
			{
				if (firstControlPointSet)
				{
					var f = handleTransform.TransformPoint(newCurvePoints[1]);
					var size = editorSettings.ScaleDrawerHandleOnScreen ? HandleUtility.GetHandleSize(f) : 1f;
					Handles.color = editorSettings.DrawModePointColor;
					Handles.CubeHandleCap(0, f, Quaternion.identity, size * editorSettings.MainPointSize, EventType.Repaint);
				}

				if (secondControlPointSet)
				{
					var g = handleTransform.TransformPoint(newCurvePoints[2]);
					var size = editorSettings.ScaleDrawerHandleOnScreen ? HandleUtility.GetHandleSize(g) : 1f;
					Handles.color = editorSettings.DrawModePointColor;
					Handles.CubeHandleCap(0, g, Quaternion.identity, size * editorSettings.MainPointSize, EventType.Repaint);
				}

			}

		}

		private void UpdateNewDrawCurvePainterPosition(Vector3 newEndPosition)
		{
			float distance;
			if (secondControlPointSet)
			{
				distance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) + Vector3.Distance(newCurvePoints[1], newCurvePoints[2]) + Vector3.Distance(newCurvePoints[2], newEndPosition);
			}
			else if (firstControlPointSet)
			{
				distance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) + Vector3.Distance(newCurvePoints[1], newEndPosition);
			}
			else
			{
				distance = Vector3.Distance(newCurvePoints[0], newEndPosition);
			}

			var dir = (newEndPosition - newCurvePoints[0]).normalized;
			var firstPointDistance = editorState.DrawCurveFirstPointHook * editorState.DrawCurveSegmentLength;
			var secondPointDistance = editorState.DrawCurveSecondPointHook * editorState.DrawCurveSegmentLength;

			if (!firstControlPointSet)
			{
				if (distance < firstPointDistance)
				{
					newCurvePoints[1] = newEndPosition;
				}
				else if (distance >= secondPointDistance)
				{
					newCurvePoints[1] = newCurvePoints[0] + dir * firstPointDistance;
					firstControlPointSet = true;
				}
			}

			if (!secondControlPointSet)
			{
				if (distance < secondPointDistance)
				{
					newCurvePoints[2] = newEndPosition;
				}
				else if (distance >= editorState.DrawCurveSegmentLength)
				{
					newCurvePoints[2] = newCurvePoints[0] + dir * secondPointDistance;
					secondControlPointSet = true;
				}
			}

			if (distance < secondPointDistance && distance >= firstPointDistance && !firstControlPointSet)
			{
				var prevFirstPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[1]) / editorState.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / editorState.DrawCurveSegmentLength;
				var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevFirstPointNormalizedDistance);
				newCurvePoints[1] = Vector3.Lerp(newCurvePoints[1], newEndPosition, Mathf.Abs(editorState.DrawCurveFirstPointHook - prevFirstPointNormalizedDistance) / normalizedDistancesDiff);
				firstControlPointSet = true;
			}

			if (distance < editorState.DrawCurveSegmentLength)
			{
				newCurvePoints[3] = newEndPosition;

				if (distance >= secondPointDistance && !secondControlPointSet)
				{
					var prevSecondPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[2]) / editorState.DrawCurveSegmentLength;
					var normalizedCurrentDistance = distance / editorState.DrawCurveSegmentLength;
					var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevSecondPointNormalizedDistance);
					newCurvePoints[2] = Vector3.Lerp(newCurvePoints[2], newEndPosition, Mathf.Abs(editorState.DrawCurveSecondPointHook - prevSecondPointNormalizedDistance) / normalizedDistancesDiff);
					secondControlPointSet = true;
				}
			}
			else
			{
				var prevLastPointNormalizedDistance = Vector3.Distance(newCurvePoints[0], newCurvePoints[3]) / editorState.DrawCurveSegmentLength;
				var normalizedCurrentDistance = distance / editorState.DrawCurveSegmentLength;
				var normalizedDistancesDiff = Mathf.Abs(normalizedCurrentDistance - prevLastPointNormalizedDistance);
				newCurvePoints[3] = Vector3.Lerp(newCurvePoints[3], newEndPosition, Mathf.Abs(1f - prevLastPointNormalizedDistance) / normalizedDistancesDiff);
				SpawnDrawCurveModeCurve(editorState.DrawCurveSmoothAcuteAngles);
				StartDrawCurveMode(newCurvePoints[3]);
				UpdateNewDrawCurvePainterPosition(newEndPosition);
			}

		}

		private void SpawnDrawCurveModeCurve(bool smoothAcuteAngles)
		{
			var p0 = newCurvePoints[0];
			var p3 = newCurvePoints[3];
			BezierUtils.GetInverseControlPoints(p0, p3, newCurvePoints[1], newCurvePoints[2], editorState.DrawCurveFirstPointHook, editorState.DrawCurveSecondPointHook, out var p1, out var p2);

			if (smoothAcuteAngles)
			{
				editorState.CurrentSpline.SetControlPointMode(editorState.CurrentSpline.PointsCount - 1, BezierControlPointMode.Aligned);
			}

			editorState.CurrentSpline.AppendCurve(p1, p2, p3, BezierControlPointMode.Free, false);
		}

		#endregion

	}

}
