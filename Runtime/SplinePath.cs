// <copyright file="SplinePath.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
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
		private List<Vector3> scales;
		[SerializeField]
		private List<Vector3> points;
		[SerializeField]
		private List<Vector3> normals;
		[SerializeField]
		private List<Vector3> tangents;
		[SerializeField]
		private List<float> parametersT;

		/// <summary>
		/// Initializes a new instance of the <see cref="SplinePath"/> class.
		/// </summary>
		public SplinePath()
		{
			this.Points = new List<Vector3>();
			this.Normals = new List<Vector3>();
			this.Tangents = new List<Vector3>();
			this.Scales = new List<Vector3>();
			this.ParametersT = new List<float>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SplinePath"/> class.
		/// </summary>
		/// <param name="points">List of points positions on path.</param>
		/// <param name="tangents">List of tangents on path.</param>
		/// <param name="normals">List of normal vectors on path.</param>
		/// <param name="scales">List of scales on path.</param>
		/// <param name="parametersT">List of t parameters on path.</param>
		public SplinePath(List<Vector3> points, List<Vector3> tangents, List<Vector3> normals, List<Vector3> scales, List<float> parametersT)
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
		public List<Vector3> Scales
		{
			get => scales;
			set => scales = value;
		}

		/// <summary>
		/// Gets or sets positions for path points.
		/// </summary>
		public List<Vector3> Points
		{
			get => points;
			set => points = value;
		}

		/// <summary>
		/// Gets or sets normal vectors for path points.
		/// </summary>
		public List<Vector3> Normals
		{
			get => normals;
			set => normals = value;
		}

		/// <summary>
		/// Gets or sets tangents for path points.
		/// </summary>
		public List<Vector3> Tangents
		{
			get => tangents;
			set => tangents = value;
		}

		/// <summary>
		/// Gets or sets t parameters of bezier spline for path points.
		/// </summary>
		public List<float> ParametersT
		{
			get => parametersT;
			set => parametersT = value;
		}

		/// <summary>
		/// Resets all variables to empty lists.
		/// </summary>
		public void Reset()
		{
			Scales.Clear();
			Points.Clear();
			Normals.Clear();
			Tangents.Clear();
			ParametersT.Clear();
		}
	}
}