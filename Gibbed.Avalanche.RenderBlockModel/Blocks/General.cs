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
    public class General : IRenderBlock
    {
        public struct SmallVertex
        {
            public short TexCoord1A;
            public short TexCoord1B;
            public short TexCoord1C;
            public short TexCoord1D;
            public float TexCoord2A;
            public float TexCoord2B;
            public float TexCoord2C;
            public short PositionX;
            public short PositionY;
            public short PositionZ;
            public short PositionW;

            public void Deserialize(Stream input)
            {
                this.TexCoord1A = input.ReadValueS16();
                this.TexCoord1B = input.ReadValueS16();
                this.TexCoord1C = input.ReadValueS16();
                this.TexCoord1D = input.ReadValueS16();
                this.TexCoord2A = input.ReadValueF32();
                this.TexCoord2B = input.ReadValueF32();
                this.TexCoord2C = input.ReadValueF32();
                this.PositionX = input.ReadValueS16();
                this.PositionY = input.ReadValueS16();
                this.PositionZ = input.ReadValueS16();
                this.PositionW = input.ReadValueS16();
            }

            public override string ToString()
            {
                return string.Format("{0},{1},{2}",
                                     this.PositionX,
                                     this.PositionY,
                                     this.PositionZ);
            }
        }

        public struct BigVertex
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float TexCoord1A;
            public float TexCoord1B;
            public float TexCoord1C;
            public float TexCoord1D;
            public float TexCoord2A;
            public float TexCoord2B;
            public float TexCoord2C;

            public void Deserialize(Stream input)
            {
                this.PositionX = input.ReadValueF32();
                this.PositionY = input.ReadValueF32();
                this.PositionZ = input.ReadValueF32();
                this.TexCoord1A = input.ReadValueF32();
                this.TexCoord1B = input.ReadValueF32();
                this.TexCoord1C = input.ReadValueF32();
                this.TexCoord1D = input.ReadValueF32();
                this.TexCoord2A = input.ReadValueF32();
                this.TexCoord2B = input.ReadValueF32();
                this.TexCoord2C = input.ReadValueF32();
            }
        }

        public struct HackToFixDumbVertex
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float TexCoordA;
            public float TexCoordB;
        }

        public byte Version;
        public uint Flags;

        public bool HasBigVertices
        {
            get { return this.Unknown11 == 0; }
        }

        public float Unknown01;
        public float Unknown02;
        public float Unknown03;
        public float Unknown04;
        public float Unknown05;
        public float Unknown06;
        public float Unknown07;
        public float Unknown08;
        public float Unknown09;
        public float Unknown10;

        public uint Unknown11;

        public float Unknown12;
        public float Unknown13;
        public float Unknown14;
        public float Unknown15;
        public float Unknown16;
        public float Unknown17;
        public int Unknown18;
        public int Unknown19;

        public List<string> Textures = new List<string>();
        public uint Unknown20;

        public List<SmallVertex> SmallVertices = new List<SmallVertex>();
        public List<HackToFixDumbVertex> HackToFixDumbVertices = new List<HackToFixDumbVertex>();
        public List<BigVertex> BigVertices = new List<BigVertex>();
        public List<short> Faces = new List<short>();

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        private static float GetFloatFromS16N(short c)
        {
            if (c == -1)
            {
                return -1.0f;
            }

            return c * (1.0f / 32767);
        }

        public void Deserialize(Stream input)
        {
            this.Version = input.ReadValueU8();
            if (this.Version != 2 && this.Version != 3)
            {
                throw new FormatException("unhandled version for General");
            }

            this.Unknown01 = input.ReadValueF32();
            this.Unknown02 = input.ReadValueF32();
            this.Unknown03 = input.ReadValueF32();
            this.Unknown04 = input.ReadValueF32();
            this.Unknown05 = input.ReadValueF32();
            this.Unknown06 = input.ReadValueF32();
            this.Unknown07 = input.ReadValueF32();
            this.Unknown08 = input.ReadValueF32();
            this.Unknown09 = input.ReadValueF32();
            this.Unknown10 = input.ReadValueF32();

            this.Unknown11 = input.ReadValueU32();

            this.Unknown12 = input.ReadValueF32();
            this.Unknown13 = input.ReadValueF32();
            this.Unknown14 = input.ReadValueF32();
            this.Unknown15 = input.ReadValueF32();
            this.Unknown16 = input.ReadValueF32();
            this.Unknown17 = input.ReadValueF32();

            if (this.Version == 3)
            {
                this.Unknown18 = input.ReadValueS32();
                this.Unknown19 = input.ReadValueS32();
            }

            this.Textures.Clear();
            for (int i = 0; i < 8; i++)
            {
                this.Textures.Add(input.ReadStringASCIIUInt32());
            }
            this.Unknown20 = input.ReadValueU32();

            if (this.HasBigVertices == false)
            {
                this.SmallVertices.Clear();
                this.HackToFixDumbVertices.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        var data = new SmallVertex();
                        data.Deserialize(input);
                        this.SmallVertices.Add(data);

                        var hack = new HackToFixDumbVertex
                        {
                            PositionX = GetFloatFromS16N(data.PositionX) * this.Unknown10,
                            PositionY = GetFloatFromS16N(data.PositionY) * this.Unknown10,
                            PositionZ = GetFloatFromS16N(data.PositionZ) * this.Unknown10,
                            TexCoordA = GetFloatFromS16N(data.TexCoord1A),
                            TexCoordB = GetFloatFromS16N(data.TexCoord1B)
                        };
                        this.HackToFixDumbVertices.Add(hack);
                    }
                }
            }
            else
            {
                this.BigVertices.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        var data = new BigVertex();
                        data.Deserialize(input);
                        this.BigVertices.Add(data);
                    }
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
        }
    }
}
