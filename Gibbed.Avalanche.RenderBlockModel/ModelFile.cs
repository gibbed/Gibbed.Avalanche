/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Avalanche.FileFormats;
using Gibbed.IO;

namespace Gibbed.Avalanche.RenderBlockModel
{
    public class ModelFile
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

        public List<IRenderBlock> Blocks =
            new List<IRenderBlock>();

        public Dictionary<IRenderBlock, DebugInfo> DebugInfos =
            new Dictionary<IRenderBlock, DebugInfo>();

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
                var debugInfo = new DebugInfo
                {
                    Offset = input.Position,
                };

                uint type = input.ReadValueU32();

                var block = BlockTypeFactory.GetBlock(type);
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
