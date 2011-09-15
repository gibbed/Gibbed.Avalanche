using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Avalanche.FileFormats
{
    public class RbModel
    {
        public float MinX;
        public float MinY;
        public float MinZ;
        public float MaxX;
        public float MaxY;
        public float MaxZ;

        public class DebugInfo
        {
            public long Offset;
            public long Size;
        }

        public List<RenderBlock.IRenderBlock> Blocks =
            new List<RenderBlock.IRenderBlock>();

        public Dictionary<RenderBlock.IRenderBlock, DebugInfo> DebugInfos =
            new Dictionary<RenderBlock.IRenderBlock, DebugInfo>();

        public void Deserialize(Stream input)
        {
            if (input.ReadStringASCIIUInt32() != "RBMDL")
            {
                throw new FormatException();
            }

            uint unk0 = input.ReadValueU32();
            uint unk1 = input.ReadValueU32();
            uint unk2 = input.ReadValueU32();

            if (unk0 != 1 || unk1 != 13)
            {
                throw new FormatException();
            }

            this.MinX = input.ReadValueF32();
            this.MinY = input.ReadValueF32();
            this.MinZ = input.ReadValueF32();
            this.MaxX = input.ReadValueF32();
            this.MaxY = input.ReadValueF32();
            this.MaxZ = input.ReadValueF32();

            this.Blocks.Clear();
            this.DebugInfos.Clear();
            uint count = input.ReadValueU32();
            for (uint i = 0; i < count; i++)
            {
                DebugInfo debugInfo = new DebugInfo();
                debugInfo.Offset = input.Position;

                uint type = input.ReadValueU32();

                var block = RenderBlock.BlockTypes.GetBlock(type);
                if (block == null)
                {
                    throw new Exception("unknown block type " + type.ToString("X8"));
                }

                block.Deserialize(input);

                if (input.ReadValueU32() != 0x89ABCDEF)
                {
                    throw new Exception("failed to read block properly");
                }

                this.Blocks.Add(block);

                debugInfo.Size = input.Position - debugInfo.Offset;
                this.DebugInfos.Add(block, debugInfo);
            }
        }

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }
    }
}
