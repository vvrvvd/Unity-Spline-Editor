using UnityEngine;

namespace SplineEditor
{

	[ExecuteInEditMode]
	[RequireComponent(typeof(BezierSpline))]
	[RequireComponent(typeof(LineRenderer))]
	public class LineRendererSpline : MonoBehaviour
	{
		public int segmentsCount = 10;
		private int _prevSegmentsCount = 0;

		private BezierSpline spline;
		private LineRenderer lineRenderer;

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
			spline.OnSplineChanged += UpdateLinePoints;
		}

		private void OnDisable()
		{
			spline.OnSplineChanged -= UpdateLinePoints;
		}

		private void UpdateLinePoints()
		{
			if(spline==null || lineRenderer == null)
			{
				return;
			}

			lineRenderer.positionCount = segmentsCount+1;

			for(var i=0; i<=segmentsCount; i++)
			{
				var t = (float)i/segmentsCount;
				t = segmentsCount == 0 ? 0 : t;
				var position = spline.GetPoint(t);
				lineRenderer.SetPosition(i, position);
			}

			if (spline.IsLoop)
			{
				lineRenderer.positionCount += 1;
				lineRenderer.SetPosition(segmentsCount+1, spline.GetPoint(0));
			}

		}

		private void Update()
		{
			UpdateLinePoints();
		}
	}

}