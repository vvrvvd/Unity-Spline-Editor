// <copyright file="CustomAnimationCurve.cs" company="vvrvvd">
// Copyright (c) vvrvvd. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using UnityEngine;

namespace SplineEditor.MeshGenerator
{
	/// <summary>
	/// Wrapper for Unity's AnimationCurve component that lets detecting curve changes through the editor.
	/// Cannot derive from AnimationCurve due to Unity not serializing derived class.
	/// Should be used with [SerializeReference] field.
	/// </summary>
	[Serializable]
	public class CustomAnimationCurve
	{
		[SerializeField]
		private AnimationCurve animationCurve;

		[SerializeField]
		private float[] previousAnimationCurveKeys;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomAnimationCurve"/> class.
		/// </summary>
		/// <param name="animationCurve">Source animation curve.</param>
		/// <param name="compareKeysCount">How many keys should be compared when checking whether curve was modified.</param>
		public CustomAnimationCurve(AnimationCurve animationCurve, int compareKeysCount)
		{
			this.animationCurve = animationCurve;
			previousAnimationCurveKeys = new float[compareKeysCount];
		}

		/// <summary>
		/// Gets or sets source animation curve.
		/// </summary>
		public AnimationCurve AnimationCurve
		{
			get => animationCurve;
			set => animationCurve = value;
		}

		/// <summary>
		/// Constructs custom animation curve representing constant line.
		/// </summary>
		/// <param name="timeStart">Start t parameter.</param>
		/// <param name="timeEnd">End t parameter.</param>
		/// <param name="value">Constant variable value across the animation curve.</param>
		/// <returns>CustomAnimationCurve with constant line animation curve as a source.</returns>
		public static CustomAnimationCurve Constant(float timeStart, float timeEnd, float value)
		{
			var curve = AnimationCurve.Constant(timeStart, timeEnd, value);
			var customCurve = new CustomAnimationCurve(curve, 10);

			return customCurve;
		}

		/// <summary>
		/// Calculates value on the curve for given t.
		/// </summary>
		/// <param name="t">Curve parameter.</param>
		/// <returns>Calculated value corresponding to t parameter on the curve.</returns>
		public float Evaluate(float t)
		{
			return animationCurve.Evaluate(t);
		}

		/// <summary>
		/// Checks whether curve was modified since last check.
		/// </summary>
		/// <returns>True if the curve was modified and false if it wasn't.</returns>
		public bool CheckWasCurveModified()
		{
			var minTime = animationCurve.keys[0].time;
			var maxTime = animationCurve.keys[animationCurve.keys.Length - 1].time;
			var timeDelta = maxTime - minTime;
			var step = timeDelta / previousAnimationCurveKeys.Length;
			var result = false;
			for (var i = 0; i < previousAnimationCurveKeys.Length; i++)
			{
				var t = step * i;
				var value = animationCurve.Evaluate(t);
				var prevValue = previousAnimationCurveKeys[i];

				result |= value != prevValue;
				previousAnimationCurveKeys[i] = value;
			}

			return result;
		}
	}
}
