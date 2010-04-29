using System;
using System.Globalization;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class StringProperty : IPropertyType
    {
        public string Value;

        public byte Id { get { return 3; } }
        public string Tag { get { return "string"; } }
        public bool Inline { get { return false; } }

        public void Default()
        {
            this.Value = "";
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                string s = this.Value;
                s = s.Substring(0, Math.Min(s.Length, 0xFFFF));

                output.WriteValueU16((ushort)s.Length, littleEndian);
                output.WriteStringASCII(s);
            }
            else
            {
                output.WriteStringASCIIZ(this.Value);
            }
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                ushort length = input.ReadValueU16(littleEndian);
                this.Value = input.ReadStringASCII(length, true);
            }
            else
            {
                this.Value = input.ReadStringASCIIZ();
            }
        }

        public void Parse(string text)
        {
            this.Value = text;
        }

        public string Compose()
        {
            if (this.Value.IndexOf('\x07') >= 0)
            {
                return this.Value.Replace('\x07', '_');
            }

            return this.Value;
        }
    }
}
