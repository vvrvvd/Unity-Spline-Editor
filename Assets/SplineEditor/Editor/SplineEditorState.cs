// <copyright file="SplineEditorState.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing serializable state for custom BezierSpline editor.
	/// </summary>
	[FilePath("SplineEditor/SplineEditorState.conf", FilePathAttribute.Location.ProjectFolder)]
	public partial class SplineEditorState : ScriptableSingleton<SplineEditorState>
	{
		private int selectedPointIndex = -1;
		private int selectedCurveIndex = -1;

		private bool isAnyPointSelected;
		private bool canSelectedCurveBeRemoved;
		private bool canNewCurveBeAdded;
		private bool isSplineLooped;
		private bool canSplineBeLooped;
		private bool canSplineBeSimplified;

		[SerializeField]
		private bool isDrawerMode;
		[SerializeField]
		private bool isNormalsEditorMode;
		[SerializeField]
		private bool drawCurveSmoothAcuteAngles = true;
		[SerializeField]
		private float drawCurveSegmentLength = 5f;
		[SerializeField]
		private float drawCurveFirstPointHook = 0.33f;
		[SerializeField]
		private float drawCurveSecondPointHook = 0.66f;
		[SerializeField]
		private bool drawPoints = true;
		[SerializeField]
		private bool drawSpline = true;
		[SerializeField]
		private bool drawNormals = false;
		[SerializeField]
		private bool showTransformHandle = true;
		[SerializeField]
		private bool alwaysDrawSplineOnScene = true;

		private BezierSpline currentSpline;
		private SplineEditor currentEditor;

		private Tool savedTool = Tool.None;
		private bool wasSplineModified = false;

		private bool isRotating;
		private bool isScaling;
		private bool isDraggingPoint;

		private Vector3 lastScale;
		private Quaternion lastRotation;

		public event Action OnSplineModified;

		public event Action OnSelectedPointChanged;

		public event Action OnSelectedSplineChanged;

		public BezierSpline CurrentSpline
		{
			get => currentSpline;
			set
			{
				if (currentSpline != null && value == currentSpline)
				{
					return;
				}

				currentSpline = value;
				OnSelectedSplineChanged?.Invoke();
			}
		}

		public SplineEditor CurrentEditor
		{
			get => currentEditor;
			set
			{
				if (currentEditor != null && value == currentEditor)
				{
					return;
				}

				currentEditor = value;
				UpdateSplineStates();
			}
		}

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

		public int SelectedCurveIndex
		{
			get => selectedCurveIndex;
			set
			{
				selectedCurveIndex = value;
				UpdateSplineStates();
			}
		}

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
				Save(true);
			}
		}

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
				Save(true);
			}
		}

		public bool DrawCurveSmoothAcuteAngles
		{
			get => drawCurveSmoothAcuteAngles;
			set
			{
				if (drawCurveSmoothAcuteAngles == value)
				{
					return;
				}

				drawCurveSmoothAcuteAngles = value;
				Save(true);
			}
		}

		public float DrawCurveSegmentLength
		{
			get => drawCurveSegmentLength;
			set
			{
				if (drawCurveSegmentLength == value)
				{
					return;
				}

				drawCurveSegmentLength = value;
				Save(true);
			}
		}

		public float DrawCurveFirstPointHook
		{
			get => drawCurveFirstPointHook;
			set
			{
				if (drawCurveFirstPointHook == value)
				{
					return;
				}

				drawCurveFirstPointHook = value;
				Save(true);
			}
		}

		public float DrawCurveSecondPointHook
		{
			get => drawCurveSecondPointHook;
			set
			{
				if (drawCurveSecondPointHook == value)
				{
					return;
				}

				drawCurveSecondPointHook = value;
				Save(true);
			}
		}

		public bool DrawPoints
		{
			get => drawPoints;
			set
			{
				if (drawPoints == value)
				{
					return;
				}

				drawPoints = value;
				Save(true);
			}
		}

		public bool DrawSpline
		{
			get => drawSpline;
			set
			{
				if (drawSpline == value)
				{
					return;
				}

				drawSpline = value;
				Save(true);
			}
		}

		public bool DrawNormals
		{
			get => drawNormals;
			set
			{
				if (drawNormals == value)
				{
					return;
				}

				drawNormals = value;
				Save(true);
			}
		}

		public bool ShowTransformHandle
		{
			get => showTransformHandle;
			set
			{
				if (showTransformHandle == value)
				{
					return;
				}

				showTransformHandle = value;
				Save(true);
			}
		}

		public bool AlwaysDrawSplineOnScene
		{
			get => alwaysDrawSplineOnScene;
			set
			{
				if (alwaysDrawSplineOnScene == value)
				{
					return;
				}

				alwaysDrawSplineOnScene = value;
				Save(true);
			}
		}

		public Tool SavedTool
		{
			get => savedTool;
			set => savedTool = value;
		}

		public bool WasSplineModified
		{
			get => wasSplineModified;
			set => wasSplineModified = value;
		}

		public bool IsRotating
		{
			get => isRotating;
			set => isRotating = value;
		}

		public bool IsScaling
		{
			get => isScaling;
			set => isScaling = value;
		}

		public bool IsDraggingPoint
		{
			get => isDraggingPoint;
			set => isDraggingPoint = value;
		}

		public Vector3 LastScale
		{
			get => lastScale;
			set => lastScale = value;
		}

		public Quaternion LastRotation
		{
			get => lastRotation;
			set => lastRotation = value;
		}

		/// <summary>
		/// Updates state properties in regard to currently selected spline.
		/// </summary>
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