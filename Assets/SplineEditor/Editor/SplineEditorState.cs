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

		/// <summary>
		/// Invoked when current spline is modified.
		/// </summary>
		public event Action OnSplineModified;

		/// <summary>
		/// Invoked when selected point is changed on scene GUI.
		/// </summary>
		public event Action OnSelectedPointChanged;

		/// <summary>
		/// Invoked when selected spline is changed on scene GUI.
		/// </summary>
		public event Action OnSelectedSplineChanged;

		/// <summary>
		/// Gets or sets a BezierSpline currently selected in hierarchy.
		/// </summary>
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

		/// <summary>
		/// Gets or sets splineEditor for BezierSpline currently selected in hierarchy.
		/// </summary>
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

		/// <summary>
		/// Gets or sets control point index currently selected on scene GUI.
		/// </summary>
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

		/// <summary>
		/// Gets or sets curve index currently selected on scene GUI.
		/// </summary>
		public int SelectedCurveIndex
		{
			get => selectedCurveIndex;
			set
			{
				selectedCurveIndex = value;
				UpdateSplineStates();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether any control point is selected on scene GUI.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether selected curve may be removed from CurrentSpline.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether a new curve may be added to CurrentSpline.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether CurrentSpline is looped.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether CurrentSpline can be looped.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether CurrentSpline can be simplified.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether Draw Curve Mode is enabled.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether Normals Editor Mode is enabled.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether angles should be smoothed when generating curves in Draw Curve Mode.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a new curve length when generating curves in Draw Curve Mode.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the first point hook (parameter t) when generating curves in Draw Curve Mode.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the second point hook (parameter t) when generating curves in Draw Curve Mode.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether control points should be drawn on scene GUI.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether splines should be drawn on scene GUI.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether normal vectors should be drawn on scene GUI.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether spline Transform handle should be visible on scene GUI.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a value indicating whether spline should be drawn on scene GUI even if not selected.
		/// </summary>
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

		/// <summary>
		/// Gets or sets a tool saved when hiding Unity tools handle.
		/// </summary>
		public Tool SavedTool
		{
			get => savedTool;
			set => savedTool = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether CurrentSpline was modified in the current frame.
		/// </summary>
		public bool WasSplineModified
		{
			get => wasSplineModified;
			set => wasSplineModified = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether selected control point or normal vector (when Normals Editor Mode is enabled) handle is rotated on scene GUI in the current frame.
		/// </summary>
		public bool IsRotating
		{
			get => isRotating;
			set => isRotating = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether selected control point scale handle is modified on scene GUI in the current frame.
		/// </summary>
		public bool IsScaling
		{
			get => isScaling;
			set => isScaling = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether selected control point position handle is modified on scene GUI in the current frame.
		/// </summary>
		public bool IsDraggingPoint
		{
			get => isDraggingPoint;
			set => isDraggingPoint = value;
		}

		/// <summary>
		/// Gets or sets selected control point last scale on scene GUI when modyfing point scale handle.
		/// </summary>
		public Vector3 LastScale
		{
			get => lastScale;
			set => lastScale = value;
		}

		/// <summary>
		/// Gets or sets selected control point last rotation on scene GUI when modyfing point rotation handle.
		/// </summary>
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