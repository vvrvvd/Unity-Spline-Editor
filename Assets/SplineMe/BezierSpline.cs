using System;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMe
{

	public enum BezierControlPointMode : int
	{
		Free = 0,
		Aligned = 1,
		Mirrored = 2
	}

	[Serializable]
	public class SplinePoint
	{
		public Vector3 position;

		public SplinePoint(Vector3 position)
		{
			this.position = position;
		}
	}

	public class BezierSpline : MonoBehaviour
	{

		public int PointsCount => points.Count;
		public List<SplinePoint> Points => points;

		[SerializeField]
		protected List<SplinePoint> points;

		[SerializeField]
		private bool isLoop;

		[SerializeField]
		private List<BezierControlPointMode> modes;

		public int CurveCount => (PointsCount - 1) / 3;

		private bool isRemovingCurve = false;

		public bool IsLoop
		{
			get
			{
				return isLoop;
			}
			set
			{
				isLoop = value;
				if (value == true)
				{
					modes[modes.Count - 1] = modes[0];
					UpdatePoint(0, points[0].position);
				}
			}
		}

		#region Getters & Setters

		public Vector3 GetPoint(float t)
		{
			var i = 0;
			if (t >= 1f)
			{
				t = 1f;
				i = PointsCount - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return transform.TransformPoint(GetPoint(Points[i].position, Points[i + 1].position, Points[i + 2].position, Points[i + 3].position, t));
		}

		public Vector3 GetVelocity(float t)
		{
			var i = 0;
			if (t >= 1f)
			{
				t = 1f;
				i = PointsCount - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return transform.TransformPoint(GetFirstDerivative(Points[i].position, Points[i + 1].position, Points[i + 2].position, Points[i + 3].position, t)) - transform.position;
		}

		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}

		public BezierControlPointMode GetControlPointMode(int index)
		{
			return modes[(index + 1) / 3];
		}

		public void SetControlPointMode(int index, BezierControlPointMode mode)
		{
			var modeIndex = (index + 1) / 3;
			modes[modeIndex] = mode;

			if (isLoop)
			{
				if (modeIndex == 0)
				{
					modes[modes.Count - 1] = mode;
				}
				else if (modeIndex == modes.Count - 1)
				{
					modes[0] = mode;
				}
			}

			ApplyContraints(index);
		}

		private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			var oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * oneMinusT * p0 +
				3f * oneMinusT * oneMinusT * t * p1 +
				3f * oneMinusT * t * t * p2 +
				t * t * t * p3;
		}

		private static Vector3 GetInversePoint1(Vector3 p0, Vector3 p2, Vector3 p3, Vector3 pointOnCurve, float t)
		{
			t = Mathf.Clamp01(t);
			var oneMinusT = 1f - t;
			return
				(pointOnCurve -
				oneMinusT * oneMinusT * oneMinusT * p0 -
				3f * oneMinusT * t * t * p2 -
				t * t * t * p3) /
				(3f * oneMinusT * oneMinusT * t);
		}

		private static Vector3 GetInversePoint2(Vector3 p0, Vector3 p1, Vector3 p3, Vector3 pointOnCurve, float t)
		{
			t = Mathf.Clamp01(t);
			var oneMinusT = 1f - t;
			return
				(pointOnCurve -
				oneMinusT * oneMinusT * oneMinusT * p0 -
				3f * oneMinusT * oneMinusT * t * p1 -
				t * t * t * p3) /
				(3f * oneMinusT * t * t);
		}

		private static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				3f * oneMinusT * oneMinusT * (p1 - p0) +
				6f * oneMinusT * t * (p2 - p1) +
				3f * t * t * (p3 - p2);
		}

		#endregion

		private void Reset()
		{
			modes = new List<BezierControlPointMode>();
			points = new List<SplinePoint>(1000);

			var p0 = new Vector3(1f, 0f, 0f);
			var p1 = new Vector3(2f, 0f, 0f);
			var p2 = new Vector3(3f, 0f, 0f);
			var p3 = new Vector3(4f, 0f, 0f);

			AddPoint(p0);
			AddPoint(p1);
			AddPoint(p2);
			AddPoint(p3);

			modes.Add(BezierControlPointMode.Free);
			modes.Add(BezierControlPointMode.Free);
		}

		public void AddCurve()
		{
			var point = Points[PointsCount - 1].position;
			point.x += 1f;
			AddPoint(point);
			point.x += 1f;
			AddPoint(point);
			point.x += 1f;
			AddPoint(point);

			var prevMode = modes[modes.Count - 1];
			modes.Add(prevMode);
			ApplyContraints(PointsCount - 4);

			if (IsLoop)
			{
				Points[PointsCount - 1].position = Points[0].position;
				modes[modes.Count - 1] = modes[0];
				ApplyContraints(0);
			}
		}

		public void AddCurve(Vector3 p1, Vector3 p2, Vector3 p3, BezierControlPointMode mode)
		{
			AddPoint(p1);
			AddPoint(p2);
			AddPoint(p3);

			modes.Add(mode);
			ApplyContraints(PointsCount - 4);

			if (IsLoop)
			{
				Points[PointsCount - 1].position = Points[0].position;
				modes[modes.Count - 1] = modes[0];
				ApplyContraints(0);
			}
		}

		public void RemoveCurve(int curveIndex)
		{
			var wasRemovingCurve = isRemovingCurve;
			isRemovingCurve = true;
			var isLastCurve = curveIndex == CurveCount;
			var isStartCurve = curveIndex == 0;
			var isMidCurve = IsLoop && curveIndex == 1 && CurveCount == 2;

			if (!wasRemovingCurve && IsLoop && isStartCurve)
			{
				RemoveCurve(CurveCount);
			}

			var beginCurveIndex = curveIndex * 3;
			var startCurveIndex = beginCurveIndex;
			if (isStartCurve)
			{
				startCurveIndex += 1;
			}
			else if (isLastCurve)
			{
				startCurveIndex = PointsCount - 2;
			}
			else if (isMidCurve)
			{
				startCurveIndex += 2;
			}

			
			RemovePoint(startCurveIndex + 1);
			RemovePoint(startCurveIndex);
			RemovePoint(startCurveIndex - 1);
			var modeIndex = (beginCurveIndex + 2) / 3;
			modes.RemoveAt(modeIndex);

			if (!wasRemovingCurve && IsLoop && isLastCurve)
			{
				RemoveCurve(0);
			}

			if (wasRemovingCurve)
			{
				return;
			}

			var nextPointIndex = (isLastCurve || startCurveIndex >= PointsCount) ? PointsCount - 1 : startCurveIndex;

			if (IsLoop && CurveCount == 1)
			{
				IsLoop = false;
			}

			UpdatePoint(nextPointIndex, Points[nextPointIndex].position);
			isRemovingCurve = false;
		}

		public void UpdatePoint(int index, Vector3 position, bool applyConstraints = true)
		{
			if (applyConstraints && index % 3 == 0)
			{
				var delta = position - Points[index].position;
				if (IsLoop)
				{
					if (index == 0)
					{
						Points[1].position += delta;
						Points[PointsCount - 2].position += delta;
						Points[PointsCount - 1].position = position;
					}
					else if (index == PointsCount - 1)
					{
						Points[0].position = position;
						Points[1].position += delta;
						Points[index - 1].position += delta;
					}
					else
					{
						Points[index - 1].position += delta;
						Points[index + 1].position += delta;
					}
				}
				else
				{
					if (index > 0)
					{
						Points[index - 1].position += delta;
					}
					if (index + 1 < PointsCount)
					{
						Points[index + 1].position += delta;
					}
				}
			}

			Points[index].position = position;

			if (applyConstraints)
			{
				ApplyContraints(index);
			}
		}

		private void ApplyContraints(int index)
		{
			var modeIndex = (index + 1) / 3;
			var mode = modes[modeIndex];
			if (mode == BezierControlPointMode.Free || (!IsLoop && (modeIndex == 0 || modeIndex == modes.Count - 1)))
			{
				return;
			}

			var middleIndex = modeIndex * 3;
			int fixedIndex, enforcedIndex;
			if (index <= middleIndex)
			{
				fixedIndex = middleIndex - 1;
				if (fixedIndex < 0)
				{
					fixedIndex = PointsCount - 2;
				}
				enforcedIndex = middleIndex + 1;
				if (enforcedIndex >= PointsCount)
				{
					enforcedIndex = 1;
				}
			}
			else
			{
				fixedIndex = middleIndex + 1;
				if (fixedIndex >= PointsCount)
				{
					fixedIndex = 1;
				}
				enforcedIndex = middleIndex - 1;
				if (enforcedIndex < 0)
				{
					enforcedIndex = PointsCount - 2;
				}
			}

			if (enforcedIndex == PointsCount || fixedIndex == PointsCount)
			{
				return;
			}

			var middle = Points[middleIndex].position;
			var enforcedTangent = middle - Points[fixedIndex].position;

			if (mode == BezierControlPointMode.Aligned)
			{
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, Points[enforcedIndex].position);
			}

			Points[enforcedIndex].position = middle + enforcedTangent;
		}

		private void AddPoint(Vector3 point)
		{
			var nextIndex = PointsCount > 0 ? PointsCount : 0;
			AddPoint(point, nextIndex);
		}

		private void AddPoint(Vector3 point, int index)
		{
			var linePoint = new SplinePoint(point);
			if(index!=PointsCount)
			{
				Points.Insert(index, linePoint);
			} else
			{
				Points.Add(linePoint);
			}
		}

		private void RemovePoint(int index)
		{
			Points.RemoveAt(index);
		}

		public void CastCurve()
		{
			for (var i = 0; i < modes.Count; i++)
			{
				modes[i] = BezierControlPointMode.Free;
			}

			var newPointsPositions = new Vector3[PointsCount];
			for (var i = 0; i < PointsCount; i++)
			{
				var isCorrectPosition = TryCastPoint(i, -transform.up, out newPointsPositions[i]);
				if (!isCorrectPosition)
				{
					continue;
				}

				UpdatePoint(i, newPointsPositions[i], false);
			}

		}

		public bool TryCastPoint(int index, Vector3 direction, out Vector3 castedPoint)
		{
			var point = Points[index];
			var worldPosition = transform.TransformPoint(point.position);
			var isCorrectPosition = Physics.Raycast(worldPosition, direction, out var hit, SplineMeTools.MaxRaycastDistance, Physics.AllLayers);

			castedPoint = isCorrectPosition ? transform.InverseTransformPoint(hit.point) : Vector3.zero;
			return isCorrectPosition;
		}

		public void AddMidCurve(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;

			var p0 = points[startPointIndex].position;
			var p1 = points[startPointIndex + 1].position;

			var t = (curveIndex + 0.5f) / CurveCount;
			var newPoint = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.25f) / CurveCount;
			var pointOnCurve = transform.InverseTransformPoint(GetPoint(t));

			//Left control point
			var leftControlPoint = GetInversePoint2(p0, p1, newPoint, pointOnCurve, 0.5f);

			p0 = newPoint;
			var p2 = points[startPointIndex + 2].position;
			var p3 = points[startPointIndex + 3].position;

			t = (curveIndex + 0.75f) / CurveCount;
			pointOnCurve = transform.InverseTransformPoint(GetPoint(t));

			//Right control point
			var rightControlPoint = GetInversePoint1(newPoint, p2, p3, pointOnCurve, 0.5f);

			AddPoint(leftControlPoint, startPointIndex + 2);
			AddPoint(newPoint, startPointIndex + 3);
			AddPoint(rightControlPoint, startPointIndex + 4);

			var modeIndex = (startPointIndex + 3) / 3;
			modes.Insert(modeIndex, BezierControlPointMode.Free);
		}

		public void FactorCurve()
		{
			for(var i=0; i<CurveCount; i+=2)
			{
				AddMidCurveAndApplyConstraints(i);
			}
		}

		public void SimplifyCurve()
		{
			for (var i = 1; i < CurveCount; i++)
			{
				RemoveCurveAndApplyConstraints(i);
				if (i == CurveCount - 1)
				{
					return;
				}
			}
		}

		public void AddMidCurveAndApplyConstraints(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;

			var p0 = points[startPointIndex].position;

			var t = (curveIndex + 0.5f) / CurveCount;
			var newPoint = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.16f) / CurveCount;
			var pointOnCurve1 = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.33f) / CurveCount;
			var pointOnCurve2 = transform.InverseTransformPoint(GetPoint(t));

			//Left control point
			GetInverseControlPoints(p0, newPoint, pointOnCurve1, pointOnCurve2, 0.32f, 0.66f, out var p1, out var p2);
			var leftControlPoint = p2;
			var updatedP1 = p1;

			var p3 = points[startPointIndex + 3].position;

			t = (curveIndex + 0.66f) / CurveCount;
			pointOnCurve1 = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.83f) / CurveCount;
			pointOnCurve2 = transform.InverseTransformPoint(GetPoint(t));

			//Right control point
			GetInverseControlPoints(newPoint, p3, pointOnCurve1, pointOnCurve2, 0.32f, 0.66f, out p1, out p2);

			var rightControlPoint = p1;
			var updatedP2 = p2;

			UpdatePoint(startPointIndex + 2, updatedP2);
			UpdatePoint(startPointIndex + 1, updatedP1);

			AddPoint(leftControlPoint, startPointIndex + 2);
			AddPoint(newPoint, startPointIndex + 3);
			AddPoint(rightControlPoint, startPointIndex + 4);

			var modeIndex = (startPointIndex + 3) / 3;
			modes.Insert(modeIndex, BezierControlPointMode.Free);
		}

		public void RemoveCurveAndApplyConstraints(int curveIndex)
		{
			var startPointIndex = curveIndex * 3;
			var p0Index = startPointIndex - 3;
			var p3Index = startPointIndex + 3;

			var p0 = points[p0Index].position;
			var p3 = points[p3Index].position;

			var t = (curveIndex - 0.5f) / CurveCount;
			var pointOnCurve1 = transform.InverseTransformPoint(GetPoint(t));

			t = (curveIndex + 0.5f) / CurveCount;
			var pointOnCurve2 = transform.InverseTransformPoint(GetPoint(t));

			GetInverseControlPoints(p0, p3, pointOnCurve1, pointOnCurve2, 0.25f, 0.75f, out var p1, out var p2);
			SetControlPointMode(p0Index, BezierControlPointMode.Free);
			SetControlPointMode(p3Index, BezierControlPointMode.Free);
			UpdatePoint(p0Index + 1, p1);
			UpdatePoint(p3Index - 1, p2);

			RemoveCurve(curveIndex);
		}


		public void GetInverseControlPoints(Vector3 p0, Vector3 p3, Vector3 f, Vector3 g, float u, float v, out Vector3 p1, out Vector3 p2)
		{
			p1 = Vector3.zero;
			p2 = Vector3.zero;

			var oneMinusU = (1f - u);
			var c =
				f -
				(oneMinusU * oneMinusU * oneMinusU * p0) -
				(u * u * u * p3);

			var oneMinusV = (1f - v);
			var d =
				g -
				(oneMinusV * oneMinusV * oneMinusV * p0) -
				(v * v * v * p3);

			var det =
					(3f * oneMinusU * oneMinusU * u * 3f * oneMinusV * v * v) -
					(3f * oneMinusU * u * u * 3f * oneMinusV * oneMinusV * v);

			var m0 = (3f * oneMinusV * v * v) / det;
			var m1 = (-3f * oneMinusU * u * u) / det;
			var m2 = (-3f * oneMinusV * oneMinusV * v) / det;
			var m3 = (3f * oneMinusU * oneMinusU * u) / det;

			var a = new float[,]
			{
				{ m0, m1 }, {m2, m3}
				// | m0 m1 |
				// | m2 m3 |
			};

			var b = new float[,]
			{
				{c.x, c.y, c.z},
				{d.x, d.y, d.z}
				// | c.x d.x |
				// | c.y d.y |
				// | c.z d.z |
			};

			var solution = MultiplyMatrices(a, b);
			p1.x = solution[0, 0];
			p1.y = solution[0, 1];
			p1.z = solution[0, 2];

			p2.x = solution[1, 0];
			p2.y = solution[1, 1];
			p2.z = solution[1, 2];
		}

		private float[,] MultiplyMatrices(float[,] a , float[,] b)
		{
			int m = a.GetLength(0);
			int n = a.GetLength(1);
			int p = b.GetLength(0);
			int q = b.GetLength(1);

			if (n != p)
			{
				Debug.LogError($"Matrix multiplication not possible due to not matching sizes {n} != {p}");
			}

			float[,] c = new float[m, q];

			for (var i = 0; i < m; i++)
			{
				for (var j = 0; j < q; j++)
				{
					c[i, j] = 0;
					for (int k = 0; k < n; k++)
					{
						c[i, j] += a[i, k] * b[k, j];
					}
				}
			}

			return c;
		}
	}

}