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

namespace Gibbed.Avalanche.RenderBlockModel.Blocks
{
    public class DeformableWindow : IRenderBlock
    {
        public struct Vertex
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float PositionW;
            public short TexCoord1A;
            public short TexCoord1B;
            public short TexCoord1C;
            public short TexCoord1D;
            public float TexCoord2A;
            public float TexCoord2B;
            public float TexCoord2C;
            public float TexCoord2D;
            public float U;
            public float V;

            public void Deserialize(Stream input)
            {
                this.PositionX = input.ReadValueF32();
                this.PositionY = input.ReadValueF32();
                this.PositionZ = input.ReadValueF32();
                this.PositionW = input.ReadValueF32();
                this.TexCoord1A = input.ReadValueS16();
                this.TexCoord1B = input.ReadValueS16();
                this.TexCoord1C = input.ReadValueS16();
                this.TexCoord1D = input.ReadValueS16();
                this.TexCoord2A = input.ReadValueF32();
                this.TexCoord2B = input.ReadValueF32();
                this.TexCoord2C = input.ReadValueF32();
                this.TexCoord2D = input.ReadValueF32();
                this.U = input.ReadValueF32();
                this.V = input.ReadValueF32();
            }

            public override string ToString()
            {
                return string.Format("{0},{1},{2}",
                                     this.PositionX,
                                     this.PositionY,
                                     this.PositionZ);
            }
        }

        public byte Version;
        public List<string> Textures = new List<string>();
        public List<Vertex> Vertices = new List<Vertex>();
        public List<short> Faces = new List<short>();
        public uint Unknown2;
        public uint Unknown3;

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream input)
        {
            this.Version = input.ReadValueU8();

            if (this.Version > 2)
            {
                throw new InvalidOperationException("unhandled version for DeformableWindow");
            }

            // TODO: vertex data (pbbbth)

            this.Textures.Clear();
            for (int i = 0; i < 8; i++)
            {
                this.Textures.Add(input.ReadStringASCIIUInt32());
            }
            this.Unknown2 = input.ReadValueU32();

            if (this.Version == 2)
            {
                this.Unknown3 = input.ReadValueU32();

                // mystic array of crap
                input.Seek(256 * 4, SeekOrigin.Current);
            }

            // vertices?
            this.Vertices.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    var data = new Vertex();
                    data.Deserialize(input);
                    this.Vertices.Add(data);
                }
            }

            this.Faces.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    this.Faces.Add(input.ReadValueS16());
                }
            }

            if (this.Version == 1)
            {
                // mystic array of crap
                input.Seek(256 * 4, SeekOrigin.Current);

                this.Unknown3 = input.ReadValueU32();
            }
        }
    }
}
