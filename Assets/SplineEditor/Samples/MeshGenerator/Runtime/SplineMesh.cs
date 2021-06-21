using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SplineEditor.MeshGenerator
{

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BezierSpline))]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class SplineMesh : MonoBehaviour
	{

		#region Enums

		public enum UVMode
		{
			Linear,
			PingPong,
		}

		#endregion

		#region Const Fields

		public const string SplineMeshSettingsName = "SplineMeshSettings";
		
		private const float Precision = 0.0001f;

		#endregion

		#region Public Static Fields

		public static string SettingsPath => $"Resources/{SplineMeshSettingsName}.asset";

		#endregion

		#region Public Fields

		[Space]
		public bool drawPoints = true;
		public bool drawNormals = false;

		[Space]
		public float width = 5f;
		public float spacing = 1f;
		public bool mirrorRightSideCurve = true;
		public AnimationCurve rightSideCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
		public AnimationCurve leftSideCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		[Space]
		public bool usePointsScale = true;

		[Space]
		public UVMode uvMode = UVMode.Linear;

		[Space]
		public bool mirrorUV = false;

		#endregion

		#region Private Fields

		private bool updateMesh;

		[SerializeField, HideInInspector]
		private Mesh cachedMesh;
		[SerializeField, HideInInspector]
		private MeshFilter meshFilter;
		[SerializeField, HideInInspector]
		private MeshRenderer meshRenderer;
		[SerializeField, HideInInspector]
		private BezierSpline bezierSpline;
		[SerializeField, HideInInspector]
		private Material savedMaterial;
		[SerializeField, HideInInspector]
		private bool isVisualizingUV = false;

		#endregion

		#region Internal Fields

		//TODO: Change to property
		[SerializeField, HideInInspector]
		public SplinePath splinePath;

		#endregion

		#region Properties

		/// <summary>
		/// Points on generated on the spline to create the mesh
		/// </summary>
		public Vector3[] Points
		{
			get => splinePath.points;
			set => splinePath.points = value;
		}

		/// <summary>
		/// Normals corresponding to the points on mesh.
		/// Generated based on Tangents.
		/// </summary>
		public Vector3[] Normals
		{
			get => splinePath.normals;
			set => splinePath.normals = value;
		}

		/// <summary>
		/// Tangents of the points on the mesh based on the spline.
		/// </summary>
		public Vector3[] Tangents
		{
			get => splinePath.tangents;
			set => splinePath.tangents = value;
		}

		/// <summary>
		/// Scale of the points on the mesh.
		/// </summary>
		public float[] Scale
		{
			get => splinePath.scales;
			set => splinePath.scales = value;
		}

		/// <summary>
		/// Value of parameter T on the points on mesh regarding to spline.
		/// </summary>
		public float[] ParametersT
		{
			get => splinePath.parametersT;
			set => splinePath.parametersT = value;
		}

		public bool IsVisualizingUV => isVisualizingUV;

		public MeshFilter MeshFilter => meshFilter;
		public MeshRenderer MeshRenderer => meshRenderer;
		public BezierSpline BezierSpline => bezierSpline;

		#endregion

		#region Initialize

		private void OnValidate()
		{
			spacing = Mathf.Max(spacing, 0.1f);

			updateMesh = true;

			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}

			if (meshRenderer == null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
			}

			if (bezierSpline == null)
			{
				bezierSpline = GetComponent<BezierSpline>();
			}

			bezierSpline.OnSplineChanged -= GenerateMesh;
			bezierSpline.OnSplineChanged += GenerateMesh;
		}

		private void Awake()
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}

			if (meshRenderer == null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
			}

			if (bezierSpline == null)
			{
				bezierSpline = GetComponent<BezierSpline>();
			}

			Assert.IsNotNull(meshFilter);
			Assert.IsNotNull(meshRenderer);
			Assert.IsNotNull(bezierSpline);

			bezierSpline.OnSplineChanged -= GenerateMesh;
			bezierSpline.OnSplineChanged += GenerateMesh;

			if(isVisualizingUV)
			{
				ToggleUV();
			}
		}

		private void LateUpdate()
		{
			if (!updateMesh)
			{
				return;
			}

			GenerateMesh();
			updateMesh = false;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Constructs and assigned mesh based on the spline. 
		/// </summary>
		public void GenerateMesh()
		{
			if (BezierSpline == null || MeshFilter == null)
			{
				return;
			}

#if UNITY_EDITOR
			Undo.RecordObject(this, "Generate Bezier Spline Mesh");
#endif

			var splineLength = bezierSpline.GetLinearLength(precision: 0.0001f, useWorldScale: false);
			var curvesCount = bezierSpline.CurvesCount;
			spacing = Mathf.Max(spacing, (splineLength) / (curvesCount * 1000f));

			ConstructMesh();
			meshFilter.sharedMesh = cachedMesh;
		}

		/// <summary>
		/// Constructs mesh based on the spline.
		/// </summary>
		/// <returns></returns>
		public Mesh ConstructMesh()
		{
			int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

			if (splinePath == null)
			{
				splinePath = new SplinePath();
			}

			BezierSpline.RecalculateNormals();
			BezierSpline.GetEvenlySpacedPoints(spacing, splinePath, Precision, false);

			var isLoop = BezierSpline.IsLoop;
			var verts = new Vector3[Points.Length * 2];
			var normals = new Vector3[Points.Length * 2];
			var uvs = new Vector2[verts.Length];
			var numTris = 2 * (Points.Length - 1) + (isLoop ? 2 : 1);
			var tris = new int[numTris * 3];
			var vertIndex = 0;
			var triIndex = 0;

			for (int i = 0; i < Points.Length; i++)
			{
				var normalVector = Normals[i];
				var right = Vector3.Cross(normalVector, Tangents[i]).normalized;
				var rightScaledWidth = width * (usePointsScale ? Scale[i] : 1f) * rightSideCurve.Evaluate(ParametersT[i]);
				var leftScaledWidth = width * (usePointsScale ? Scale[i] : 1f) * leftSideCurve.Evaluate(ParametersT[i]);

				verts[vertIndex] = Points[i] + right * (mirrorRightSideCurve ? -rightScaledWidth : leftScaledWidth);
				verts[vertIndex + 1] = Points[i] + right * rightScaledWidth;

				normals[vertIndex] = normalVector;
				normals[vertIndex + 1] = normalVector;

				var v = GetUV(i);
				uvs[vertIndex] = new Vector2(0, v);
				uvs[vertIndex + 1] = new Vector2(1, v);

				if (i < Points.Length - 1 || isLoop)
				{
					for (int j = 0; j < triangleMap.Length; j++)
					{
						tris[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
					}
				}

				vertIndex += 2;
				triIndex += 6;
			}

			if (cachedMesh == null)
			{
				cachedMesh = new Mesh();
			}

			cachedMesh.Clear();
			cachedMesh.vertices = verts;
			cachedMesh.normals = normals;
			cachedMesh.triangles = tris;
			cachedMesh.uv = uvs;

			return cachedMesh;
		}
		/// <summary>
		/// Toggles mesh material replacement with UV visualization material when it is not shown, and brings back previous material if UV was being shown.
		/// </summary>
		/// <param name="state"></param>
		public void ToggleUV()
		{
			ToggleUV(!isVisualizingUV);
		}

		/// <summary>
		/// Replace current mesh material with UV visualization material when 'state' is true and back to regular material when 'state' is false.
		/// </summary>
		/// <param name="state"></param>
		public void ToggleUV(bool state)
		{
			if (isVisualizingUV == state)
			{
				return;
			}

			var settingsScriptable = Resources.Load<SplineMeshConfiguration>(SplineMeshSettingsName);
			if (settingsScriptable==null)
			{
				Debug.LogError("[Spline Mesh] SplineMeshSettings.asset file cannot be found in Resources folder");
				return;
			}

			isVisualizingUV = state;
			var prevMaterial = meshRenderer.sharedMaterial;
			meshRenderer.sharedMaterial = isVisualizingUV ? settingsScriptable.uvMaterial : savedMaterial;
			savedMaterial = isVisualizingUV ? prevMaterial : settingsScriptable.uvMaterial;

		}

		#endregion

		#region Private Methods

		private float GetUV(int pointIndex)
		{
			var uv = pointIndex / (float)(Points.Length - 1);
			switch (uvMode)
			{
				case UVMode.PingPong:
					uv = 1 - Mathf.Abs(2 * uv - 1);
					break;
				case UVMode.Linear:
				default:
					break;
			}

			return mirrorUV ? 1 - uv : uv;
		}

		#endregion

	}

}
