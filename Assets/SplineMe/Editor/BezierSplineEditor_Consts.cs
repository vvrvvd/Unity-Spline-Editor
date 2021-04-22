using UnityEngine;

namespace SplineMe
{


	public class BezierSplineEditor_Consts
	{

		public const int CurveStepsCount = 10;

		public static Color LineStartPointColor => Color.green;
		public static Color LineMidPointColor => Color.white;
		public static Color LineEndPointColor => Color.red;
		public static Color LineColor => Color.red;
		public static Color SelectedLineColor => Color.blue;
		public static Color TangentLineColor => Color.grey;
		public static Color DirectionLineColor => Color.green;
		public static Color SegmentsColor => Color.blue;

		public static Color CurvePointColor => Color.white;
		public static Color SnapEndPointsLineColor => Color.blue;

		public static Color[] ModeColors = {
			Color.green,	//Free
			Color.yellow,	//Aligned
			Color.cyan		//Mirrored
		};

		public const float LineWidth = 2f;
		public const float HandlePointSize = 0.08f;
		public const float HandleSegmentSize = 0.03f;
		public const float PickPointSize = 0.12f;
		public const float DirectionScale = 0.5f;

		public const float CreateCurveSegmentSize = 1f;
		public const float DrawCurveSphereSize = 0.30f;
		public const float DrawCurveSegmentLength = 2f;
		public const float DrawCurveMinLengthToVisualize = DrawCurveSegmentLength * 0.1f;
		public const float DrawCurveFirstControlPointT = 0.33f;
		public const float DrawCurveSecondControlPointT = 0.66f;

		public const float SnapSplineEndPointsMinDistance = 1f;

		public static Color DrawCurvePointColor => Color.blue;

	}

}
