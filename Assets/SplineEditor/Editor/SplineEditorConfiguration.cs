// <copyright file="SplineEditorConfiguration.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using UnityEngine;

namespace SplineEditor.Editor
{
	/// <summary>
	/// Scriptable object containing SplineEditor configuration.
	/// </summary>
	[CreateAssetMenu(fileName = "SplineEditorSettings", menuName = "Spline Editor/Spline Editor Configuration", order = 1)]
	public class SplineEditorConfiguration : ScriptableObject
	{
		private static SplineEditorConfiguration instance;

		[Header("General")]
		[SerializeField]
		private bool openSplineEditorWithSpline = true;
		[SerializeField]
		private GUISkin guiSkin = default;

		[Header("Point")]
		[SerializeField]
		private bool scalePointOnScreen = true;
		[SerializeField]
		private float mainPointSize = 0.08f;
		[SerializeField]
		private float tangentPointSize = 0.08f;
		[SerializeField]
		private Color pointColor = Color.white;
		[SerializeField]
		private Color freeModeColor = Color.green;
		[SerializeField]
		private Color alignedModeColor = Color.yellow;
		[SerializeField]
		private Color mirroredModeColor = Color.cyan;
		[SerializeField]
		private Color autoModeColor = Color.grey;

		[Header("Curve")]
		[SerializeField]
		private Color selectedCurveColor = Color.blue;
		[SerializeField]
		private Color tangentLineColor = Color.grey;

		[Header("Spline")]
		[SerializeField]
		private float splineWidth = 2f;
		[SerializeField]
		private Color splineColor = Color.red;

		[Header("Normals")]
		[SerializeField]
		private float normalVectorLength = 5f;
		[SerializeField]
		private Color normalsColor = Color.green;

		[Header("Drawer Tool")]
		[SerializeField]
		private bool scaleDrawerHandleOnScreen = true;
		[SerializeField]
		private float drawerModeHandleSize = 0.30f;
		[SerializeField]
		private Color drawerModeHandleColor = Color.green;
		[SerializeField]
		private Color drawModePointColor = Color.blue;
		[SerializeField]
		private Color drawerModeCurveColor = Color.blue;

		[Header("Image buttons")]
		[SerializeField]
		private Texture settingsIcon;
		[SerializeField]
		private Texture imageLayoutIcon;

		[Space]

		[SerializeField]
		private Texture applyToAllPointsIcon;

		[Space]

		[SerializeField]
		private Texture addCurveIcon;
		[SerializeField]
		private Texture removeCurveIcon;
		[SerializeField]
		private Texture splitCurveIcon;

		[Space]

		[SerializeField]
		private Texture closeLoopIcon;
		[SerializeField]
		private Texture openLoopIcon;
		[SerializeField]
		private Texture factorSplineIcon;
		[SerializeField]
		private Texture simplifySplineIcon;
		[SerializeField]
		private Texture castSplineIcon;

		[SerializeField]
		private Texture castToCameraSplineIcon;

		[Space]

		[SerializeField]
		private Texture normalsToolIcon;

		[Space]

		[SerializeField]
		private Texture drawerToolIcon;

		/// <summary>
		/// Gets the first SplineEditor configuration object found in the resources.
		/// Caches the result so it's loaded from resources only once.
		/// </summary>
		public static SplineEditorConfiguration Instance
		{
			get
			{
				if (instance == null)
				{
					var loadedInstances = Resources.LoadAll<SplineEditorConfiguration>(string.Empty);

					if (loadedInstances.Length == 0)
					{
						Debug.LogError("[SplineEditor] There is a missing editor settings scriptable for Spline Editor in Resources. Create new settings through \"Spline Editor / Spline Editor Configuration\" and put them in Resources folder.");
						return null;
					}

					instance = loadedInstances[0];
				}

				return instance;
			}
		}

		/// <summary>
		/// Gets or sets Draw Curve Mode tool icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture DrawerToolIcon
		{
			get => drawerToolIcon;
			set => drawerToolIcon = value;
		}

		/// <summary>
		/// Gets or sets Normals Editor Mode tool icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture NormalsToolIcon
		{
			get => normalsToolIcon;
			set => normalsToolIcon = value;
		}

		/// <summary>
		/// Gets or sets cast to camera view icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture CastToCameraSplineIcon
		{
			get => castToCameraSplineIcon;
			set => castToCameraSplineIcon = value;
		}

		/// <summary>
		/// Gets or sets cast to spline icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture CastSplineIcon
		{
			get => castSplineIcon;
			set => castSplineIcon = value;
		}

		/// <summary>
		/// Gets or sets simplify spline icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture SimplifySplineIcon
		{
			get => simplifySplineIcon;
			set => simplifySplineIcon = value;
		}

		/// <summary>
		/// Gets or sets factor spline icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture FactorSplineIcon
		{
			get => factorSplineIcon;
			set => factorSplineIcon = value;
		}

		/// <summary>
		/// Gets or sets open spline loop icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture OpenLoopIcon
		{
			get => openLoopIcon;
			set => openLoopIcon = value;
		}

		/// <summary>
		/// Gets or sets close spline loop icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture CloseLoopIcon
		{
			get => closeLoopIcon;
			set => closeLoopIcon = value;
		}

		/// <summary>
		/// Gets or sets split spline icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture SplitCurveIcon
		{
			get => splitCurveIcon;
			set => splitCurveIcon = value;
		}

		/// <summary>
		/// Gets or sets remove selected curve icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture RemoveCurveIcon
		{
			get => removeCurveIcon;
			set => removeCurveIcon = value;
		}

