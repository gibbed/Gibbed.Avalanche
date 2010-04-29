using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.PropertyTypes
{
    public class FloatListProperty : IPropertyType
    {
        public List<float> Values;

        public byte Id { get { return 10; } }
        public string Tag { get { return "vec_float"; } }
        public bool Inline { get { return false; } }

        public void Default()
        {
            this.Values = new List<float>();
        }

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                output.WriteValueS32(this.Values.Count, littleEndian);
                foreach (float value in this.Values)
                {
                    output.WriteValueF32(value, littleEndian);
                }
            }
            else
            {
                output.WriteValueS32(this.Values.Count * 4, littleEndian);
                foreach (float value in this.Values)
                {
                    output.WriteValueF32(value, littleEndian);
                }
            }
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                int count = input.ReadValueS32(littleEndian);
                this.Values = new List<float>();
                for (int i = 0; i < count; i++)
                {
                    this.Values.Add(input.ReadValueF32(littleEndian));
                }
            }
            else
            {
                int count = input.ReadValueS32(littleEndian) / 4;
                this.Values = new List<float>();
                for (int i = 0; i < count; i++)
                {
                    this.Values.Add(input.ReadValueF32(littleEndian));
                }
            }
        }

        public void Parse(string text)
        {
            string[] parts = text.Split(',');
            this.Values = new List<float>();
            foreach (var part in parts)
            {
                this.Values.Add(float.Parse(part, CultureInfo.InvariantCulture));
            }
        }

        public string Compose()
        {
            return this.Values.Implode(v => v.ToString(CultureInfo.InvariantCulture), ",");
        }
    }
}
