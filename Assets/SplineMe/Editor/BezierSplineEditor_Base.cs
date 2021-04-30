using System;
using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Static Fields

		private static BezierSpline currentSpline;		
		internal static BezierSpline CurrentSpline
		{
			get => currentSpline;
			set
			{
				if(value==currentSpline)
				{
					return;
				}

				currentSpline = value;
				OnCurrentSplineChanged?.Invoke();
			}
		}

		private static BezierSplineEditor currentEditor;
		internal static BezierSplineEditor CurrentEditor
		{
			get => currentEditor;
			set
			{
				if (value == currentEditor)
				{
					return;
				}

				currentEditor = value;
			}
		}

		internal static event Action OnCurrentSplineChanged;
		internal static event Action OnSelectedCurveChanged;

		#endregion

		#region Private Fields

		private Event currentEvent;
		private Transform handleTransform;
		private Quaternion handleRotation;

		#endregion

		#region Properties

		private int selectedPointIndex = -1;
		private int SelectedPointIndex
		{
			get => selectedPointIndex;
			set
			{
				selectedPointIndex = value;
				var newSelectedCurveIndex = value != -1 ? value / 3 : -1;
				if (newSelectedCurveIndex == CurrentSpline.CurvesCount)
				{
					newSelectedCurveIndex = CurrentSpline.IsLoop ? 0 : CurrentSpline.CurvesCount - 1;
				}
				SelectedCurveIndex = newSelectedCurveIndex;
			}
		}

		internal int selectedCurveIndex = -1;
		internal int SelectedCurveIndex
		{
			get => selectedCurveIndex;
			private set
			{
				selectedCurveIndex = value;
				OnSelectedCurveChanged?.Invoke();
			}
		}

		internal bool IsAnyPointSelected => SelectedPointIndex != -1 && SelectedPointIndex < CurrentSpline.PointsCount;
		internal bool HasMoreThanOneCurve => CurrentSpline.CurvesCount > 1;
		internal bool CanBeSimplified => HasMoreThanOneCurve && (!CurrentSpline.IsLoop || CurrentSpline.CurvesCount > 2);
		internal bool CanSelectedCurveBeRemoved => IsAnyPointSelected && HasMoreThanOneCurve && (!CurrentSpline.IsLoop || CurrentSpline.CurvesCount > 2);
		internal bool CanNewCurveBeAdded => !CurrentSpline.IsLoop;

		#endregion

		#region Unity Callbacks

		private void OnEnable()
		{
			currentEditor = this;
			CurrentSpline = target as BezierSpline;

			InitializeGUI();
			InitializeSceneGUI();
			InitializeShortcuts();
			InitializeDrawCurveMode();
		}

		private void OnDisable()
		{
			if (currentEditor == null || currentEditor.target != target)
			{
				return;
			}

			CurrentSpline = null;
			currentEditor = null;
			ReleaseGUI();
		}

		private void OnSceneGUI()
		{
			if (currentEditor==null || currentEditor.target != target)
			{
				return;
			}

			currentEvent = Event.current;
			handleTransform = CurrentSpline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (SelectedPointIndex >= CurrentSpline.PointsCount)
			{
				SelectIndex(CurrentSpline.PointsCount - 1);
			}

			ApplyShortcuts();
			DrawSceneGUI();
			DrawGUI();
		}

		public override void OnInspectorGUI()
		{
			var prevEditor = currentEditor;
			var prevSpline = CurrentSpline;

			currentEditor = this;
			CurrentSpline = target as BezierSpline;

			DrawInspectorGUI();

			currentEditor = prevEditor;
			CurrentSpline = prevSpline;
		}

		#endregion

		#region Tools Methods

		internal void SelectIndex(int pointIndex)
		{
			if (pointIndex == -1 && SelectedPointIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(pointIndex);
			Repaint();
		}

		private void UpdateSelectedIndex(int pointIndex)
		{
			SelectedPointIndex = pointIndex;

			if (pointIndex != -1)
			{
				ToggleDrawCurveMode(false);
			}

			Repaint();
		}

		internal void AddCurve()
		{
			if(!CanNewCurveBeAdded)
			{
				return;
			}

			Undo.RecordObject(CurrentSpline, "Add Curve");

			if(SelectedCurveIndex == 0 && (currentSpline.CurvesCount != 1 || selectedPointIndex <= 1))
			{
				AddBeginningCurve();
			}
			else
			{
				AddEndingCurve();
			}
		}

		internal void AddEndingCurve()
		{
			var pointsCount = CurrentSpline.PointsCount;
			var deltaDir = (CurrentSpline.Points[pointsCount - 1].position - CurrentSpline.Points[pointsCount - 2].position).normalized * BezierSplineEditor_Consts.CreateCurveSegmentSize / 3;
			var p1 = CurrentSpline.Points[pointsCount - 1].position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = CurrentSpline.GetControlPointMode(SelectedPointIndex);

			CurrentSpline.AppendCurve(p1, p2, p3, prevMode, false);
			UpdateSelectedIndex(CurrentSpline.PointsCount - 1);
		}

		internal void AddBeginningCurve()
		{
			var deltaDir = (CurrentSpline.Points[1].position - CurrentSpline.Points[0].position).normalized * BezierSplineEditor_Consts.CreateCurveSegmentSize / 3;
			var p1 = CurrentSpline.Points[0].position - deltaDir;
			var p2 = p1 - deltaDir;
			var p3 = p2 - deltaDir;

			var prevMode = CurrentSpline.GetControlPointMode(SelectedPointIndex);

			CurrentSpline.AppendCurve(p1, p2, p3, prevMode, true);
			UpdateSelectedIndex(0);
		}

		internal void SplitCurve(float splitPointValue)
		{
			if (!currentEditor.IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(CurrentSpline, "Add Mid Curve");
			var wasLastPoint = SelectedPointIndex == CurrentSpline.PointsCount - 1;
			CurrentSpline.InsertCurve(SelectedCurveIndex, splitPointValue);

			if (wasLastPoint && !CurrentSpline.IsLoop)
			{
				currentEditor.SelectIndex(CurrentSpline.PointsCount - 4);
			}
			else if (SelectedPointIndex != 0 && !(wasLastPoint && CurrentSpline.IsLoop))
			{
				currentEditor.SelectIndex(SelectedPointIndex + 3);
			}
			else
			{
				currentEditor.SelectIndex(3);
			}
		}

		internal void RemoveSelectedCurve()
		{
			if (!CanSelectedCurveBeRemoved)
			{
				return;
			}

			Undo.RecordObject(CurrentSpline, "Remove Curve");
			var curveToRemove = SelectedCurveIndex;
			var removeFirstPoint = (selectedPointIndex - curveToRemove * 3) < 2;
			CurrentSpline.RemoveCurve(curveToRemove, removeFirstPoint);
			var nextSelectedIndex = Mathf.Min(SelectedPointIndex, CurrentSpline.PointsCount - 1);
			UpdateSelectedIndex(nextSelectedIndex);
		}

		private void CastSpline(Vector3 direction)
		{
			Undo.RecordObject(CurrentSpline, "Cast Curve Points");
			var pointsCount = CurrentSpline.PointsCount;
			for (var i = 0; i < pointsCount; i += 3)
			{
				CurrentSpline.SetControlPointMode(i, BezierSpline.BezierControlPointMode.Free);
			}

			var newPointsPositions = new Vector3[pointsCount];
			for (var i = 0; i < pointsCount; i++)
			{
				var worldPosition = handleTransform.TransformPoint(CurrentSpline.Points[i].position);
				Vector3Utils.TryCastPoint(worldPosition, direction, out newPointsPositions[i]);
				newPointsPositions[i] = handleTransform.InverseTransformPoint(newPointsPositions[i]);
			}

			for (var i = 0; i < pointsCount; i += 3)
			{
				var prevPoint = i > 0 ? CurrentSpline.Points[i - 1].position : Vector3.zero;
				var nextPoint = i < pointsCount - 1 ? CurrentSpline.Points[i + 1].position : Vector3.zero;

				CurrentSpline.UpdatePoint(i, newPointsPositions[i], false, true);

				var isPreviousPointCasted = i > 0 && newPointsPositions[i - 1] != prevPoint;
				if (isPreviousPointCasted)
				{
					CurrentSpline.UpdatePoint(i - 1, newPointsPositions[i - 1], false, false);
				}

				var isNextPointCasted = i < pointsCount - 1 && newPointsPositions[i + 1] != nextPoint;
				if (isNextPointCasted)
				{
					CurrentSpline.UpdatePoint(i + 1, newPointsPositions[i + 1], false, false);
				}
			}
		}

		internal void FactorCurve()
		{
			Undo.RecordObject(CurrentSpline, "Factor Curve");
			CurrentSpline.FactorSpline();
			if (SelectedPointIndex != -1)
			{
				currentEditor.SelectIndex(SelectedPointIndex * 2);
			}
		}

		internal void SimplifySpline()
		{
			if (!CanBeSimplified)
			{
				return;
			}

			Undo.RecordObject(CurrentSpline, "Simplify Curve");
			CurrentSpline.SimplifySpline();
			if (SelectedPointIndex != -1)
			{
				currentEditor.SelectIndex(SelectedPointIndex / 2);
			}
		}

		internal bool TryCastMousePoint(out Vector3 castedPoint)
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