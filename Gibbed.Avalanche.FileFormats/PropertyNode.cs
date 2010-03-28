using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public class PropertyNode
    {
        public Dictionary<string, PropertyNode> ChildrenByName;
        public Dictionary<string, object> ValuesByName;
        public Dictionary<uint, PropertyNode> ChildrenByHash;
        public Dictionary<uint, object> ValuesByHash;
        public byte[] Unknown3;

        public uint GetUInt32(uint id)
        {
            if (!(this.ValuesByHash[id] is int))
            {
                throw new InvalidOperationException();
            }

            return (uint)((int)this.ValuesByHash[id]);
        }

        public uint GetUInt32(string id)
        {
            return this.GetUInt32(id.HashJenkins());
        }

        public string GetString(uint id)
        {
            if (!(this.ValuesByHash[id] is string))
            {
                throw new InvalidOperationException();
            }

            return (string)this.ValuesByHash[id];
        }

        public string GetString(string id)
        {
            return this.GetString(id.HashJenkins());
        }

        public Matrix GetMatrix(uint id)
        {
            if (!(this.ValuesByHash[id] is Matrix))
            {
                throw new InvalidOperationException();
            }

            return (Matrix)this.ValuesByHash[id];
        }

        public Matrix GetMatrix(string id)
        {
            return this.GetMatrix(id.HashJenkins());
        }

        public void Serialize(Stream output, bool littleEndian)
        {
            byte count = 0;

            if (this.ChildrenByHash != null && this.ChildrenByHash.Count > 0)
            {
                count++;
            }

            if (this.ValuesByHash != null && this.ValuesByHash.Count > 0)
            {
                count++;
            }

            if (this.Unknown3 != null && this.Unknown3.Length > 0)
            {
                count++;
            }

            output.WriteValueU8(count);

            if (this.Unknown3 != null && this.Unknown3.Length > 0)
            {
                output.WriteValueU16(3, littleEndian);
                output.WriteValueU16((ushort)(this.Unknown3.Length & 0xFFFF), littleEndian);
                output.Write(this.Unknown3, 0, this.Unknown3.Length & 0xFFFF);
            }

            if (this.ChildrenByHash != null && this.ChildrenByHash.Count > 0)
            {
                output.WriteValueU16(4, littleEndian);
                output.WriteValueU16((ushort)this.ChildrenByHash.Count, littleEndian);
                foreach (var kvp in this.ChildrenByHash.Take(this.ChildrenByHash.Count & 0xFFFF))
                {
                    output.WriteValueU32(kvp.Key, littleEndian);
                    kvp.Value.Serialize(output, littleEndian);
                }
            }

            if (this.ValuesByHash != null && this.ValuesByHash.Count > 0)
            {
                output.WriteValueU16(5, littleEndian);
                output.WriteValueU16((ushort)this.ValuesByHash.Count, littleEndian);
                foreach (var kvp in this.ValuesByHash.Take(this.ValuesByHash.Count & 0xFFFF))
                {
                    output.WriteValueU32(kvp.Key, littleEndian);
                    PropertyHelpers.WriteTypedValue(output, kvp.Value, littleEndian);
                }
            }
        }

        public void Deserialize(Stream input, bool littleEndian)
        {
            List<ushort> used = new List<ushort>();
            byte count = input.ReadValueU8();

            for (byte i = 0; i < count; i++)
            {
                ushort type = input.ReadValueU16(littleEndian);
                ushort subcount = input.ReadValueU16(littleEndian);

                if (used.Contains(type) == true)
                {
                    throw new Exception();
                }

                used.Add(type);

                if (type == 1)
                {
                    this.ChildrenByName = new Dictionary<string, PropertyNode>();
                    for (ushort j = 0; j < subcount; j++)
                    {
                        uint length = input.ReadValueU32(littleEndian);
                        if (length >= 0x7FFF)
                        {
                            throw new Exception();
                        }
                        string id = input.ReadStringASCII(length, true);
                        PropertyNode child = new PropertyNode();
                        child.Deserialize(input, littleEndian);
                        this.ChildrenByName.Add(id, child);
                    }
                }
                else if (type == 2)
                {
                    this.ValuesByName = new Dictionary<string, object>();
                    for (ushort j = 0; j < subcount; j++)
                    {
                        uint length = input.ReadValueU32(littleEndian);
                        if (length >= 0x7FFF)
                        {
                            throw new Exception();
                        }
                        string id = input.ReadStringASCII(length, true);
                        this.ValuesByName.Add(id, PropertyHelpers.ReadTypedValue(input, littleEndian));
                    }
                }
                else if (type == 3)
                {
                    this.Unknown3 = new byte[subcount];
                    input.Read(this.Unknown3, 0, subcount);
                }
                else if (type == 4)
                {
                    this.ChildrenByHash = new Dictionary<uint, PropertyNode>();
                    for (ushort j = 0; j < subcount; j++)
                    {
                        uint id = input.ReadValueU32(littleEndian);
                        PropertyNode child = new PropertyNode();
                        child.Deserialize(input, littleEndian);
                        this.ChildrenByHash.Add(id, child);
                    }
                }
                else if (type == 5)
                {
                    this.ValuesByHash = new Dictionary<uint, object>();
                    for (ushort j = 0; j < subcount; j++)
                    {
                        uint id = input.ReadValueU32(littleEndian);
                        this.ValuesByHash.Add(id, PropertyHelpers.ReadTypedValue(input, littleEndian));
                    }
                }
                else
                {
                    throw new FormatException("unknown object section type " + type.ToString());
                }
            }
        }
    }
}
