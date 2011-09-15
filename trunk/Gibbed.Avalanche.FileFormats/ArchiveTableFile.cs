using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Avalanche.FileFormats
{
    public class ArchiveTableFile
    {
        public struct Entry
        {
            public uint Offset;
            public uint Size;
        }

        public uint Alignment;
        private List<uint> _Keys = new List<uint>();
        private Hashtable Entries = new Hashtable();

        public IEnumerable<uint> Keys
        {
            get { return this._Keys; }
        }

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public bool Contains(uint key)
        {
            return this.Entries.ContainsKey(key);
        }

        public Entry Get(uint key)
        {
            if (this.Entries.ContainsKey(key) == false)
            {
                throw new KeyNotFoundException();
            }

            return (Entry)this.Entries[key];
        }

        public void Set(uint key, Entry entry)
        {
            this.Entries[key] = entry;
        }

        public Entry this[uint key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                this.Set(key, value);
            }
        }

        public void Set(uint key, uint offset, uint size)
        {
            this.Set(key, new Entry()
            {
                Offset = offset,
                Size = size,
            });
        }

        public void Remove(uint key)
        {
            if (this.Entries.ContainsKey(key) == false)
            {
                throw new KeyNotFoundException();
            }

            this.Entries.Remove(key);
        }

        public void Move(uint oldKey, uint newKey)
        {
            if (this.Entries.ContainsKey(oldKey) == false)
            {
                throw new KeyNotFoundException();
            }
            else if (this.Entries.ContainsKey(newKey) == true)
            {
                throw new ArgumentException("table already contains the new key", "newKey");
            }

            this.Entries[newKey] = this.Entries[oldKey];
            this.Entries.Remove(oldKey);
        }

        public void Deserialize(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            
            this.Alignment = input.ReadValueU32();

            if (this.Alignment != 0x0800 && this.Alignment.Swap() != 0x0800)
            {
                throw new FormatException("strange alignment");
            }

            bool littleEndian;

            if (this.Alignment == 0x0800)
            {
                littleEndian = true;
            }
            else
            {
                this.Alignment = this.Alignment.Swap();
                littleEndian = false;
            }

            this.Entries.Clear();
            this._Keys.Clear();
            while (input.Position + 12 <= input.Length)
            {
                Entry entry = new Entry();
                uint name = input.ReadValueU32(littleEndian);
                entry.Offset = input.ReadValueU32(littleEndian);
                entry.Size = input.ReadValueU32(littleEndian);
                this._Keys.Add(name);
                this.Entries.Add(name, entry);
            }
        }
    }
}
