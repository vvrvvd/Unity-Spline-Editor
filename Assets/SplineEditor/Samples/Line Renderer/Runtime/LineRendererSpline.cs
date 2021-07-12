using UnityEngine;

namespace SplineEditor
{

	[ExecuteAlways]
	[RequireComponent(typeof(BezierSpline))]
	[RequireComponent(typeof(LineRenderer))]
	public class LineRendererSpline : MonoBehaviour
	{
		[SerializeField]
		private int segmentsCount = 10;
		[SerializeField]
		private bool useEvenlySpacedPoints = false;

		private int _prevSegmentsCount = 0;
		private BezierSpline spline;
		private LineRenderer lineRenderer;

		public BezierSpline BezierSpline => spline;
		public LineRenderer LineRenderer => lineRenderer;

		public int SegmentsCount {
			get => segmentsCount;
            set {
				if(segmentsCount == value) {
					return;
                }

				segmentsCount = value;
				UpdateLinePoints();
            }
        }

		private void OnValidate()
		{
			if (spline == null)
			{
				spline = GetComponent<BezierSpline>();
			}

			if (lineRenderer == null)
			{
				lineRenderer = GetComponent<LineRenderer>();
			}

			segmentsCount = Mathf.Max(0, segmentsCount);
			if (_prevSegmentsCount != segmentsCount)
			{
				_prevSegmentsCount = segmentsCount;
				UpdateLinePoints();
			}
		}

		private void OnEnable()
		{
			if (spline == null) {
				spline = GetComponent<BezierSpline>();
			}

			if (lineRenderer == null) {
				lineRenderer = GetComponent<LineRenderer>();
			}

			spline.OnSplineChanged += UpdateLinePoints;
		}

		private void OnDisable()
		{
			spline.OnSplineChanged -= UpdateLinePoints;
		}

		private void UpdateLinePoints() {
			if (spline == null || lineRenderer == null) {
				return;
			}

			lineRenderer.positionCount = segmentsCount + 1;

			for (var i = 0; i <= segmentsCount; i++) {
				var t = (float)i / segmentsCount;
				t = segmentsCount == 0 ? 0 : t;
				var position = spline.GetPoint(t);
				lineRenderer.SetPosition(i, position);
			}

			if (spline.IsLoop) {
				lineRenderer.positionCount += 1;
				lineRenderer.SetPosition(segmentsCount + 1, spline.GetPoint(0));
			}

		}
	}

}