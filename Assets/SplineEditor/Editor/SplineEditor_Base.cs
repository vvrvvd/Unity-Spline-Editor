// <copyright file="SplineEditor_Base.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing custom editor to BezierSpline component.
	/// </summary>
	[CustomEditor(typeof(BezierSpline))]
	public partial class SplineEditor : UnityEditor.Editor
	{
		private Event currentEvent;
		private Transform handleTransform;
		private Quaternion handleRotation;

		private static SplineEditorState EditorState => SplineEditorState.instance;

		private static SplineEditorConfiguration EditorSettings => SplineEditorConfiguration.instance;

		/// <summary>
		/// Draws custom inspector GUI for BezierSpline.
		/// </summary>
		public override void OnInspectorGUI()
		{
			var prevEditor = EditorState.CurrentEditor;
			var prevSpline = EditorState.CurrentSpline;

			EditorState.CurrentEditor = this;
			EditorState.CurrentSpline = target as BezierSpline;

			if (EditorState.CurrentEditor == null || EditorState.CurrentSpline == null)
			{
				return;
			}

			DrawInspectorGUI();

			EditorState.CurrentEditor = prevEditor;
			EditorState.CurrentSpline = prevSpline;
		}

		private void OnEnable()
		{
			if (SplineEditorConfiguration.instance.OpenSplineEditorWithSpline && !EditorWindow.HasOpenInstances<SplineEditorWindow>())
			{
				SplineEditorWindow.Initialize();
			}

			EditorState.CurrentEditor = this;
			EditorState.CurrentSpline = target as BezierSpline;

			InitializeTools();
			InitializeSceneGUI();
			InitializeFlags();
			InitializeDrawCurveMode();
			InitializeNormalsEditorMode();
		}

		private void OnDisable()
		{
			if (EditorState.CurrentEditor == null || EditorState.CurrentEditor.target != target || EditorState.CurrentSpline == null)
			{
				return;
			}

			EditorState.CurrentSpline = null;
			EditorState.CurrentEditor = null;
			ReleaseTools();
		}

		private void OnSceneGUI()
		{
			if (EditorState.CurrentEditor == null || EditorState.CurrentEditor.target != target || EditorState.CurrentSpline == null)
			{
				return;
			}

			currentEvent = Event.current;
			handleTransform = EditorState.CurrentSpline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (currentEvent.type == EventType.ValidateCommand)
			{
				EditorState.wasSplineModified = true;
				var lastPoint = EditorState.CurrentSpline.Points[EditorState.CurrentSpline.PointsCount - 1];
				curveDrawerPosition = lastPoint.Position;
				if (EditorState.CurrentSpline != null)
				{
					EditorState.CurrentSpline.UpdateSpline();
				}
			}

			if (EditorState.SelectedPointIndex >= EditorState.CurrentSpline.PointsCount)
			{
				SelectIndex(EditorState.CurrentSpline.PointsCount - 1);
			}
			else if (EditorState.SelectedCurveIndex >= EditorState.CurrentSpline.CurvesCount)
			{
				EditorState.SelectedCurveIndex = EditorState.CurrentSpline.CurvesCount - 1;
			}

			InvokeScheduledActions();
			DrawSceneGUI();
			UpdateTools();

			EditorState.UpdateSplineStates();

			if (EditorState.CurrentEditor != null && EditorState.IsSplineLooped && EditorState.IsDrawerMode)
			{
				EditorState.CurrentEditor.ToggleDrawCurveMode(false);
			}
		}

		private void SelectIndex(int pointIndex)
		{
			if (pointIndex == -1 && EditorState.SelectedPointIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(pointIndex);
			Repaint();
		}

		private void UpdateSelectedIndex(int pointIndex)
		{
			EditorState.SelectedPointIndex = pointIndex;
			EditorState.lastRotation = handleRotation;

			if (pointIndex != -1)
			{
				ToggleDrawCurveMode(false);
			}

			Repaint();
		}

		private void AddCurve(float curveLength)
		{
			if (!EditorState.CanNewCurveBeAdded)
			{
				return;
			}

			Undo.RecordObject(EditorState.CurrentSpline, "Add Curve");

			if (EditorState.SelectedCurveIndex == 0 && (EditorState.CurrentSpline.CurvesCount != 1 || EditorState.SelectedPointIndex <= 1))
			{
				AddBeginningCurve(curveLength);
			}
			else
			{
				AddEndingCurve(curveLength);
			}
		}

		private void AddEndingCurve(float curveLength)
		{
			var pointsCount = EditorState.CurrentSpline.PointsCount;
			var deltaDir = (EditorState.CurrentSpline.Points[pointsCount - 1].Position - EditorState.CurrentSpline.Points[pointsCount - 2].Position).normalized * curveLength / 3;
			var p1 = EditorState.CurrentSpline.Points[pointsCount - 1].Position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = EditorState.CurrentSpline.GetControlPointMode(EditorState.SelectedPointIndex);

			EditorState.CurrentSpline.AppendCurve(p1, p2, p3, prevMode, false);
			UpdateSelectedIndex(EditorState.CurrentSpline.PointsCount - 1);
		}

		private void AddBeginningCurve(float curveLength)
		{
			var deltaDir = (EditorState.CurrentSpline.Points[1].Position - EditorState.CurrentSpline.Points[0].Position).normalized * curveLength / 3;
			var p1 = EditorState.CurrentSpline.Points[0].Position - deltaDir;
			var p2 = p1 - deltaDir;
			var p3 = p2 - deltaDir;

			var prevMode = EditorState.CurrentSpline.GetControlPointMode(EditorState.SelectedPointIndex);

			EditorState.CurrentSpline.AppendCurve(p1, p2, p3, prevMode, true);
			UpdateSelectedIndex(0);
		}

		private void SplitCurve(float splitPointValue)
		{
			if (!EditorState.IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(EditorState.CurrentSpline, "Add Mid Curve");
			var wasLastPoint = EditorState.SelectedPointIndex == EditorState.CurrentSpline.PointsCount - 1;
			EditorState.CurrentSpline.InsertCurve(EditorState.SelectedCurveIndex, splitPointValue);

			if (wasLastPoint && !EditorState.CurrentSpline.IsLoop)
			{
				EditorState.CurrentEditor.SelectIndex(EditorState.CurrentSpline.PointsCount - 4);
			}
			else if (EditorState.SelectedPointIndex != 0 && !(wasLastPoint && EditorState.CurrentSpline.IsLoop))
			{
				EditorState.CurrentEditor.SelectIndex(EditorState.SelectedPointIndex + 3);
			}
			else
			{
				EditorState.CurrentEditor.SelectIndex(3);
			}

			EditorState.wasSplineModified = true;
		}

		private void RemoveSelectedCurve()
		{
			if (!EditorState.CanSelectedCurveBeRemoved)
			{
				return;
			}

			Undo.RecordObject(EditorState.CurrentSpline, "Remove Curve");
			var curveToRemove = EditorState.SelectedCurveIndex;
			EditorState.CurrentSpline.RemoveCurve(curveToRemove);
			var nextSelectedIndex = Mathf.Min(EditorState.SelectedPointIndex, EditorState.CurrentSpline.PointsCount - 1);
			UpdateSelectedIndex(nextSelectedIndex);

			EditorState.wasSplineModified = true;
		}

		private void CastSpline(Vector3 direction)
		{
			Undo.RecordObject(EditorState.CurrentSpline, "Cast Curve Points");
			var pointsCount = EditorState.CurrentSpline.PointsCount;
			EditorState.CurrentSpline.SetAllControlPointsMode(BezierSpline.BezierControlPointMode.Free);

			var newPointsPositions = new Vector3[pointsCount];
			for (var i = 0; i < pointsCount; i++)
			{
				var worldPosition = handleTransform.TransformPoint(EditorState.CurrentSpline.Points[i].Position);
				PhysicsUtils.TryCastPoint(worldPosition, direction, out newPointsPositions[i]);
				newPointsPositions[i] = handleTransform.InverseTransformPoint(newPointsPositions[i]);
			}

			for (var i = 0; i < pointsCount; i += 3)
			{
				var prevPoint = i > 0 ? EditorState.CurrentSpline.Points[i - 1].Position : Vector3.zero;
				var nextPoint = i < pointsCount - 1 ? EditorState.CurrentSpline.Points[i + 1].Position : Vector3.zero;

				EditorState.CurrentSpline.SetPoint(i, newPointsPositions[i], false, true);

				var isPreviousPointCasted = i > 0 && newPointsPositions[i - 1] != prevPoint;
				if (isPreviousPointCasted)
				{
					EditorState.CurrentSpline.SetPoint(i - 1, newPointsPositions[i - 1], false, false);
				}

				var isNextPointCasted = i < pointsCount - 1 && newPointsPositions[i + 1] != nextPoint;
				if (isNextPointCasted)
				{
					EditorState.CurrentSpline.SetPoint(i + 1, newPointsPositions[i + 1], false, false);
				}
			}

			EditorState.wasSplineModified = true;
		}

		private void ToggleCloseLoop()
		{
			Undo.RecordObject(EditorState.CurrentSpline, "Toggle Close Loop");
			EditorState.CurrentSpline.ToggleClosingLoopCurve();
			EditorState.CurrentEditor.SelectIndex(0);

			EditorState.wasSplineModified = true;
		}

		private void FactorCurve()
		{
			Undo.RecordObject(EditorState.CurrentSpline, "Factor Curve");
			EditorState.CurrentSpline.FactorSpline();
			if (EditorState.SelectedPointIndex != -1)
			{
				EditorState.CurrentEditor.SelectIndex(EditorState.SelectedPointIndex * 2);
			}

			EditorState.wasSplineModified = true;
		}

		private void SimplifySpline()
		{
			if (!EditorState.CanSplineBeSimplified)
			{
				return;
			}

			Undo.RecordObject(EditorState.CurrentSpline, "Simplify Curve");
			EditorState.CurrentSpline.SimplifySpline();
			if (EditorState.SelectedPointIndex != -1)
			{
				EditorState.CurrentEditor.SelectIndex(EditorState.SelectedPointIndex / 2);
			}

			EditorState.wasSplineModified = true;
		}

		private bool TryCastMousePoint(out Vector3 castedPoint)
		{
			var mousePosition = Event.current.mousePosition;
			var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
			var isCorrectPosition = Physics.Raycast(ray, out var hit, Mathf.Infinity, Physics.AllLayers);

			if (isCorrectPosition)
			{
				castedPoint = hit.point;
			}
			else
			{
				castedPoint = Vector3.zero;
			}

			return isCorrectPosition;
		}
	}
}