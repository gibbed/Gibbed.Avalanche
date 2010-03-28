using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Gibbed.Avalanche.FileFormats
{
    public class PropertyFile
    {
        public List<PropertyNode> Nodes;

        public PropertyFile()
        {
            this.Nodes = new List<PropertyNode>();
        }

        public void Serialize(Stream output, bool littleEndian)
        {
            foreach (var node in this.Nodes)
            {
                node.Serialize(output, littleEndian);
            }
        }

        public void Deserialize(Stream input, bool littleEndian)
        {
            input.Seek(0, SeekOrigin.Begin);
            this.Nodes = new List<PropertyNode>();
            while (input.Position < input.Length)
            {
                var node = new PropertyNode();
                node.Deserialize(input, littleEndian);
                this.Nodes.Add(node);
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
