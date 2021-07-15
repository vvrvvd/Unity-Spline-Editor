using UnityEngine;

namespace SplineEditor
{

	[ExecuteAlways]
	[RequireComponent(typeof(BezierSpline))]
	[RequireComponent(typeof(LineRenderer))]
	public class LineRendererSpline : MonoBehaviour
	{
		private const int MinSegmentsCount = 1;
		private const float MinSpacingValue = 0.01f;

		[SerializeField]
		private int segmentsCount = 10;
		[SerializeField]
		private float pointsSpacing = 1f;
		[SerializeField]
		private bool useEvenlySpacedPoints = true;

		private BezierSpline bezierSpline;
		private LineRenderer lineRenderer;
		private SplinePath cachedSplinePath;

		public int SegmentsCount
		{
			get => segmentsCount;
			set
			{
				var clampedValue = Mathf.Max(MinSegmentsCount, value);

				if (segmentsCount == clampedValue)
				{
					return;
				}

				segmentsCount = clampedValue;

				if (!useEvenlySpacedPoints)
				{
					UpdateLinePoints();
				}
			}
		}

		public float PointsSpacing
		{
			get => pointsSpacing;
			set
			{
				var clampedValue = Mathf.Max(MinSpacingValue, value);

				if (pointsSpacing == clampedValue)
				{
					return;
				}

				pointsSpacing = clampedValue;

				if (useEvenlySpacedPoints)
				{
					UpdateLinePoints();
				}
			}
		}

		public bool UseEvenlySpacedPoints
		{
			get => useEvenlySpacedPoints;
			set
			{
				if (useEvenlySpacedPoints == value)
				{
					return;
				}

				useEvenlySpacedPoints = value;
				UpdateLinePoints();
			}
		}

		public BezierSpline BezierSpline => bezierSpline;
		public LineRenderer LineRenderer => lineRenderer;

		private void OnValidate()
		{
			if (bezierSpline == null)
			{
				bezierSpline = GetComponent<BezierSpline>();
			}

			if (lineRenderer == null)
			{
				lineRenderer = GetComponent<LineRenderer>();
			}

		}

		private void OnEnable()
		{
			if (bezierSpline == null)
			{
				bezierSpline = GetComponent<BezierSpline>();
			}

			if (lineRenderer == null)
			{
				lineRenderer = GetComponent<LineRenderer>();
			}

			bezierSpline.OnSplineChanged += UpdateLinePoints;
			UpdateLinePoints();
		}

		private void OnDisable()
		{
			bezierSpline.OnSplineChanged -= UpdateLinePoints;
		}

		[ContextMenu("Update Line Points")]
		public void UpdateLinePoints()
		{
			if (bezierSpline == null || lineRenderer == null)
			{
				return;
			}

			lineRenderer.useWorldSpace = false;

			if (useEvenlySpacedPoints)
			{
				UpdateLinePointsWithEvenlySpacedPoints();
			}
			else
			{
				UpdateLinePointsWithSegments();
			}

			if (bezierSpline.IsLoop)
			{
				lineRenderer.positionCount += 1;
				lineRenderer.SetPosition(lineRenderer.positionCount - 1, bezierSpline.GetPoint(0, false));
			}
		}

		private void UpdateLinePointsWithEvenlySpacedPoints()
		{
			if(cachedSplinePath == null)
			{
				cachedSplinePath = new SplinePath();
			}

			bezierSpline.GetEvenlySpacedPoints(pointsSpacing, cachedSplinePath, useWorldSpace: false);

			var generatedPoints = cachedSplinePath.points;
			var generatedPointsLength = generatedPoints.Length;

			lineRenderer.positionCount = generatedPointsLength;

			for (var i = 0; i < generatedPointsLength; i++)
			{
				var position = generatedPoints[i];
				lineRenderer.SetPosition(i, position);
			}
		}

		private void UpdateLinePointsWithSegments()
		{
			lineRenderer.positionCount = segmentsCount + 1;

			for (var i = 0; i <= segmentsCount; i++)
			{
				var t = (float)i / segmentsCount;
				t = segmentsCount == 0 ? 0 : t;
				var position = bezierSpline.GetPoint(t, false);
				lineRenderer.SetPosition(i, position);
			}

		}

	}

}