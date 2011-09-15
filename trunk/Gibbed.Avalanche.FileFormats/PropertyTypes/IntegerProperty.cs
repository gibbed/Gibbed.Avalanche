using System;
using System.Globalization;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class IntegerProperty : IPropertyType
    {
        public int Value;

        public byte Id { get { return 1; } }
        public string Tag { get { return "int"; } }
        public bool Inline { get { return true; } }

        public void Default()
        {
            this.Value = 0;
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            output.WriteValueS32(this.Value, littleEndian);
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            this.Value = input.ReadValueS32(littleEndian);
        }

        public void Parse(string text)
        {
            this.Value = int.Parse(text, CultureInfo.InvariantCulture);
        }

        public string Compose()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
