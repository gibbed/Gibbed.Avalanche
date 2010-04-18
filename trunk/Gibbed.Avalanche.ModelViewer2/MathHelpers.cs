using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal static class MathHelpers
    {
        public static float ToDegrees(float radians)
        {
            return (radians * 57.29578f);
        }

        public static float ToRadians(float degrees)
        {
            return (degrees * 0.01745329f);
        }
    }
}
