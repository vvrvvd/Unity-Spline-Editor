
using UnityEngine;

namespace SplineEditor
{

	public class SplinePath
	{

		public Vector3[] points;
		public Vector3[] tangents;

		public SplinePath(Vector3[] points, Vector3[] tangents)
		{
			this.points = points;
			this.tangents = tangents;
		}

	}

}