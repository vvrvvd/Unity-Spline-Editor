using UnityEditor;
using UnityEngine;

namespace SplineMe.Editor
{
	[CustomEditor(typeof(BezierSpline))]
	public partial class BezierSplineEditor : UnityEditor.Editor
	{

		#region Static Fields
		
		internal static BezierSpline currentSpline;
		internal static BezierSplineEditor currentEditor;

		#endregion

		#region Private Fields

		private int selectedPointIndex = -1;

		private Event currentEvent;
		private Transform handleTransform;
		private Quaternion handleRotation;

		#endregion

		#region Properties

		internal int SelectedCurveIndex
		{
			get
			{
				var selectedCurveIndex = selectedPointIndex != -1 ? selectedPointIndex / 3 : -1;
				if (selectedCurveIndex == currentSpline.CurvesCount)
				{
					selectedCurveIndex = currentSpline.IsLoop ? 0 : currentSpline.CurvesCount - 1;
				}

				return selectedCurveIndex;
			}
		}

		internal bool IsAnyPointSelected => selectedPointIndex != -1 && selectedPointIndex < currentSpline.PointsCount;
		internal bool HasMoreThanOneCurve => currentSpline.CurvesCount > 1;
		internal bool CanBeSimplified => HasMoreThanOneCurve && (!currentSpline.IsLoop || currentSpline.CurvesCount > 2);
		internal bool CanSelectedCurveBeRemoved => IsAnyPointSelected && HasMoreThanOneCurve && (!currentSpline.IsLoop || currentSpline.CurvesCount > 2);
		internal bool CanNewCurveBeAdded => !currentSpline.IsLoop;

		#endregion

		#region Unity Callbacks

		private void OnEnable()
		{
			currentEditor = this;
			currentSpline = target as BezierSpline;

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

			currentSpline = null;
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
			handleTransform = currentSpline.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

			if (selectedPointIndex >= currentSpline.PointsCount)
			{
				SelectIndex(currentSpline.PointsCount - 1);
			}

			ApplyShortcuts();
			DrawSceneGUI();
			DrawGUI();
		}

		public override void OnInspectorGUI()
		{
			var prevEditor = currentEditor;
			var prevSpline = currentSpline;

			currentEditor = this;
			currentSpline = target as BezierSpline;

			DrawInspectorGUI();

			currentEditor = prevEditor;
			currentSpline = prevSpline;
		}

		#endregion

		#region Tools Methods

		private void SelectIndex(int index)
		{
			if (index == -1 && selectedPointIndex == -1)
			{
				return;
			}

			UpdateSelectedIndex(index);
			Repaint();
		}

		private void UpdateSelectedIndex(int index)
		{
			selectedPointIndex = index;

			if (index != -1)
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

			Undo.RecordObject(currentSpline, "Add Curve");

			if(SelectedCurveIndex == 0)
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
			var pointsCount = currentSpline.PointsCount;
			var deltaDir = (currentSpline.Points[pointsCount - 1].position - currentSpline.Points[pointsCount - 2].position).normalized * BezierSplineEditor_Consts.CreateCurveSegmentSize / 3;
			var p1 = currentSpline.Points[pointsCount - 1].position + deltaDir;
			var p2 = p1 + deltaDir;
			var p3 = p2 + deltaDir;

			var prevMode = currentSpline.GetControlPointMode(selectedPointIndex);

			currentSpline.AppendCurve(p1, p2, p3, prevMode, false);
			UpdateSelectedIndex(currentSpline.PointsCount - 1);
		}

		private void AddBeginningCurve()
		{
			var deltaDir = (currentSpline.Points[1].position - currentSpline.Points[0].position).normalized * BezierSplineEditor_Consts.CreateCurveSegmentSize / 3;
			var p1 = currentSpline.Points[1].position - deltaDir;
			var p2 = p1 - deltaDir;
			var p3 = p2 - deltaDir;

			var prevMode = currentSpline.GetControlPointMode(selectedPointIndex);

			currentSpline.AppendCurve(p1, p2, p3, prevMode, true);
			UpdateSelectedIndex(0);
		}

		private void AddMidCurve()
		{
			if (!currentEditor.IsAnyPointSelected)
			{
				return;
			}

			Undo.RecordObject(currentSpline, "Add Mid Curve");
			var wasLastPoint = selectedPointIndex == currentSpline.PointsCount - 1;
			currentSpline.InsertCurve(SelectedCurveIndex, 0.5f);

			if (wasLastPoint && !currentSpline.IsLoop)
			{
				currentEditor.SelectIndex(currentSpline.PointsCount - 4);
			}
			else if (selectedPointIndex != 0 && !(wasLastPoint && currentSpline.IsLoop))
			{
				currentEditor.SelectIndex(selectedPointIndex + 3);
			}
			else
			{
				currentEditor.SelectIndex(3);
			}
		}

		private void RemoveSelectedCurve()
		{
			if (!CanSelectedCurveBeRemoved)
			{
				return;
			}

			Undo.RecordObject(currentSpline, "Remove Curve");
			var curveToRemove = SelectedCurveIndex;
			currentSpline.RemoveCurve(curveToRemove);
			var nextSelectedIndex = Mathf.Min(selectedPointIndex, currentSpline.PointsCount - 1);
			UpdateSelectedIndex(nextSelectedIndex);
		}

		private void CastCurve()
		{
			Undo.RecordObject(currentSpline, "Cast Curve Points");
			var pointsCount = currentSpline.PointsCount;
			for (var i = 0; i < pointsCount; i+=3)
			{
				currentSpline.SetControlPointMode(i, BezierSpline.BezierControlPointMode.Free);
			}

			var newPointsPositions = new Vector3[pointsCount];
			for (var i = 0; i < pointsCount; i++)
			{
				var worldPosition = handleTransform.TransformPoint(currentSpline.Points[i].position);
				Vector3Utils.TryCastPoint(worldPosition, -handleTransform.up, out newPointsPositions[i]);
				newPointsPositions[i] = handleTransform.InverseTransformPoint(newPointsPositions[i]);
			}

			for (var i = 0; i < pointsCount; i += 3)
			{
				var prevPoint = i > 0 ? currentSpline.Points[i - 1].position : Vector3.zero;
				var nextPoint = i < pointsCount - 1 ? currentSpline.Points[i + 1].position : Vector3.zero;

				currentSpline.UpdatePoint(i, newPointsPositions[i], false, true);

				var isPreviousPointCasted = i > 0 && newPointsPositions[i - 1] != prevPoint;
				if (isPreviousPointCasted)
				{
					currentSpline.UpdatePoint(i - 1, newPointsPositions[i - 1], false, false);
				}

				var isNextPointCasted = i < pointsCount - 1 && newPointsPositions[i + 1] != nextPoint;
				if (isNextPointCasted)
				{
					currentSpline.UpdatePoint(i + 1, newPointsPositions[i + 1], false, false);
				}
			}

		}

		private void FactorCurve()
		{
			Undo.RecordObject(currentSpline, "Factor Curve");
			currentSpline.FactorSpline();
			if (selectedPointIndex != -1)
			{
				currentEditor.SelectIndex(selectedPointIndex * 2);
			}
		}

		private void SimplifyCurve()
		{
			if (!CanBeSimplified)
			{
				return;
			}

			Undo.RecordObject(currentSpline, "Simplify Curve");
			currentSpline.SimplifySpline();
			if (selectedPointIndex != -1)
			{
				currentEditor.SelectIndex(selectedPointIndex / 2);
			}
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