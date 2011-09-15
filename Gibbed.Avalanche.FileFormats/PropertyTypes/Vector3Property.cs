using System;
using System.Globalization;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class Vector3Property : IPropertyType
    {
        public float X;
        public float Y;
        public float Z;

        public byte Id { get { return 5; } }
        public string Tag { get { return "vec"; } }
        public bool Inline { get { return false; } }

        public void Default()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
            this.Z = 0.0f;
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            output.WriteValueF32(this.X, littleEndian);
            output.WriteValueF32(this.Y, littleEndian);
            output.WriteValueF32(this.Z, littleEndian);
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            this.X = input.ReadValueF32(littleEndian);
            this.Y = input.ReadValueF32(littleEndian);
            this.Z = input.ReadValueF32(littleEndian);
        }

        public void Parse(string text)
        {
            string[] parts = text.Split(',');

            if (parts.Length != 3)
            {
                throw new InvalidOperationException("Vector3 requires two float values delimited by a comma");
            }

            this.X = float.Parse(parts[0], CultureInfo.InvariantCulture);
            this.Y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            this.Z = float.Parse(parts[2], CultureInfo.InvariantCulture);
        }

        public string Compose()
        {
            return String.Format("{0},{1},{2}",
                this.X.ToString(CultureInfo.InvariantCulture),
                this.Y.ToString(CultureInfo.InvariantCulture),
                this.Z.ToString(CultureInfo.InvariantCulture));
        }
    }
}
