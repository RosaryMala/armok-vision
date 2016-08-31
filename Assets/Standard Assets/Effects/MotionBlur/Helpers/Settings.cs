// Setting class and enumerations for the motion blur effect

using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    public partial class MotionBlur : MonoBehaviour
    {
        /// Class used for storing settings of MotionBlur.
        [Serializable]
        public class Settings
        {
            /// The angle of rotary shutter. The larger the angle is, the longer
            /// the exposure time is.
            [SerializeField, Range(0, 360)]
            [Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
            public float shutterAngle;

            /// The amount of sample points, which affects quality and performance.
            [SerializeField]
            [Tooltip("The amount of sample points, which affects quality and performance.")]
            public int sampleCount;

            /// The strength of multiple frame blending. The opacity of preceding
            /// frames are determined from this coefficient and time differences.
            [SerializeField, Range(0, 1)]
            [Tooltip("The strength of multiple frame blending")]
            public float frameBlending;

            /// Returns the default settings.
            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        shutterAngle = 270,
                        sampleCount = 10,
                        frameBlending = 0
                    };
                }
            }
        }
    }
}
