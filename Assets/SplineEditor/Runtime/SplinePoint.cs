// <copyright file="SplinePoint.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using UnityEngine;

namespace SplineEditor
{
	/// <summary>
	/// Class for keeping spline positions.
	/// </summary>
	[Serializable]
	public class SplinePoint
	{
		[SerializeField]
		private Vector3 position;

		/// <summary>
		/// Initializes a new instance of the <see cref="SplinePoint"/> class.
		/// </summary>
		/// <param name="position">Point initial position.</param>
		public SplinePoint(Vector3 position)
		{
			this.Position = position;
		}

		/// <summary>
		/// Gets or sets point position.
		/// </summary>
		public Vector3 Position { get => position; set => position = value; }
	}
}
