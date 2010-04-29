using System;
using System.Globalization;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class Vector2Property : IPropertyType
    {
        public float X;
        public float Y;

        public byte Id { get { return 4; } }
        public string Tag { get { return "vec2"; } }
        public bool Inline { get { return false; } }

        public void Default()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            output.WriteValueF32(this.X, littleEndian);
            output.WriteValueF32(this.Y, littleEndian);
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            this.X = input.ReadValueF32(littleEndian);
            this.Y = input.ReadValueF32(littleEndian);
        }

        public void Parse(string text)
        {
            string[] parts = text.Split(',');
            
            if (parts.Length != 2)
            {
                throw new InvalidOperationException("Vector2 requires two float values delimited by a comma");
            }

            this.X = float.Parse(parts[0], CultureInfo.InvariantCulture);
            this.Y = float.Parse(parts[1], CultureInfo.InvariantCulture);
        }

        public string Compose()
        {
            return String.Format("{0},{1}",
                this.X.ToString(CultureInfo.InvariantCulture),
                this.Y.ToString(CultureInfo.InvariantCulture));
        }
    }
}
