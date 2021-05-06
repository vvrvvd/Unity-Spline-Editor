using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public partial class SplineEditor : UnityEditor.Editor
	{

		#region Static Fields

		internal static event Action OnSplineModified;
		internal static event Action OnSelectedPointChanged;
		internal static event Action OnSelectedSplineChanged;

		private static BezierSpline currentSpline;
		internal static BezierSpline CurrentSpline
		{
			get => currentSpline;
			private set
			{
				if (value == currentSpline)
				{
					return;
				}

				currentSpline = value;
				OnSelectedSplineChanged?.Invoke();
			}
		}

		private static SplineEditor currentEditor;
		internal static SplineEditor CurrentEditor
		{
			get => currentEditor;
			private set
			{
				if (value == currentEditor)
				{
					return;
				}

				currentEditor = value;
				UpdateSplineStates();
			}
		}

		private static int selectedPointIndex = -1;
		internal static int SelectedPointIndex
		{
			get => selectedPointIndex;
			private set
			{
				selectedPointIndex = value;
				var newSelectedCurveIndex = value != -1 ? value / 3 : -1;
				if (newSelectedCurveIndex == CurrentSpline.CurvesCount)
				{
					newSelectedCurveIndex = CurrentSpline.IsLoop ? 0 : CurrentSpline.CurvesCount - 1;
				}
				SelectedCurveIndex = newSelectedCurveIndex;
				OnSelectedPointChanged?.Invoke();
			}
		}

		private static int selectedCurveIndex = -1;
		internal static int SelectedCurveIndex
		{
			get => selectedCurveIndex;
			private set
			{
				selectedCurveIndex = value;
				UpdateSplineStates();
			}
		}

		private static bool isAnyPointSelected;
		internal static bool IsAnyPointSelected
		{
			get => isAnyPointSelected;
			set {
				if (isAnyPointSelected == value)
				{
					return;
				}

				isAnyPointSelected = value;
			}
		}

		private static bool canSelectedCurveBeRemoved;
		internal static bool CanSelectedCurveBeRemoved
		{
			get => canSelectedCurveBeRemoved;
			set
			{
				if (canSelectedCurveBeRemoved == value)
				{
					return;
				}

				canSelectedCurveBeRemoved = value;
				wasSplineModified = true;
			}
		}

		private static bool canNewCurveBeAdded;
		internal static bool CanNewCurveBeAdded
		{
			get => canNewCurveBeAdded;
			set
			{
				if (canNewCurveBeAdded == value)
				{
					return;
				}

				canNewCurveBeAdded = value;
				wasSplineModified = true;
			}
		}

		private static bool isSplineLooped;
		internal static bool IsSplineLooped
		{
			get => isSplineLooped;
			set
			{
				if (isSplineLooped == value)
				{
					return;
				}

				isSplineLooped = value;
				wasSplineModified = true;
			}
		}

		private static bool canSplineBeLooped;
		internal static bool CanSplineBeLooped
		{
			get => canSplineBeLooped;
			set
			{
				if (canSplineBeLooped == value)
				{
					return;
				}

				canSplineBeLooped = value;
				wasSplineModified = true;
			}
		}

		private static bool canSplineBeSimplified;
		internal static bool CanSplineBeSimplified
		{
			get => canSplineBeSimplified;
			set
			{
				if (canSplineBeSimplified == value)
				{
					return;
				}

				canSplineBeSimplified = value;
				wasSplineModified = true;
			}
		}

		internal static bool DrawCurveSmoothAcuteAngles = false;
		internal static float DrawCurveSegmentLength = 1f;
		internal static float DrawCurveFirstPointHook = 0.33f;
		internal static float DrawCurveSecondPointHook = 0.66f;

		private static bool wasSplineModified = false;

		#endregion

		#region Private Fields

		private Event currentEvent;
		private Transform handleTransform;
		private Quaternion handleRotation;

		#endregion

		#region Static Methods

		private static void UpdateSplineStates()
		{
			IsSplineLooped = CurrentSpline != null && CurrentSpline.IsLoop;
			IsAnyPointSelected = CurrentSpline != null && SelectedPointIndex != -1 && SelectedPointIndex < CurrentSpline.PointsCount;
			
			CanSplineBeLooped = CurrentSpline != null && CurrentSpline.CurvesCount > 1;
			CanNewCurveBeAdded = !IsSplineLooped;
			CanSplineBeSimplified = CanSplineBeLooped && (!CurrentSpline.IsLoop || CurrentSpline.CurvesCount > 2);
			CanSelectedCurveBeRemoved = IsAnyPointSelected && CanSplineBeLooped && (!CurrentSpline.IsLoop || CurrentSpline.CurvesCount > 2);

			if (wasSplineModified)
			{
				OnSplineModified?.Invoke();
				wasSplineModified = false;
			}
		}

		#endregion

		#region Unity Callbacks

		private void OnEnable()
		{
			currentEditor = this;
			CurrentSpline = target as BezierSpline;

			InitializeGUI();
			InitializeSceneGUI();
			InitializeFlags();
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
			if (currentEditor == null || currentEditor.target != target)
			{
				return;
			}

			currentEvent = Event.current;
			handleTransform = CurrentSpline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (currentEvent.type == EventType.ValidateCommand)
			{
				wasSplineModified = true;
			}

			if (SelectedPointIndex >= CurrentSpline.PointsCount)
			{
				SelectIndex(CurrentSpline.PointsCount - 1);
			} else if(SelectedCurveIndex >= CurrentSpline.CurvesCount)
			{
				SelectedCurveIndex = CurrentSpline.CurvesCount - 1;
			}

			InvokeScheduledActions();
			DrawSceneGUI();
			DrawGUI();
			UpdateSplineStates();
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

		private void SelectIndex(int pointIndex)
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

		private void AddCurve()
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

		private void AddEndingCurve()
		{
			var pointsCount = CurrentSpline.PointsCount;
			var deltaDir = (CurrentSpline.Points[pointsCount - 1].position - CurrentSpline.Points[pointsCount - 2].position).normalized * SplineEditor_Consts.CreateCurveSegmentSize / 3;
			var p1 = CurrentSpline.Points[pointsCount - 1].position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = CurrentSpline.GetControlPointMode(SelectedPointIndex);

			CurrentSpline.AppendCurve(p1, p2, p3, prevMode, false);
			UpdateSelectedIndex(CurrentSpline.PointsCount - 1);
		}

		private void AddBeginningCurve()
		{
			var deltaDir = (CurrentSpline.Points[1].position - CurrentSpline.Points[0].position).normalized * SplineEditor_Consts.CreateCurveSegmentSize / 3;
			var p1 = CurrentSpline.Points[0].position - deltaDir;
			var p2 = p1 - deltaDir;
			var p3 = p2 - deltaDir;

			var prevMode = CurrentSpline.GetControlPointMode(SelectedPointIndex);

			CurrentSpline.AppendCurve(p1, p2, p3, prevMode, true);
			UpdateSelectedIndex(0);
		}

		private void SplitCurve(float splitPointValue)
		{
			if (!IsAnyPointSelected)
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

			wasSplineModified = true;
		}

		private void RemoveSelectedCurve()
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

			wasSplineModified = true;
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

			wasSplineModified = true;
		}

		private void FactorCurve()
		{
			Undo.RecordObject(CurrentSpline, "Factor Curve");
			CurrentSpline.FactorSpline();
			if (SelectedPointIndex != -1)
			{
				currentEditor.SelectIndex(SelectedPointIndex * 2);
			}

			wasSplineModified = true;
		}

		private void SimplifySpline()
		{
			if (!CanSplineBeSimplified)
			{
				return;
			}

			Undo.RecordObject(CurrentSpline, "Simplify Curve");
			CurrentSpline.SimplifySpline();
			if (SelectedPointIndex != -1)
			{
				currentEditor.SelectIndex(SelectedPointIndex / 2);
			}

			wasSplineModified = true;
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