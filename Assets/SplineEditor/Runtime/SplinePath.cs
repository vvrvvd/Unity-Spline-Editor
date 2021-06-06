
using UnityEngine;

namespace SplineEditor
{

	public class SplinePath
	{

		public Vector3[] points;
		public Vector3[] normals;
		public Vector3[] tangents;

		public SplinePath()
		{
			this.points = new Vector3[0];
			this.normals = new Vector3[0];
			this.tangents = new Vector3[0];
		}

		public SplinePath(Vector3[] points, Vector3[] tangents, Vector3[] normals)
		{
			this.points = points;
			this.normals = normals;
			this.tangents = tangents;
		}

	}

}