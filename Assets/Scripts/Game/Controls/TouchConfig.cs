using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    /// <summary>
    /// Important constants describing properties of touch controlling
    /// </summary>
    public static class TouchConfig
    {
        private static readonly double inchInMilimeters = 25.4f;
        private static readonly double dpmm = Screen.dpi / inchInMilimeters;
        private static float AcceptedDistanceForClickInMilimeters { get; } = 1.99f;
        public static float AcceptedDistanceForTapInPixels { get; } = (float)(AcceptedDistanceForClickInMilimeters * dpmm);

        private static float AcceptedDifferenceForDoubleTapInMilimeters { get; } = 8f;
        public static float AcceptedDifferenceForDoubleTapInPixels { get; } = (float)(AcceptedDifferenceForDoubleTapInMilimeters * dpmm);

        public static float TimeBetweenTaps = 1.5f;
        public static float TimeForValidTap = 0.7f;
    }
}
