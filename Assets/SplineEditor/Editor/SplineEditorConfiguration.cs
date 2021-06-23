using UnityEngine;

namespace SplineEditor.Editor
{

	//[CreateAssetMenu(fileName = "SplineEditorSettings", menuName = "Spline Editor/Settings", order = 1)]
	public class SplineEditorConfiguration : ScriptableObject
	{

		[Header("General")]
		public bool OpenSplineEditorWithSpline= true;
		public GUISkin guiSkin = default;

		[Header("Point")]
		public bool ScalePointOnScreen = true;
		public float MainPointSize = 0.08f;
		public float TangentPointSize = 0.08f;
		public float BeginAndEndPointsScale = 2f;
		public Color PointColor = Color.white;
		public Color FreeModeColor = Color.green;
		public Color AlignedModeColor = Color.yellow;
		public Color MirroredModeColor = Color.cyan;
		public Color AutoModeColor = Color.grey;

		[Header("Curve")]
		public Color SelectedCurveColor = Color.blue;
		public Color TangentLineColor = Color.grey;

		[Header("Spline")]
		public float SplineWidth = 2f;
		public Color SplineColor = Color.red;


		[Header("Normals")]
		public float NormalVectorLength = 5f;
		public Color NormalsColor = Color.green;


		[Header("Drawer Tool")]
		public bool ScaleDrawerHandleOnScreen = true;
		public float DrawerModeHandleSize = 0.30f;
		public Color DrawerModeHandleColor = Color.green;
		public Color DrawModePointColor = Color.blue;
		public Color DrawerModeCurveColor = Color.blue;

		[Header("Image buttons")]
		public Texture settingsIcon;
		public Texture imageLayoutIcon;
		[Space]
		public Texture applyToAllPointsIcon;
		[Space]
		public Texture addCurveIcon;
		public Texture removeCurveIcon;
		public Texture splitCurveIcon;
		[Space]
		public Texture closeLoopIcon;
		public Texture openLoopIcon;
		public Texture factorSplineIcon;
		public Texture simplifySplineIcon;
		public Texture castSplineIcon;
		public Texture castToCameraSplineIcon;
		[Space]
		public Texture normalsToolIcon;
		[Space]
		public Texture drawerToolIcon;

	}

}
