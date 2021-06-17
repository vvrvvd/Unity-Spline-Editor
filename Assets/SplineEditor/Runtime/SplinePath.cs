
using UnityEngine;

namespace SplineEditor
{

	public class SplinePath
	{

		public float[] scales;
		public float[] parametersT;
		public Vector3[] points;
		public Vector3[] normals;
		public Vector3[] tangents;

		public SplinePath()
		{
			this.points = new Vector3[0];
			this.normals = new Vector3[0];
			this.tangents = new Vector3[0];
			this.scales = new float[0];
			this.parametersT = new float[0];
		}

		public SplinePath(Vector3[] points, Vector3[] tangents, Vector3[] normals, float[] scales, float[] parametersT)
		{
			this.points = points;
			this.normals = normals;
			this.tangents = tangents;
			this.scales = scales;
			this.parametersT = parametersT;
		}

	}

}