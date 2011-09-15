using System;
using System.Globalization;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class FloatProperty : IPropertyType
    {
        public float Value;

        public byte Id { get { return 2; } }
        public string Tag { get { return "float"; } }
        public bool Inline { get { return true; } }

        public void Default()
        {
            this.Value = 0.0f;
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            output.WriteValueF32(this.Value, littleEndian);
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            this.Value = input.ReadValueF32(littleEndian);
        }

        public void Parse(string text)
        {
            this.Value = float.Parse(text, CultureInfo.InvariantCulture);
        }

        public string Compose()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