		/// <summary>
		/// Gets or sets add new curve icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture AddCurveIcon
		{
			get => addCurveIcon;
			set => addCurveIcon = value;
		}

		/// <summary>
		/// Gets or sets apply mode to all points icon for a button in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture ApplyToAllPointsIcon
		{
			get => applyToAllPointsIcon;
			set => applyToAllPointsIcon = value;
		}

		/// <summary>
		/// Gets or sets use image buttons icon in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture ImageLayoutIcon
		{
			get => imageLayoutIcon;
			set => imageLayoutIcon = value;
		}

		/// <summary>
		/// Gets or sets settings icon in SplineEditor window.
		/// Is serialized.
		/// </summary>
		public Texture SettingsIcon
		{
			get => settingsIcon;
			set => settingsIcon = value;
		}

		/// <summary>
		/// Gets or sets color for curves drawn during Draw Curve Mode.
		/// Is serialized.
		/// </summary>
		public Color DrawerModeCurveColor
		{
			get => drawerModeCurveColor;
			set => drawerModeCurveColor = value;
		}

		/// <summary>
		/// Gets or sets color for Draw Curve Mode points indicator on scene.
		/// Is serialized.
		/// </summary>
		public Color DrawModePointColor
		{
			get => drawModePointColor;
			set => drawModePointColor = value;
		}

		/// <summary>
		/// Gets or sets color for Draw Curve Mode handle on scene.
		/// Is serialized.
		/// </summary>
		public Color DrawerModeHandleColor
		{
			get => drawerModeHandleColor;
			set => drawerModeHandleColor = value;
		}

		/// <summary>
		/// Gets or sets size for Draw Curve Mode handle on scene.
		/// Is serialized.
		/// </summary>
		public float DrawerModeHandleSize
		{
			get => drawerModeHandleSize;
			set => drawerModeHandleSize = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether scene handle should be scaled to screen size in Draw Curve Mode.
		/// Is serialized.
		/// </summary>
		public bool ScaleDrawerHandleOnScreen
		{
			get => scaleDrawerHandleOnScreen;
			set => scaleDrawerHandleOnScreen = value;
		}

		/// <summary>
		/// Gets or sets color for normal vectors drawn on scene.
		/// Is serialized.
		/// </summary>
		public Color NormalsColor
		{
			get => normalsColor;
			set => normalsColor = value;
		}

		/// <summary>
		/// Gets or sets length for normal vectors drawn on scene.
		/// Is serialized.
		/// </summary>
		public float NormalVectorLength
		{
			get => normalVectorLength;
			set => normalVectorLength = value;
		}

		/// <summary>
		/// Gets or sets color for splines drawn on scene.
		/// Is serialized.
		/// </summary>
		public Color SplineColor
		{
			get => splineColor;
			set => splineColor = value;
		}

		/// <summary>
		/// Gets or sets width for splines drawn on scene.
		/// Is serialized.
		/// </summary>
		public float SplineWidth
		{
			get => splineWidth;
			set => splineWidth = value;
		}

		/// <summary>
		/// Gets or sets color for tangent lines drawn on scene.
		/// Is serialized.
		/// </summary>
		public Color TangentLineColor
		{
			get => tangentLineColor;
			set => tangentLineColor = value;
		}

		/// <summary>
		/// Gets or sets color for selected curve on scene.
		/// Is serialized.
		/// </summary>
		public Color SelectedCurveColor
		{
			get => selectedCurveColor;
			set => selectedCurveColor = value;
		}

		/// <summary>
		/// Gets or sets color for auto control points on scene.
		/// Is serialized.
		/// </summary>
		public Color AutoModeColor
		{
			get => autoModeColor;
			set => autoModeColor = value;
		}

		/// <summary>
		/// Gets or sets color for mirrored control points on scene.
		/// Is serialized.
		/// </summary>
		public Color MirroredModeColor
		{
			get => mirroredModeColor;
			set => mirroredModeColor = value;
		}

		/// <summary>
		/// Gets or sets color for aligned control points on scene.
		/// Is serialized.
		/// </summary>
		public Color AlignedModeColor
		{
			get => alignedModeColor;
			set => alignedModeColor = value;
		}

		/// <summary>
		/// Gets or sets color for free control points on scene.
		/// Is serialized.
		/// </summary>
		public Color FreeModeColor
		{
			get => freeModeColor;
			set => freeModeColor = value;
		}

		/// <summary>
		/// Gets or sets color for main control points on scene.
		/// Is serialized.
		/// </summary>
		public Color PointColor
		{
			get => pointColor;
			set => pointColor = value;
		}

		/// <summary>
		/// Gets or sets size for control points on scene (p1 and p2).
		/// Is serialized.
		/// </summary>
		public float TangentPointSize
		{
			get => tangentPointSize;
			set => tangentPointSize = value;
		}

		/// <summary>
		/// Gets or sets size for main control points on scene (p0 and p3).
		/// Is serialized.
		/// </summary>
		public float MainPointSize
		{
			get => mainPointSize;
			set => mainPointSize = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether control points should be scaled to screen size on scene.
		/// Is serialized.
		/// </summary>
		public bool ScalePointOnScreen
		{
			get => scalePointOnScreen;
			set => scalePointOnScreen = value;
		}

		/// <summary>
		/// Gets or sets skin used for drawing SplineEditor window.
		/// Is serialized.
		/// </summary>
		public GUISkin GuiSkin
		{
			get => guiSkin;
			set => guiSkin = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether SplineEditor window should be automatically opened when BezierSpline component is selected in hierarchy.
		/// Is serialized.
		/// </summary>
		public bool OpenSplineEditorWithSpline
		{
			get => openSplineEditorWithSpline;
			set => openSplineEditorWithSpline = value;
		}
	}
}
