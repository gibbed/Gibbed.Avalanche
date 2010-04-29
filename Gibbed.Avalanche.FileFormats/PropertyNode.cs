using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public class PropertyNode
    {
        public Dictionary<string, PropertyNode> ChildrenByName =
            new Dictionary<string, PropertyNode>();

        public Dictionary<string, IPropertyType> ValuesByName =
            new Dictionary<string, IPropertyType>();

        public Dictionary<uint, PropertyNode> ChildrenByHash =
            new Dictionary<uint, PropertyNode>();

        public Dictionary<uint, IPropertyType> ValuesByHash =
            new Dictionary<uint, IPropertyType>();

        public byte[] Unknown3;

        public void Serialize(Stream output, bool raw, bool littleEndian)
        {
            if (raw == true)
            {
                byte count = 0;

                if (this.ChildrenByName.Count > 0)
                {
                    count++;
                }

                if (this.ValuesByName.Count > 0)
                {
                    count++;
                }

                if (this.ChildrenByHash.Count > 0)
                {
                    count++;
                }

                if (this.ValuesByHash.Count > 0)
                {
                    count++;
                }

                if (this.Unknown3 != null && this.Unknown3.Length > 0)
                {
                    count++;
                }

                output.WriteValueU8(count);

                if (this.ChildrenByName.Count > 0)
                {
                    output.WriteValueU16(1, littleEndian);
                    output.WriteValueU16((ushort)this.ChildrenByName.Count, littleEndian);
                    foreach (var kvp in this.ChildrenByName.Take(this.ChildrenByName.Count & 0xFFFF))
                    {
                        output.WriteValueS32(kvp.Key.Length, littleEndian);
                        output.WriteStringASCII(kvp.Key);
                        kvp.Value.Serialize(output, raw, littleEndian);
                    }
                }

                if (this.ValuesByName.Count > 0)
                {
                    output.WriteValueU16(2, littleEndian);
                    output.WriteValueU16((ushort)this.ValuesByName.Count, littleEndian);
                    foreach (var kvp in this.ValuesByName.Take(this.ValuesByName.Count & 0xFFFF))
                    {
                        output.WriteValueS32(kvp.Key.Length, littleEndian);
                        output.WriteStringASCII(kvp.Key);
                        output.WriteValueU8(kvp.Value.Id);
                        kvp.Value.Serialize(output, raw, littleEndian);
                    }
                }

                if (this.Unknown3 != null && this.Unknown3.Length > 0)
                {
                    output.WriteValueU16(3, littleEndian);
                    output.WriteValueU16((ushort)(this.Unknown3.Length & 0xFFFF), littleEndian);
                    output.Write(this.Unknown3, 0, this.Unknown3.Length & 0xFFFF);
                }

                if (this.ChildrenByHash.Count > 0)
                {
                    output.WriteValueU16(4, littleEndian);
                    output.WriteValueU16((ushort)this.ChildrenByHash.Count, littleEndian);
                    foreach (var kvp in this.ChildrenByHash.Take(this.ChildrenByHash.Count & 0xFFFF))
                    {
                        output.WriteValueU32(kvp.Key, littleEndian);
                        kvp.Value.Serialize(output, raw, littleEndian);
                    }
                }

                if (this.ValuesByHash.Count > 0)
                {
                    output.WriteValueU16(5, littleEndian);
                    output.WriteValueU16((ushort)this.ValuesByHash.Count, littleEndian);
                    foreach (var kvp in this.ValuesByHash.Take(this.ValuesByHash.Count & 0xFFFF))
                    {
                        output.WriteValueU32(kvp.Key, littleEndian);
                        output.WriteValueU8(kvp.Value.Id);
                        kvp.Value.Serialize(output, raw, littleEndian);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Deserialize(Stream input, bool raw, bool littleEndian)
        {
            this.ChildrenByName.Clear();
            this.ValuesByName.Clear();
            this.ChildrenByHash.Clear();
            this.ValuesByHash.Clear();
            this.Unknown3 = null;

            if (raw == true)
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
                        for (ushort j = 0; j < subcount; j++)
                        {
                            uint length = input.ReadValueU32(littleEndian);
                            if (length >= 0x7FFF)
                            {
                                throw new Exception();
                            }
                            string id = input.ReadStringASCII(length, true);
                            PropertyNode child = new PropertyNode();
                            child.Deserialize(input, raw, littleEndian);
                            this.ChildrenByName.Add(id, child);
                        }
                    }
                    else if (type == 2)
                    {
                        for (ushort j = 0; j < subcount; j++)
                        {
                            uint length = input.ReadValueU32(littleEndian);
                            if (length >= 0x7FFF)
                            {
                                throw new Exception();
                            }
                            string id = input.ReadStringASCII(length, true);

                            byte propertyType = input.ReadValueU8();
                            IPropertyType value = PropertyHelpers.GetPropertyType(
                                propertyType, raw);
                            value.Deserialize(input, true, littleEndian);
                            this.ValuesByName.Add(id, value);
                        }
                    }
                    else if (type == 3)
                    {
                        this.Unknown3 = new byte[subcount];
                        input.Read(this.Unknown3, 0, subcount);
                    }
                    else if (type == 4)
                    {
                        for (ushort j = 0; j < subcount; j++)
                        {
                            uint id = input.ReadValueU32(littleEndian);
                            PropertyNode child = new PropertyNode();
                            child.Deserialize(input, raw, littleEndian);
                            this.ChildrenByHash.Add(id, child);
                        }
                    }
                    else if (type == 5)
                    {
                        for (ushort j = 0; j < subcount; j++)
                        {
                            uint id = input.ReadValueU32(littleEndian);

                            byte propertyType = input.ReadValueU8();
                            IPropertyType value = PropertyHelpers.GetPropertyType(
                                propertyType, raw);
                            value.Deserialize(input, true, littleEndian);
                            this.ValuesByHash.Add(id, value);
                        }
                    }
                    else
                    {
                        throw new FormatException("unknown object section type " + type.ToString());
                    }
                }
            }
            else
            {
                while (input.Position < input.Length)
                {
                    uint hash = input.ReadValueU32(littleEndian);
                    uint mode = input.ReadValueU32(littleEndian);
                    uint offset = input.ReadValueU32(littleEndian);
                    uint next = input.ReadValueU32(littleEndian);

                    if (mode == 1)
                    {
                        PropertyNode child = new PropertyNode();

                        if (offset != 0xFFFFFFFF)
                        {
                            input.Seek(offset, SeekOrigin.Begin);
                            offset = input.ReadValueU32(littleEndian);

                            if (offset != 0xFFFFFFFF)
                            {
                                input.Seek(offset, SeekOrigin.Begin);
                                child.Deserialize(input, raw, littleEndian);
                            }
                        }

                        this.ChildrenByHash.Add(hash, child);
                    }
                    else if (mode == 2)
                    {
                        IPropertyType value;

                        // null value?
                        if (offset == 0xFFFFFFFF)
                        {
                            value = null;
                        }
                        else
                        {
                            input.Seek(offset, SeekOrigin.Begin);
                            uint type = input.ReadValueU32(littleEndian);

                            value = PropertyHelpers.GetPropertyType(type, raw);

                            if (value.Inline == true)
                            {
                                value.Deserialize(input, false, littleEndian);
                            }
                            else
                            {
                                offset = input.ReadValueU32(littleEndian);
                                if (offset == 0xFFFFFFFF)
                                {
                                    value.Default();
                                }
                                else
                                {
                                    input.Seek(offset, SeekOrigin.Begin);
                                    value.Deserialize(input, false, littleEndian);
                                }
                            }
                        }

                        this.ValuesByHash.Add(hash, value);
                    }

                    if (next == 0xFFFFFFFF)
                    {
                        break;
                    }

                    input.Seek(next, SeekOrigin.Begin);
                }
            }
        }
    }
}
