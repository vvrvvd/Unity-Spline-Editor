using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMe.Editor
{

	[CreateAssetMenu(fileName = "SplineEditorSettings", menuName = "Spline Editor/Settings", order = 1)]
	public class SplineEditorSettings : ScriptableObject
	{

		public GUISkin guiSkin = default;

	}

}
