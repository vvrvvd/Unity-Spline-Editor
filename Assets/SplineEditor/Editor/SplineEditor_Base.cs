using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public partial class SplineEditor : UnityEditor.Editor
	{

		public const string SplineEditorSettingsName = "SplineEditorSettings";

		private static SplineEditorState editorState => SplineEditorState.instance;
		private static SplineEditorConfiguration editorSettings => SplineEditorConfiguration.instance;

		#region Private Fields

		private Event currentEvent;
		private Transform handleTransform;
		private Quaternion handleRotation;

		#endregion

		#region Unity Callbacks

		private void OnEnable()
		{

			if (SplineEditorConfiguration.instance.OpenSplineEditorWithSpline && !EditorWindow.HasOpenInstances<SplineEditorWindow>())
			{
				SplineEditorWindow.Initialize();
			}

			editorState.CurrentEditor = this;
			editorState.CurrentSpline = target as BezierSpline;

			InitializeTools();
			InitializeSceneGUI();
			InitializeFlags();
			InitializeDrawCurveMode();
			InitializeNormalsEditorMode();
		}

		private void OnDisable()
		{
			if (editorState.CurrentEditor == null || editorState.CurrentEditor.target != target || editorState.CurrentSpline == null)
			{
				return;
			}

			editorState.CurrentSpline = null;
			editorState.CurrentEditor = null;
			ReleaseTools();
		}

		private void OnSceneGUI()
		{
			if (editorState.CurrentEditor == null || editorState.CurrentEditor.target != target || editorState.CurrentSpline == null)
			{
				return;
			}

			currentEvent = Event.current;
			handleTransform = editorState.CurrentSpline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (currentEvent.type == EventType.ValidateCommand)
			{
				editorState.wasSplineModified = true;
				var lastPoint = editorState.CurrentSpline.Points[editorState.CurrentSpline.PointsCount - 1];
				curveDrawerPosition = lastPoint.position;
				editorState.CurrentSpline?.OnSplineChanged?.Invoke();
			}


			if (editorState.SelectedPointIndex >= editorState.CurrentSpline.PointsCount)
			{
				SelectIndex(editorState.CurrentSpline.PointsCount - 1);
			} else if(editorState.SelectedCurveIndex >= editorState.CurrentSpline.CurvesCount)
			{
				editorState.SelectedCurveIndex = editorState.CurrentSpline.CurvesCount - 1;
			}

			InvokeScheduledActions();
			DrawSceneGUI();
			UpdateTools();

			editorState.UpdateSplineStates();

			if (editorState.CurrentEditor != null && editorState.IsSplineLooped && editorState.IsDrawerMode)
			{
				editorState.CurrentEditor.ToggleDrawCurveMode(false);
			}
		}

		public override void OnInspectorGUI()
		{
			var prevEditor = editorState.CurrentEditor;
			var prevSpline = editorState.CurrentSpline;

			editorState.CurrentEditor = this;
			editorState.CurrentSpline = target as BezierSpline;

			if(editorState.CurrentEditor ==null || editorState.CurrentSpline ==null)
			{
				return;
			}

			DrawInspectorGUI();

			editorState.CurrentEditor = prevEditor;
			editorState.CurrentSpline = prevSpline;
		}

		#endregion

		#region Tools Methods

		private void SelectIndex(int pointIndex)
		{
			if (pointIndex == -1 && editorState.SelectedPointIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(pointIndex);
			Repaint();
		}

		private void UpdateSelectedIndex(int pointIndex)
		{
			editorState.SelectedPointIndex = pointIndex;
			editorState.lastRotation = handleRotation;

			if (pointIndex != -1)
			{
				ToggleDrawCurveMode(false);
			}

			Repaint();
		}

		private void AddCurve(float curveLength)
		{
			if(!editorState.CanNewCurveBeAdded)
			{
				return;
			}

			Undo.RecordObject(editorState.CurrentSpline, "Add Curve");

			if(editorState.SelectedCurveIndex == 0 && (editorState.CurrentSpline.CurvesCount != 1 || editorState.SelectedPointIndex <= 1))
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
			var pointsCount = editorState.CurrentSpline.PointsCount;
			var deltaDir = (editorState.CurrentSpline.Points[pointsCount - 1].position - editorState.CurrentSpline.Points[pointsCount - 2].position).normalized * curveLength / 3;
			var p1 = editorState.CurrentSpline.Points[pointsCount - 1].position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = editorState.CurrentSpline.GetControlPointMode(editorState.SelectedPointIndex);

			editorState.CurrentSpline.AppendCurve(p1, p2, p3, prevMode, false);
			UpdateSelectedIndex(editorState.CurrentSpline.PointsCount - 1);
		}

		private void AddBeginningCurve(float curveLength)
		{
			var deltaDir = (editorState.CurrentSpline.Points[1].position - editorState.CurrentSpline.Points[0].position).normalized * curveLength / 3;
			var p1 = editorState.CurrentSpline.Points[0].position - deltaDir;
			var p2 = p1 - deltaDir;
			var p3 = p2 - deltaDir;

			var prevMode = editorState.CurrentSpline.GetControlPointMode(editorState.SelectedPointIndex);

			editorState.CurrentSpline.AppendCurve(p1, p2, p3, prevMode, true);
			UpdateSelectedIndex(0);
		}

		private void SplitCurve(float splitPointValue)
		{
			if (!editorState.IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(editorState.CurrentSpline, "Add Mid Curve");
			var wasLastPoint = editorState.SelectedPointIndex == editorState.CurrentSpline.PointsCount - 1;
			editorState.CurrentSpline.InsertCurve(editorState.SelectedCurveIndex, splitPointValue);

			if (wasLastPoint && !editorState.CurrentSpline.IsLoop)
			{
				editorState.CurrentEditor.SelectIndex(editorState.CurrentSpline.PointsCount - 4);
			}
			else if (editorState.SelectedPointIndex != 0 && !(wasLastPoint && editorState.CurrentSpline.IsLoop))
			{
				editorState.CurrentEditor.SelectIndex(editorState.SelectedPointIndex + 3);
			}
			else
			{
				editorState.CurrentEditor.SelectIndex(3);
			}

			editorState.wasSplineModified = true;
		}

		private void RemoveSelectedCurve()
		{
			if (!editorState.CanSelectedCurveBeRemoved)
			{
				return;
			}

			Undo.RecordObject(editorState.CurrentSpline, "Remove Curve");
			var curveToRemove = editorState.SelectedCurveIndex;
			var removeFirstPoint = (editorState.SelectedPointIndex - curveToRemove * 3) < 2;
			editorState.CurrentSpline.RemoveCurve(curveToRemove, removeFirstPoint);
			var nextSelectedIndex = Mathf.Min(editorState.SelectedPointIndex, editorState.CurrentSpline.PointsCount - 1);
			UpdateSelectedIndex(nextSelectedIndex);

			editorState.wasSplineModified = true;
		}

		private void CastSpline(Vector3 direction)
		{
			Undo.RecordObject(editorState.CurrentSpline, "Cast Curve Points");
			var pointsCount = editorState.CurrentSpline.PointsCount;
			editorState.CurrentSpline.SetAllControlPointsMode(BezierSpline.BezierControlPointMode.Free);

			var newPointsPositions = new Vector3[pointsCount];
			for (var i = 0; i < pointsCount; i++)
			{
				var worldPosition = handleTransform.TransformPoint(editorState.CurrentSpline.Points[i].position);
				Vector3Utils.TryCastPoint(worldPosition, direction, out newPointsPositions[i]);
				newPointsPositions[i] = handleTransform.InverseTransformPoint(newPointsPositions[i]);
			}

			for (var i = 0; i < pointsCount; i += 3)
			{
				var prevPoint = i > 0 ? editorState.CurrentSpline.Points[i - 1].position : Vector3.zero;
				var nextPoint = i < pointsCount - 1 ? editorState.CurrentSpline.Points[i + 1].position : Vector3.zero;

				editorState.CurrentSpline.UpdatePoint(i, newPointsPositions[i], false, true);

				var isPreviousPointCasted = i > 0 && newPointsPositions[i - 1] != prevPoint;
				if (isPreviousPointCasted)
				{
					editorState.CurrentSpline.UpdatePoint(i - 1, newPointsPositions[i - 1], false, false);
				}

				var isNextPointCasted = i < pointsCount - 1 && newPointsPositions[i + 1] != nextPoint;
				if (isNextPointCasted)
				{
					editorState.CurrentSpline.UpdatePoint(i + 1, newPointsPositions[i + 1], false, false);
				}
			}

			editorState.wasSplineModified = true;
		}

		private void ToggleCloseLoop()
		{
			Undo.RecordObject(editorState.CurrentSpline, "Toggle Close Loop");
			editorState.CurrentSpline.ToggleCloseLoop();
			editorState.CurrentEditor.SelectIndex(0);

			editorState.wasSplineModified = true;
		}

		private void FactorCurve()
		{
			Undo.RecordObject(editorState.CurrentSpline, "Factor Curve");
			editorState.CurrentSpline.FactorSpline();
			if (editorState.SelectedPointIndex != -1)
			{
				editorState.CurrentEditor.SelectIndex(editorState.SelectedPointIndex * 2);
			}

			editorState.wasSplineModified = true;
		}

		private void SimplifySpline()
		{
			if (!editorState.CanSplineBeSimplified)
			{
				return;
			}

			Undo.RecordObject(editorState.CurrentSpline, "Simplify Curve");
			editorState.CurrentSpline.SimplifySpline();
			if (editorState.SelectedPointIndex != -1)
			{
				editorState.CurrentEditor.SelectIndex(editorState.SelectedPointIndex / 2);
			}

			editorState.wasSplineModified = true;
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

        #endregion

    }

}