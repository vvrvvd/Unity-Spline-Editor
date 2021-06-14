using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public partial class SplineEditor : UnityEditor.Editor
	{

		public const string SplineEditorSettingsName = "SplineEditorSettings";

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

				if (!stopEventsOnInitialization)
				{
					OnSelectedSplineChanged?.Invoke();
				}
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

				if(!stopEventsOnInitialization)
				{
					OnSelectedPointChanged?.Invoke();
				}
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

		private static bool isDrawerMode;
		internal static bool IsDrawerMode
		{
			get => isDrawerMode;
			set
			{
				if (isDrawerMode == value)
				{
					return;
				}

				isDrawerMode = value;
				wasSplineModified = true;
			}
		}


		private static bool isNormalsEditorMode;
		internal static bool IsNormalsEditorMode
		{
			get => isNormalsEditorMode;
			set
			{
				if (isNormalsEditorMode == value)
				{
					return;
				}

				isNormalsEditorMode = value;
				wasSplineModified = true;
			}
		}

		internal static bool DrawCurveSmoothAcuteAngles = true;
		internal static float DrawCurveSegmentLength = 1f;
		internal static float DrawCurveFirstPointHook = 0.33f;
		internal static float DrawCurveSecondPointHook = 0.66f;

		private static bool wasSplineModified = false;
		private static bool stopEventsOnInitialization = false;
		private static float previousGlobalNormalsRotation = 0f;
		private static SplineEditorConfiguration editorSettings;

		#endregion

		#region Private Fields

		private Event currentEvent;
		private Transform handleTransform;
		private Quaternion handleRotation;

		#endregion

		#region Static Methods

		internal static void UpdateSplineStates()
		{
			IsSplineLooped = CurrentSpline != null && CurrentSpline.IsLoop;
			IsAnyPointSelected = CurrentSpline != null && SelectedPointIndex != -1 && SelectedPointIndex < CurrentSpline.PointsCount;
			
			CanSplineBeLooped = CurrentSpline != null && CurrentSpline.CurvesCount > 1;
			CanNewCurveBeAdded = !IsSplineLooped;
			CanSplineBeSimplified = CanSplineBeLooped && (!CurrentSpline.IsLoop || CurrentSpline.CurvesCount > 2);
			CanSelectedCurveBeRemoved = IsAnyPointSelected && CanSplineBeLooped && (!CurrentSpline.IsLoop || CurrentSpline.CurvesCount > 2);

			if (CurrentSpline!=null && CurrentSpline.GlobalNormalsRotation != previousGlobalNormalsRotation)
			{
				previousGlobalNormalsRotation = CurrentSpline.GlobalNormalsRotation;
				wasSplineModified = true;
			}

			if(CurrentEditor!=null && IsSplineLooped && IsDrawerMode)
			{
				CurrentEditor.ToggleDrawCurveMode(false);
			}

			if (!stopEventsOnInitialization && wasSplineModified)
			{
				OnSplineModified?.Invoke();
				wasSplineModified = false;
			}

		}

		#endregion

		#region Unity Callbacks

		[RuntimeInitializeOnLoadMethod]
		private static void TryLoadEditorSettings()
		{
			if (editorSettings != null)
			{
				return;
			}
			
			editorSettings = Resources.Load<SplineEditorConfiguration>(SplineEditorSettingsName);
		}

		private void OnEnable()
		{
			TryLoadEditorSettings();

			if (editorSettings!=null && editorSettings.OpenSplineEditorWithSpline)
			{
				SplineEditorWindow.Initialize();
			}

			stopEventsOnInitialization = true;
			CurrentEditor = this;
			CurrentSpline = target as BezierSpline;

			InitializeTools();
			InitializeSceneGUI();
			InitializeFlags();
			InitializeDrawCurveMode();
			InitializeNormalsEditorMode();
		}

		private void OnDisable()
		{
			if (CurrentEditor == null || CurrentEditor.target != target)
			{
				return;
			}

			CurrentSpline = null;
			CurrentEditor = null;
			ReleaseTools();
		}

		private void OnSceneGUI()
		{
			if (CurrentEditor == null || CurrentEditor.target != target)
			{
				return;
			}

			TryLoadEditorSettings();

			currentEvent = Event.current;
			handleTransform = CurrentSpline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (currentEvent.type == EventType.ValidateCommand)
			{
				wasSplineModified = true;
				var lastPoint = CurrentSpline.Points[CurrentSpline.PointsCount - 1];
				curveDrawerPosition = lastPoint.position;
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
			UpdateTools();
			UpdateSplineStates();

			if(stopEventsOnInitialization)
			{
				OnSelectedSplineChanged?.Invoke();
				OnSelectedPointChanged?.Invoke();
				OnSplineModified?.Invoke();
				stopEventsOnInitialization = false;
			}
		}

		public override void OnInspectorGUI()
		{
			var prevEditor = CurrentEditor;
			var prevSpline = CurrentSpline;

			CurrentEditor = this;
			CurrentSpline = target as BezierSpline;

			TryLoadEditorSettings();
			DrawInspectorGUI();

			CurrentEditor = prevEditor;
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
			lastRotation = handleRotation;

			if (pointIndex != -1)
			{
				ToggleDrawCurveMode(false);
			}

			Repaint();
		}

		private void AddCurve(float curveLength)
		{
			if(!CanNewCurveBeAdded)
			{
				return;
			}

			Undo.RecordObject(CurrentSpline, "Add Curve");

			if(SelectedCurveIndex == 0 && (currentSpline.CurvesCount != 1 || selectedPointIndex <= 1))
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
			var pointsCount = CurrentSpline.PointsCount;
			var deltaDir = (CurrentSpline.Points[pointsCount - 1].position - CurrentSpline.Points[pointsCount - 2].position).normalized * curveLength / 3;
			var p1 = CurrentSpline.Points[pointsCount - 1].position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = CurrentSpline.GetControlPointMode(SelectedPointIndex);

			CurrentSpline.AppendCurve(p1, p2, p3, prevMode, false);
			UpdateSelectedIndex(CurrentSpline.PointsCount - 1);
		}

		private void AddBeginningCurve(float curveLength)
		{
			var deltaDir = (CurrentSpline.Points[1].position - CurrentSpline.Points[0].position).normalized * curveLength / 3;
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
				CurrentEditor.SelectIndex(CurrentSpline.PointsCount - 4);
			}
			else if (SelectedPointIndex != 0 && !(wasLastPoint && CurrentSpline.IsLoop))
			{
				CurrentEditor.SelectIndex(SelectedPointIndex + 3);
			}
			else
			{
				CurrentEditor.SelectIndex(3);
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
			CurrentSpline.SetAllControlPointsMode(BezierSpline.BezierControlPointMode.Free);

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

		private void ToggleCloseLoop()
		{
			Undo.RecordObject(CurrentSpline, "Toggle Close Loop");
			CurrentSpline.ToggleCloseLoop();
			CurrentEditor.SelectIndex(0);

			wasSplineModified = true;
		}

		private void FactorCurve()
		{
			Undo.RecordObject(CurrentSpline, "Factor Curve");
			CurrentSpline.FactorSpline();
			if (SelectedPointIndex != -1)
			{
				CurrentEditor.SelectIndex(SelectedPointIndex * 2);
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
				CurrentEditor.SelectIndex(SelectedPointIndex / 2);
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