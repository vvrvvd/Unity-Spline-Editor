// <copyright file="SplinePath.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Class used for representing path generated on spline.
	/// It contains information about points, scales, normals, tangents and t parameters to generated points on spline.
	/// </summary>
	[Serializable]
	public class SplinePath
	{
		[SerializeField]
		private Vector3[] scales;
		[SerializeField]
		private Vector3[] points;
		[SerializeField]
		private Vector3[] normals;
		[SerializeField]
		private Vector3[] tangents;
		[SerializeField]
		private float[] parametersT;

		/// <summary>
		/// Initializes a new instance of the <see cref="SplinePath"/> class.
		/// </summary>
		public SplinePath()
		{
			this.Points = new Vector3[0];
			this.Normals = new Vector3[0];
			this.Tangents = new Vector3[0];
			this.Scales = new Vector3[0];
			this.ParametersT = new float[0];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SplinePath"/> class.
		/// </summary>
		/// <param name="points">Array of points positions on path.</param>
		/// <param name="tangents">Array of tangents on path.</param>
		/// <param name="normals">Array of normal vectors on path.</param>
		/// <param name="scales">Array of scales on path.</param>
		/// <param name="parametersT">Array of t parameters on path.</param>
		public SplinePath(Vector3[] points, Vector3[] tangents, Vector3[] normals, Vector3[] scales, float[] parametersT)
		{
			this.Points = points;
			this.Normals = normals;
			this.Tangents = tangents;
			this.Scales = scales;
			this.ParametersT = parametersT;
		}

		/// <summary>
		/// Gets or sets scales for path points.
		/// </summary>
		public Vector3[] Scales
		{
			get => scales;
			set => scales = value;
		}

		/// <summary>
		/// Gets or sets positions for path points.
		/// </summary>
		public Vector3[] Points
		{
			get => points;
			set => points = value;
		}

		/// <summary>
		/// Gets or sets normal vectors for path points.
		/// </summary>
		public Vector3[] Normals
		{
			get => normals;
			set => normals = value;
		}

		/// <summary>
		/// Gets or sets tangents for path points.
		/// </summary>
		public Vector3[] Tangents
		{
			get => tangents;
			set => tangents = value;
		}

		/// <summary>
		/// Gets or sets t parameters of bezier spline for path points.
		/// </summary>
		public float[] ParametersT
		{
			get => parametersT;
			set => parametersT = value;
		}
	}
}