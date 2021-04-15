using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMe
{

	[ExecuteInEditMode]
	[RequireComponent(typeof(LineRenderer))]
	public class LineRendererSpline : MonoBehaviour
	{
		public int segmentsCount = 10;
		private int _prevSegmentsCount = 0;

		private BezierSpline spline;
		private LineRenderer lineRenderer;

		private void OnValidate()
		{
			segmentsCount = Mathf.Max(0, segmentsCount);
			if(_prevSegmentsCount!=segmentsCount)
			{
				_prevSegmentsCount = segmentsCount;
				UpdateLinePoints();
			}
		}

		private void UpdateLinePoints()
		{

			if (spline == null)
			{
				spline = GetComponent<BezierSpline>();
			}

			if (lineRenderer == null)
			{
				lineRenderer = GetComponent<LineRenderer>();
			}

			lineRenderer.positionCount = segmentsCount+1;

			for(var i=0; i<=segmentsCount; i++)
			{
				var t = (float)i/segmentsCount;
				t = segmentsCount == 0 ? 0 : t;
				var position = spline.GetPoint(t);
				lineRenderer.SetPosition(i, position);
			}

		}

		private void Update()
		{
			UpdateLinePoints();
		}
	}

}