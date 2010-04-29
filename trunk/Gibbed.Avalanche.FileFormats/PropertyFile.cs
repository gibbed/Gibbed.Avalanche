using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public class PropertyFile
    {
        public bool Raw = true;
        public List<PropertyNode> Nodes;

        public PropertyFile()
        {
            this.Nodes = new List<PropertyNode>();
        }

        public void Serialize(Stream output, bool littleEndian)
        {
            if (this.Raw == true)
            {
                foreach (var node in this.Nodes)
                {
                    node.Serialize(output, this.Raw, littleEndian);
                }
            }
            else
            {
                foreach (var node in this.Nodes)
                {
                    MemoryStream memory = new MemoryStream();
                    node.Serialize(memory, this.Raw, littleEndian);
                    memory.Position = 0;

                    output.WriteStringASCII("PCBB");
                    output.WriteValueU32((uint)memory.Length, littleEndian);
                    output.WriteFromStream(memory, memory.Length);
                }
            }
        }

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.Nodes = new List<PropertyNode>();

            input.Seek(0, SeekOrigin.Begin);
            if (input.ReadStringASCII(4) == "PCBB")
            {
                this.Raw = false;
                input.Seek(0, SeekOrigin.Begin);

                while (input.Position < input.Length)
                {
                    if (input.ReadStringASCII(4) != "PCBB")
                    {
                        //throw new FormatException("invalid magic tag");
                        break;
                    }

                    uint length = input.ReadValueU32(littleEndian);
                    if (input.Position + length > input.Length)
                    {
                        throw new InvalidOperationException("object size greater than file size");
                    }

                    MemoryStream memory = input.ReadToMemoryStream(length);
                    var node = new PropertyNode();
                    node.Deserialize(memory, false, littleEndian);
                    this.Nodes.Add(node);
                }
            }
            else
            {
                this.Raw = true;
                input.Seek(0, SeekOrigin.Begin);

                while (input.Position < input.Length)
                {
                    var node = new PropertyNode();
                    node.Deserialize(input, true, littleEndian);
                    this.Nodes.Add(node);
                }
            }
        }

        public PropertyNode GetAnyNodeById(uint id)
        {
            foreach (var node in this.Nodes)
            {
                if (node.ChildrenByHash == null ||
                    node.ChildrenByHash.Count == 0)
                {
                    continue;
                }

                var child = node.ChildrenByHash.SingleOrDefault(
                    n => n.Key == id);
                if (child.Value != null)
                {
                    return child.Value;
                }
            }
            return null;
        }

        public PropertyNode GetAnyNodeById(string id)
        {
            return this.GetAnyNodeById(id.HashJenkins());
        }
    }
}
