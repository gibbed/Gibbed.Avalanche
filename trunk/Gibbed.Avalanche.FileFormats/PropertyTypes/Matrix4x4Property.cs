using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Helpers;
using System.IO;
using System.Globalization;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class Matrix4x4Property : IPropertyType
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;

        public float M21;
        public float M22;
        public float M23;
        public float M24;

        public float M31;
        public float M32;
        public float M33;
        public float M34;

        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public byte Id { get { throw new NotSupportedException(); } }
        public string Tag { get { return "mat4x4"; } }
        public bool Inline { get { return false; } }

        public void Default()
        {
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            output.WriteValueF32(this.M11, littleEndian);
            output.WriteValueF32(this.M12, littleEndian);
            output.WriteValueF32(this.M13, littleEndian);
            output.WriteValueF32(this.M14, littleEndian);

            output.WriteValueF32(this.M21, littleEndian);
            output.WriteValueF32(this.M22, littleEndian);
            output.WriteValueF32(this.M23, littleEndian);
            output.WriteValueF32(this.M24, littleEndian);

            output.WriteValueF32(this.M31, littleEndian);
            output.WriteValueF32(this.M32, littleEndian);
            output.WriteValueF32(this.M33, littleEndian);
            output.WriteValueF32(this.M34, littleEndian);

            output.WriteValueF32(this.M41, littleEndian);
            output.WriteValueF32(this.M42, littleEndian);
            output.WriteValueF32(this.M43, littleEndian);
            output.WriteValueF32(this.M44, littleEndian);
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            this.M11 = input.ReadValueF32(littleEndian);
            this.M12 = input.ReadValueF32(littleEndian);
            this.M13 = input.ReadValueF32(littleEndian);
            this.M14 = input.ReadValueF32(littleEndian);

            this.M21 = input.ReadValueF32(littleEndian);
            this.M22 = input.ReadValueF32(littleEndian);
            this.M23 = input.ReadValueF32(littleEndian);
            this.M24 = input.ReadValueF32(littleEndian);

            this.M31 = input.ReadValueF32(littleEndian);
            this.M32 = input.ReadValueF32(littleEndian);
            this.M33 = input.ReadValueF32(littleEndian);
            this.M34 = input.ReadValueF32(littleEndian);

            this.M41 = input.ReadValueF32(littleEndian);
            this.M42 = input.ReadValueF32(littleEndian);
            this.M43 = input.ReadValueF32(littleEndian);
            this.M44 = input.ReadValueF32(littleEndian);
        }

        public void Parse(string text)
        {
            string[] parts = text.Split(',');
            if (parts.Length != 3 * 4)
            {
                throw new InvalidOperationException("Matrix4x3 requires twelve float values delimited by a comma");
            }

            this.M11 = float.Parse(parts[0], CultureInfo.InvariantCulture);
            this.M12 = float.Parse(parts[1], CultureInfo.InvariantCulture);
            this.M13 = float.Parse(parts[2], CultureInfo.InvariantCulture);
            this.M14 = float.Parse(parts[3], CultureInfo.InvariantCulture);
            this.M21 = float.Parse(parts[4], CultureInfo.InvariantCulture);
            this.M22 = float.Parse(parts[5], CultureInfo.InvariantCulture);
            this.M23 = float.Parse(parts[6], CultureInfo.InvariantCulture);
            this.M24 = float.Parse(parts[7], CultureInfo.InvariantCulture);
            this.M31 = float.Parse(parts[8], CultureInfo.InvariantCulture);
            this.M32 = float.Parse(parts[9], CultureInfo.InvariantCulture);
            this.M33 = float.Parse(parts[10], CultureInfo.InvariantCulture);
            this.M34 = float.Parse(parts[11], CultureInfo.InvariantCulture);
            this.M41 = float.Parse(parts[12], CultureInfo.InvariantCulture);
            this.M42 = float.Parse(parts[13], CultureInfo.InvariantCulture);
            this.M43 = float.Parse(parts[14], CultureInfo.InvariantCulture);
            this.M44 = float.Parse(parts[15], CultureInfo.InvariantCulture);
        }

        public string Compose()
        {
            return String.Format(
                "{0},{1},{2},{3}, {4},{5},{6},{7}, {8},{9},{10},{11}, {12},{13},{14},{15}",
                this.M11.ToString(CultureInfo.InvariantCulture),
                this.M12.ToString(CultureInfo.InvariantCulture),
                this.M13.ToString(CultureInfo.InvariantCulture),
                this.M14.ToString(CultureInfo.InvariantCulture),
                this.M21.ToString(CultureInfo.InvariantCulture),
                this.M22.ToString(CultureInfo.InvariantCulture),
                this.M23.ToString(CultureInfo.InvariantCulture),
                this.M24.ToString(CultureInfo.InvariantCulture),
                this.M31.ToString(CultureInfo.InvariantCulture),
                this.M32.ToString(CultureInfo.InvariantCulture),
                this.M33.ToString(CultureInfo.InvariantCulture),
                this.M34.ToString(CultureInfo.InvariantCulture),
                this.M41.ToString(CultureInfo.InvariantCulture),
                this.M42.ToString(CultureInfo.InvariantCulture),
                this.M43.ToString(CultureInfo.InvariantCulture),
                this.M44.ToString(CultureInfo.InvariantCulture));
        }
    }
}
