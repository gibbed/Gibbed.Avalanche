using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class IntegerListProperty : IPropertyType
    {
        public List<int> Values;

        public byte Id { get { return 9; } }
        public string Tag { get { return "vec_int"; } }
        public bool Inline { get { return false; } }

        public void Default()
        {
            this.Values = new List<int>();
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                output.WriteValueS32(this.Values.Count, littleEndian);
                foreach (int value in this.Values)
                {
                    output.WriteValueS32(value, littleEndian);
                }
            }
            else
            {
                output.WriteValueS32(this.Values.Count * 4, littleEndian);
                foreach (int value in this.Values)
                {
                    output.WriteValueS32(value, littleEndian);
                }
            }
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                int count = input.ReadValueS32(littleEndian);
                this.Values = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    this.Values.Add(input.ReadValueS32(littleEndian));
                }
            }
            else
            {
                int count = input.ReadValueS32(littleEndian) / 4;
                this.Values = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    this.Values.Add(input.ReadValueS32(littleEndian));
                }
            }
        }

        public void Parse(string text)
        {
            string[] parts = text.Split(',');
            this.Values = new List<int>();
            foreach (var part in parts)
            {
                this.Values.Add(int.Parse(part, CultureInfo.InvariantCulture));
            }
        }

        public string Compose()
        {
            return this.Values.Implode(v => v.ToString(CultureInfo.InvariantCulture), ",");
        }
    }
}
