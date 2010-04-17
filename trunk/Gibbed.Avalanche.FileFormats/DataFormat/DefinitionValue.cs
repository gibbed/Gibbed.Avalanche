using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.DataFormat
{
    public class DefinitionValue
    {
        public string Name;
        public uint TypeHash;
        public uint Size;
        public uint Offset;
        public uint Unknown2C;
        public uint Unknown30;
        public uint Unknown34;

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.Name = input.ReadStringASCII(32, true);
            this.TypeHash = input.ReadValueU32(littleEndian);
            this.Size = input.ReadValueU32(littleEndian);
            this.Offset = input.ReadValueU32(littleEndian);
            this.Unknown2C = input.ReadValueU32(littleEndian);
            this.Unknown30 = input.ReadValueU32(littleEndian);
            this.Unknown34 = input.ReadValueU32(littleEndian);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? base.ToString() : this.Name;
        }
    }
}
