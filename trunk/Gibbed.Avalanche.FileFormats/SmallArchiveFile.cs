using System;
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

            public void Serialize(Stream output)
            {
                output.WriteValueS32(this.Name.Length);
                output.WriteStringASCII(this.Name);
                output.WriteValueU32(this.Offset);
                output.WriteValueU32(this.Size);
            }

            public void Deserialize(Stream input)
            {
                uint length = input.ReadValueU32();
                if (length > 256)
                {
                    throw new FormatException("doubt there is a file with more than 256 characters in its name");
                }

                this.Name = input.ReadStringASCII(length);
                this.Offset = input.ReadValueU32();
                this.Size = input.ReadValueU32();
            }
        }

        public List<Entry> Entries = new List<Entry>();

        public void Serialize(Stream output)
        {
            MemoryStream index = new MemoryStream();
            foreach (var entry in this.Entries)
            {
                entry.Serialize(index);
            }
            index.SetLength(index.Length.Align(16));
            index.Position = 0;

            output.WriteValueU32(4);
            output.WriteStringASCII("SARC");
            output.WriteValueU32(2);
            output.WriteValueU32((uint)index.Length);
            output.WriteFromStream(index, index.Length);
        }

        public void Deserialize(Stream input)
        {
            if (input.ReadValueU32() != 4)
            {
                throw new FormatException("bad header size");
            }

            if (input.ReadStringASCII(4) != "SARC")
            {
                throw new FormatException("bad header magic");
            }

            if (input.ReadValueU32() != 2)
            {
                throw new FormatException("bad header version");
            }

            MemoryStream index = input.ReadToMemoryStream(input.ReadValueU32());
            this.Entries = new List<Entry>();
            while (index.Length - index.Position > 15)
            {
                Entry entry = new Entry();
                entry.Deserialize(index);
                this.Entries.Add(entry);
            }
        }
    }
}
