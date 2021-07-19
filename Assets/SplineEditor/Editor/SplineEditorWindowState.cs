using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	[FilePath("SplineEditor/SplineEditorWindowState.conf", FilePathAttribute.Location.ProjectFolder)]
	public partial class SplineEditorWindowState : ScriptableSingleton<SplineEditorWindowState>
	{

		//GUI
		[SerializeField]
		private bool useText = true;
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

		[SerializeField]
		private bool useImages = true;
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

		//Point
		[SerializeField]
		private bool isPointSectionFolded = true;
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

		private Vector3 previousPointScale = Vector3.one;
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

		private Vector3 previousPointPosition = Vector3.zero;
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

		private BezierControlPointMode previousPointMode = BezierControlPointMode.Free;
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

		//Curve
		[SerializeField]
		private bool isCurveSectionFolded = true;
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

		[SerializeField]
		private float addCurveLength = 1f;
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

		[SerializeField]
		private float splitCurveValue = 0.5f;
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

		//Spline
		[SerializeField]
		private bool isSplineSectionFolded = true;
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

		private Transform customTransform = null;
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

		//Normals
		private bool previousFlipNormals = false;
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

		private bool previousDrawNormals = false;
		public bool PreviousDrawNormals
		{
			get => previousDrawNormals;
			set
			{
				if (previousDrawNormals == value)
				{
					return;
				}

				previousDrawNormals = value;
			}
		}

		[SerializeField]
		private bool isNormalsSectionFolded = true;
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

		private float previousNormalLocalRotation = 0f;
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

		private float previousNormalsGlobalRotation = 0f;
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

		//Drawer Tool
		[SerializeField]
		private bool isDrawerSectionFolded = true;
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

		private float previousSplineLength = 0f;
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