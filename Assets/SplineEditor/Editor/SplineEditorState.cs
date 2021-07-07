using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	[FilePath("SplineEditor/SplineEditorState.conf", FilePathAttribute.Location.ProjectFolder)]
	public partial class SplineEditorState : ScriptableSingleton<SplineEditorState>
	{

		public event Action OnSplineModified;
		public event Action OnSelectedPointChanged;
		public event Action OnSelectedSplineChanged;

		private BezierSpline currentSpline;
		public BezierSpline CurrentSpline
		{
			get => currentSpline;
			set
			{
				if (value == currentSpline)
				{
					return;
				}

				currentSpline = value;
				OnSelectedSplineChanged?.Invoke();
			}
		}

		private SplineEditor currentEditor;
		public SplineEditor CurrentEditor
		{
			get => currentEditor;
			set
			{
				if (value == currentEditor)
				{
					return;
				}

				currentEditor = value;
				UpdateSplineStates();
			}
		}

		private int selectedPointIndex = -1;
		public int SelectedPointIndex
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
				OnSelectedPointChanged?.Invoke();
			}
		}

		private int selectedCurveIndex = -1;
		public int SelectedCurveIndex
		{
			get => selectedCurveIndex;
			set
			{
				selectedCurveIndex = value;
				UpdateSplineStates();
			}
		}

		private bool isAnyPointSelected;
		public bool IsAnyPointSelected
		{
			get => isAnyPointSelected;
			set
			{
				if (isAnyPointSelected == value)
				{
					return;
				}

				isAnyPointSelected = value;
			}
		}

		private bool canSelectedCurveBeRemoved;
		public bool CanSelectedCurveBeRemoved
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

		private bool canNewCurveBeAdded;
		public bool CanNewCurveBeAdded
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

		private bool isSplineLooped;
		public bool IsSplineLooped
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

		private bool canSplineBeLooped;
		public bool CanSplineBeLooped
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

		private bool canSplineBeSimplified;
		public bool CanSplineBeSimplified
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

		private bool isDrawerMode;
		public bool IsDrawerMode
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


		private bool isNormalsEditorMode;
		public bool IsNormalsEditorMode
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

		public bool DrawCurveSmoothAcuteAngles = true;
		public float DrawCurveSegmentLength = 5f;
		public float DrawCurveFirstPointHook = 0.33f;
		public float DrawCurveSecondPointHook = 0.66f;

		public Tool savedTool = Tool.None;
		public bool wasSplineModified = false;

		public bool isRotating;
		public bool isScaling;
		public bool isDraggingPoint;
		
		public Vector3 lastScale;
		public Quaternion lastRotation;

		public bool drawPoints = true;
		public bool drawSpline = true;
		public bool drawDirections = false;
		public bool showTransformHandle = true;
		public bool alwaysDrawSplineOnScene = true;
		public bool drawNormals = false;

		public void UpdateSplineStates()
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

	}

}