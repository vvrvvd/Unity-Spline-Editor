using UnityEditor;
using UnityEngine;
using static SplineEditor.BezierSpline;

namespace SplineEditor.Editor
{
	[FilePath("SplineEditor/SplineEditorWindowState.conf", FilePathAttribute.Location.ProjectFolder)]
	public partial class SplineEditorWindowState : ScriptableSingleton<SplineEditorWindowState>
	{

		//GUI
		public bool useText = false;
		public bool useImages = false;

		//Point
		public bool isPointSectionFolded = true;
		public Vector3 previousPointScale = Vector3.one;
		public Vector3 previousPointPosition = Vector3.zero;
		public BezierControlPointMode previousPointMode = BezierControlPointMode.Free;

		//Curve
		public bool isCurveSectionFolded = true;
		public float addCurveLength = 1f;
		public float splitCurveValue = 0.5f;

		//Spline
		public bool isSplineSectionFolded = true;
		public Transform customTransform = null;

		//Normals
		public bool previousFlipNormals = false;
		public bool previousDrawNormals = false;
		public bool isNormalsEditorMode = false;
		public bool isNormalsSectionFolded = true;
		public float previousNormalLocalRotation = 0f;
		public float previousNormalsGlobalRotation = 0f;

		//Drawer Tool
		public bool isDrawerMode = false;
		public bool isDrawerSectionFolded = true;
		public float previousSplineLength = 0f;

	}

}