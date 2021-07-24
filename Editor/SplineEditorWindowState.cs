// <copyright file="SplineEditorWindowState.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Class providing serializable state for SplineEditor.
	/// </summary>
	[FilePath("UserSettings/SplineEditor/SplineEditorWindowState.asset", FilePathAttribute.Location.ProjectFolder)]
	public partial class SplineEditorWindowState : ScriptableSingleton<SplineEditorWindowState>
	{
		// GUI
		[SerializeField]
		private bool useText = true;
		[SerializeField]
		private bool useImages = true;

		// Point
		[SerializeField]
		private bool isPointSectionFolded = true;
		private Vector3 previousPointScale = Vector3.one;
		private Vector3 previousPointPosition = Vector3.zero;
		private BezierControlPointMode previousPointMode = BezierControlPointMode.Free;

		[SerializeField]
		private bool isCurveSectionFolded = true;
		[SerializeField]
		private float addCurveLength = 1f;
		[SerializeField]
		private float splitCurveValue = 0.5f;

		// Spline
		[SerializeField]
		private bool isSplineSectionFolded = true;
		private Transform customTransform = null;

		// Normals
		private bool previousFlipNormals = false;
		[SerializeField]
		private bool isNormalsSectionFolded = true;
		private float previousNormalLocalRotation = 0f;
		private float previousNormalsGlobalRotation = 0f;

		// Drawer Tool
		[SerializeField]
		private bool isDrawerSectionFolded = true;
		private float previousSplineLength = 0f;

		/// <summary>
		/// Gets or sets a value indicating whether GUI displays text on buttons.
		/// </summary>
		public bool UseText
		{
			get => useText;
			set
			{
				if (useText == value)
				{
					return;
				}

				useText = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether GUI displays image icons on buttons.
		/// </summary>
		public bool UseImages
		{
			get => useImages;
			set
			{
				if (useImages == value)
				{
					return;
				}

				useImages = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Point options section is folded.
		/// </summary>
		public bool IsPointSectionFolded
		{
			get => isPointSectionFolded;
			set
			{
				if (isPointSectionFolded == value)
				{
					return;
				}

				isPointSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets selected point position from the last available frame.
		/// </summary>
		public Vector3 PreviousPointPosition
		{
			get => previousPointPosition;
			set
			{
				if (previousPointPosition == value)
				{
					return;
				}

				previousPointPosition = value;
			}
		}

		/// <summary>
		/// Gets or sets selected point scale from the last available frame.
		/// </summary>
		public Vector3 PreviousPointScale
		{
			get => previousPointScale;
			set
			{
				if (previousPointScale == value)
				{
					return;
				}

				previousPointScale = value;
			}
		}

		/// <summary>
		/// Gets or sets selected point mode from the last available frame.
		/// </summary>
		public BezierControlPointMode PreviousPointMode
		{
			get => previousPointMode;
			set
			{
				if (previousPointMode == value)
				{
					return;
				}

				previousPointMode = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Curve options section is folded.
		/// </summary>
		public bool IsCurveSectionFolded
		{
			get => isCurveSectionFolded;
			set
			{
				if (isCurveSectionFolded == value)
				{
					return;
				}

				isCurveSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets new curves initial length.
		/// </summary>
		public float AddCurveLength
		{
			get => addCurveLength;
			set
			{
				if (addCurveLength == value)
				{
					return;
				}

				addCurveLength = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets split curve value (parameter t to be used for getting split point on a bezier curve).
		/// </summary>
		public float SplitCurveValue
		{
			get => splitCurveValue;
			set
			{
				if (splitCurveValue == value)
				{
					return;
				}

				splitCurveValue = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Spline options section is folded.
		/// </summary>
		public bool IsSplineSectionFolded
		{
			get => isSplineSectionFolded;
			set
			{
				if (isSplineSectionFolded == value)
				{
					return;
				}

				isSplineSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets custom transform for casting spline.
		/// </summary>
		public Transform CustomTransform
		{
			get => customTransform;
			set
			{
				if (customTransform == value)
				{
					return;
				}

				customTransform = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Normals Editor Mode options section is folded.
		/// </summary>
		public bool IsNormalsSectionFolded
		{
			get => isNormalsSectionFolded;
			set
			{
				if (isNormalsSectionFolded == value)
				{
					return;
				}

				isNormalsSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether normals were flipped in the last available frame.
		/// </summary>
		public bool PreviousFlipNormals
		{
			get => previousFlipNormals;
			set
			{
				if (previousFlipNormals == value)
				{
					return;
				}

				previousFlipNormals = value;
			}
		}

		/// <summary>
		/// Gets or sets selected point normal rotation offset from the last available frame.
		/// </summary>
		public float PreviousNormalLocalRotation
		{
			get => previousNormalLocalRotation;
			set
			{
				if (previousNormalLocalRotation == value)
				{
					return;
				}

				previousNormalLocalRotation = value;
			}
		}

		/// <summary>
		/// Gets or sets selected point normal rotation offset from the last available frame.
		/// </summary>
		public float PreviousNormalsGlobalRotation
		{
			get => previousNormalsGlobalRotation;
			set
			{
				if (previousNormalsGlobalRotation == value)
				{
					return;
				}

				previousNormalsGlobalRotation = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Draw Curve Mode options section is folded.
		/// </summary>
		public bool IsDrawerSectionFolded
		{
			get => isDrawerSectionFolded;
			set
			{
				if (isDrawerSectionFolded == value)
				{
					return;
				}

				isDrawerSectionFolded = value;
				Save(true);
			}
		}

		/// <summary>
		/// Gets or sets bezier spline length from the last available frame.
		/// </summary>
		public float PreviousSplineLength
		{
			get => previousSplineLength;
			set
			{
				if (previousSplineLength == value)
				{
					return;
				}

				previousSplineLength = value;
			}
		}
	}
}