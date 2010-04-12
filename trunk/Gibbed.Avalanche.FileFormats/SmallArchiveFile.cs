using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public class SmallArchiveFile
    {
        public class Entry
        {
            public string Name;
            public uint Offset;
            public uint Size;

            public void Serialize(Stream output, bool littleEndian)
            {
                output.WriteValueS32(Encoding.ASCII.GetByteCount(this.Name), littleEndian);
                output.WriteStringASCII(this.Name);
                output.WriteValueU32(this.Offset, littleEndian);
                output.WriteValueU32(this.Size, littleEndian);
            }

            public void Deserialize(Stream input, bool littleEndian)
            {
                uint length = input.ReadValueU32(littleEndian);
                if (length > 1024)
                {
                    throw new FormatException("doubt there is a file with more than 256 characters in its name");
                }

                this.Name = input.ReadStringASCII(length);
                this.Offset = input.ReadValueU32(littleEndian);
                this.Size = input.ReadValueU32(littleEndian);
            }
        }

        public bool LittleEndian;
        public List<Entry> Entries = new List<Entry>();

        public void Serialize(Stream output)
        {
            MemoryStream index = new MemoryStream();
            foreach (var entry in this.Entries)
            {
                entry.Serialize(index, this.LittleEndian);
            }

            index.SetLength(index.Length.Align(16));
            index.Position = 0;

            output.WriteValueU32(4, this.LittleEndian);
            output.WriteStringASCII("SARC");
            output.WriteValueU32(2, this.LittleEndian);
            output.WriteValueU32((uint)index.Length, this.LittleEndian);

            output.WriteFromStream(index, index.Length);
        }

        public void Deserialize(Stream input)
        {
            uint magicSize = input.ReadValueU32();

            if (magicSize != 4 && magicSize.Swap() != 4)
            {
                throw new FormatException("bad header size");
            }

            this.LittleEndian = magicSize == 4;

            if (input.ReadStringASCII(4) != "SARC")
            {
                throw new FormatException("bad header magic");
            }

            if (input.ReadValueU32(this.LittleEndian) != 2)
            {
                throw new FormatException("bad header version");
            }

            MemoryStream index = input.ReadToMemoryStream(input.ReadValueU32(this.LittleEndian));
            this.Entries = new List<Entry>();
            while (index.Length - index.Position > 15)
            {
                Entry entry = new Entry();
                entry.Deserialize(index, this.LittleEndian);
                this.Entries.Add(entry);
            }
        }
    }
}
