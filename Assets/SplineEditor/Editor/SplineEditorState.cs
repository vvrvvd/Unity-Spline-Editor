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

		public event Action OnSplineModified;
		public event Action OnSelectedPointChanged;
		public event Action OnSelectedSplineChanged;

		private BezierSpline currentSpline;
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

		private SplineEditor currentEditor;
		public SplineEditor CurrentEditor
		{
			get => currentEditor;
			set
			{
				if (currentEditor!=null && value == currentEditor)
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

		[SerializeField]
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
				Save(true);
			}
		}

		[SerializeField]
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
				Save(true);
			}
		}

		[SerializeField]
		private bool drawCurveSmoothAcuteAngles = true;
		public bool DrawCurveSmoothAcuteAngles { 
			get => drawCurveSmoothAcuteAngles; 
			set 
			{ 
				if(drawCurveSmoothAcuteAngles == value)
				{
					return;
				}

				drawCurveSmoothAcuteAngles = value; 
				Save(true);
			}
		}

		[SerializeField]
		private float drawCurveSegmentLength = 5f;
		public float DrawCurveSegmentLength { 
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

		[SerializeField]
		private float drawCurveFirstPointHook = 0.33f;
		public float DrawCurveFirstPointHook { 
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

		[SerializeField]
		private float drawCurveSecondPointHook = 0.66f;
		public float DrawCurveSecondPointHook { 
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

		[SerializeField]
		private bool drawPoints = true;
		public bool DrawPoints { 
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

		[SerializeField]
		private bool drawSpline = true;
		public bool DrawSpline { 
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

		[SerializeField]
		private bool drawNormals = false;
		public bool DrawNormals { 
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

		[SerializeField]
		private bool showTransformHandle = true;
		public bool ShowTransformHandle { 
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

		[SerializeField]
		private bool alwaysDrawSplineOnScene = true;
		public bool AlwaysDrawSplineOnScene { 
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

		[NonSerialized]
		public Tool savedTool = Tool.None;
		[NonSerialized]
		public bool wasSplineModified = false;

		[NonSerialized]
		public bool isRotating;
		[NonSerialized]
		public bool isScaling;
		[NonSerialized]
		public bool isDraggingPoint;

		[NonSerialized]
		public Vector3 lastScale;
		[NonSerialized]
		public Quaternion lastRotation;

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