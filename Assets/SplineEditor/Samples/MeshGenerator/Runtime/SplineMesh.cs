using UnityEngine;
using UnityEngine.Assertions;

namespace SplineEditor.MeshGenerator
{

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BezierSpline))]
	[DisallowMultipleComponent]
    [ExecuteAlways]
    public class SplineMesh : MonoBehaviour
    {

        #region Public Fields

        public int segmentsCount = 10;
        public float precision = 0.01f;

        #endregion

        #region Private Fields

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private BezierSpline bezierSpline;

		#endregion

		#region Properties

        public MeshFilter MeshFilter => meshFilter;
        public MeshRenderer MeshRenderer => meshRenderer;
        public BezierSpline BezierSpline => bezierSpline;


		#endregion

		#region Initialize

		private void OnValidate()
		{
            precision = Mathf.Max(precision, 0.001f);
            segmentsCount = Mathf.Max(segmentsCount, 1);
        }

		private void Awake()
		{
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            bezierSpline = GetComponent<BezierSpline>();

            Assert.IsNotNull(meshFilter);
            Assert.IsNotNull(meshRenderer);
            Assert.IsNotNull(bezierSpline);

            bezierSpline.OnSplineChanged += GenerateMesh;
        }

        #endregion

        #region Public Methods

        public void GenerateMesh()
        {

        }

        #endregion

        #region Private Methods

        #endregion

    }

}
