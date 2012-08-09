/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Runtime.InteropServices;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal static class ShaderNatives
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct bool4
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool X;

            [MarshalAs(UnmanagedType.Bool)]
            public bool Y;

            [MarshalAs(UnmanagedType.Bool)]
            public bool Z;

            [MarshalAs(UnmanagedType.Bool)]
            public bool W;

            public bool4(bool x, bool y, bool z, bool w)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.W = w;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct float4
        {
            public float X;
            public float Y;
            public float Z;
            public float W;

            public float4(float x, float y, float z, float w)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.W = w;
            }

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

            public float3x4(float v11, float v12, float v13, float v14, float v21, float v22, float v23, float v24, float v31, float v32, float v33, float v34)
            {
                this.V11 = v11;
                this.V12 = v12;
                this.V13 = v13;
                this.V14 = v14;
                this.V21 = v21;
                this.V22 = v22;
                this.V23 = v23;
                this.V24 = v24;
                this.V31 = v31;
                this.V32 = v32;
                this.V33 = v33;
                this.V34 = v34;
            }

            public static implicit operator float3x4(SlimDX.Matrix matrix)
            {
                var value = new float3x4();
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
                return value;
            }
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
