using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal static class ShaderNatives
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct float4
        {
            public float X;
            public float Y;
            public float Z;
            public float W;

            public static implicit operator float4(SlimDX.Vector4 vector)
            {
                var value = new float4();
                value.X = vector.X;
                value.Y = vector.Y;
                value.Z = vector.Z;
                value.W = vector.W;
                return value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct float3x4
        {
            public float V11;
            public float V12;
            public float V13;
            public float V14;
            public float V21;
            public float V22;
            public float V23;
            public float V24;
            public float V31;
            public float V32;
            public float V33;
            public float V34;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct float4x4
        {
            public float V11;
            public float V12;
            public float V13;
            public float V14;
            public float V21;
            public float V22;
            public float V23;
            public float V24;
            public float V31;
            public float V32;
            public float V33;
            public float V34;
            public float V41;
            public float V42;
            public float V43;
            public float V44;

            public static implicit operator float4x4(SlimDX.Matrix matrix)
            {
                var value = new float4x4();
                value.V11 = matrix.M11;
                value.V12 = matrix.M12;
                value.V13 = matrix.M13;
                value.V14 = matrix.M14;
                value.V21 = matrix.M21;
                value.V22 = matrix.M22;
                value.V23 = matrix.M23;
                value.V24 = matrix.M24;
                value.V31 = matrix.M31;
                value.V32 = matrix.M32;
                value.V33 = matrix.M33;
                value.V34 = matrix.M34;
                value.V41 = matrix.M41;
                value.V42 = matrix.M42;
                value.V43 = matrix.M43;
                value.V44 = matrix.M44;
                return value;
            }
        }
    }
}
