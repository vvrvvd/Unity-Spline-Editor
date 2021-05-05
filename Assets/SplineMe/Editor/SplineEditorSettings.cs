using UnityEngine;

namespace SplineEditor.Editor
{

	[CreateAssetMenu(fileName = "SplineEditorSettings", menuName = "Spline Editor/Settings", order = 1)]
	public class SplineEditorSettings : ScriptableObject
	{

		public GUISkin guiSkin = default;

		[Header("Image buttons")]
		public Texture imageLayoutIcon;
		[Space]
		public Texture addCurveIcon;
		public Texture removeCurveIcon;
		public Texture splitCurveIcon;
		[Space]
		public Texture factorSplineIcon;
		public Texture simplifySplineIcon;
		public Texture castSplineIcon;
		public Texture castToCameraSplineIcon;
		[Space]
		public Texture drawerToolIcon;

	}

}
