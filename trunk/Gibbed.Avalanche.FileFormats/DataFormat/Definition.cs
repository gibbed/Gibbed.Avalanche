using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.DataFormat
{
    public class Definition
    {
        public DefinitionType DefinitionType;
        public uint Size;
        public uint Unknown08;
        public uint TypeHash;
        public string Name;
        public uint Unknown50;
        public uint ElementTypeHash;
        public uint Unknown58;
        public uint Unknown5C;
        public List<DefinitionValue> ValueDefinitions = new List<DefinitionValue>();

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.DefinitionType = input.ReadValueEnum<DefinitionType>(littleEndian);
            this.Size = input.ReadValueU32(littleEndian);
            this.Unknown08 = input.ReadValueU32(littleEndian);
            this.TypeHash = input.ReadValueU32(littleEndian);
            this.Name = input.ReadStringASCII(64, true);
            this.Unknown50 = input.ReadValueU32(littleEndian);
            this.ElementTypeHash = input.ReadValueU32(littleEndian);
            this.Unknown58 = input.ReadValueU32(littleEndian);

            uint count = input.ReadValueU32(littleEndian);
            this.ValueDefinitions.Clear();
            for (uint i = 0; i < count; i++)
            {
                DefinitionValue definition = new DefinitionValue();
                definition.Deserialize(input, littleEndian);
                this.ValueDefinitions.Add(definition);
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? base.ToString() : this.Name;
        }
    }
}
