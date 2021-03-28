using System;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMe
{

	[Serializable]
	public class LinePoint
	{
		public Vector3 position;

		public LinePoint(Vector3 position)
		{
			this.position = position;
		}
	}

	public class Polyline : MonoBehaviour
	{
		[SerializeField, NonReorderableAttribute]
		protected List<LinePoint> inspectorPoints;

		public int PointsCount => inspectorPoints.Count;
		public List<LinePoint> Points => reversedPoints;

		//We have to keep reversed list of points to properly dynamically generate and remove new points using shortcuts
		[SerializeField, HideInInspector]
		protected List<LinePoint> reversedPoints;

		public void UpdatePoint(int index, Vector3 position)
		{
			reversedPoints[index].position = position;
			inspectorPoints[PointsCount - index - 1].position = position;
		}

		public void AddPoint(Vector3 point)
		{
			var nextIndex = PointsCount > 0 ? PointsCount : 0;
			AddPoint(point, nextIndex);
		}

		public void AddPoint(Vector3 point, int index)
		{
			var linePoint = new LinePoint(point);
			reversedPoints.Insert(index, linePoint);
			inspectorPoints.Insert(inspectorPoints.Count - index, linePoint);
		}

		public void RemovePoint(int index)
		{
			reversedPoints.RemoveAt(index);
			inspectorPoints.RemoveAt(inspectorPoints.Count - index - 1);
		}

		protected virtual void Reset()
		{
			reversedPoints = new List<LinePoint>();
			inspectorPoints = new List<LinePoint>();

			AddPoint(Vector3.zero);
			AddPoint(Vector3.left);
		}

		protected virtual void OnValidate()
		{

			while (inspectorPoints.Count < reversedPoints.Count)
			{
				reversedPoints.RemoveAt(0);
			}

			var isEmptyList = reversedPoints.Count == 0;
			var index = 0;
			if (isEmptyList)
			{
				for (var i = inspectorPoints.Count - 1; i >= 0; i--)
				{
					var newPoint = inspectorPoints[inspectorPoints.Count - index - 1];
					reversedPoints.Add(newPoint);
					index++;
				}
			}
			else
			{
				for (var i = reversedPoints.Count; i < inspectorPoints.Count; i++)
				{
					var newPoint = inspectorPoints[inspectorPoints.Count - index - 1];
					reversedPoints.Insert(0, newPoint);
					index++;
				}
			}

			for(var i=0; i< reversedPoints.Count; i++)
			{
				reversedPoints[i].position = inspectorPoints[inspectorPoints.Count-i-1].position;
			}


		}

	}

}
