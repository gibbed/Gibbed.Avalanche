using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Helpers;
using System.IO;

namespace Gibbed.Avalanche.FileFormats.DataFormat
{
    public class Entry
    {
        public uint NameHash;
        public uint TypeHash;
        public uint Offset;
        public uint Size;
        public string Name;

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.NameHash = input.ReadValueU32(littleEndian);
            this.TypeHash = input.ReadValueU32(littleEndian);
            this.Offset = input.ReadValueU32(littleEndian);
            this.Size = input.ReadValueU32(littleEndian);
            this.Name = input.ReadStringASCII(32, true);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? base.ToString() : this.Name;
        }
    }
}
