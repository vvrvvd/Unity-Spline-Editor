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

        public AnimationCurve animationCurve;

        [SerializeField]
        private float[] previousAnimationCurveKeys;

        public CustomAnimationCurve(AnimationCurve animationCurve, int compareKeysCount)
		{
            this.animationCurve = animationCurve;
            previousAnimationCurveKeys = new float[compareKeysCount];
        }

        public static CustomAnimationCurve Constant(float timeStart, float timeEnd, float value)
        {
            var curve = AnimationCurve.Constant(timeStart, timeEnd, value);
            var customCurve = new CustomAnimationCurve(curve, 10);

            return customCurve;
        }

        public bool CheckWasCurveModified()
		{
            var minTime = animationCurve.keys[0].time;
            var maxTime = animationCurve.keys[animationCurve.keys.Length - 1].time;
            var timeDelta = maxTime - minTime;
            var step = timeDelta / previousAnimationCurveKeys.Length;
            var result = false;
            for(var i=0; i<previousAnimationCurveKeys.Length; i++)
			{
                var t = step * i;
                var value = animationCurve.Evaluate(t);
                var prevValue = previousAnimationCurveKeys[i];

                result |= (value != prevValue);
                previousAnimationCurveKeys[i] = value;
			}

            return result;
        }

        public float Evaluate(float t)
        {
            return animationCurve.Evaluate(t);
        }

    }

}
