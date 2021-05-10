using System;
using UnityEngine;

namespace SplineEditor
{

	[Serializable]
	public class SplinePoint
	{
		public Vector3 position;

		public SplinePoint(Vector3 position)
		{
			this.position = position;
		}
	}

}
