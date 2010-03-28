using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.Model
{
    public class SkinnedGeneral : IRenderBlock
    {
        public class VectorDataSmall
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;

            public byte TexCoord1A;
            public byte TexCoord1B;
            public byte TexCoord1C;
            public byte TexCoord1D;

            public byte TexCoord2A;
            public byte TexCoord2B;
            public byte TexCoord2C;
            public byte TexCoord2D;

            public void Deserialize(Stream input)
            {
                this.PositionX = input.ReadValueF32();
                this.PositionY = input.ReadValueF32();
                this.PositionZ = input.ReadValueF32();

                this.TexCoord1A = input.ReadValueU8();
                this.TexCoord1B = input.ReadValueU8();
                this.TexCoord1C = input.ReadValueU8();
                this.TexCoord1D = input.ReadValueU8();

                this.TexCoord2A = input.ReadValueU8();
                this.TexCoord2B = input.ReadValueU8();
                this.TexCoord2C = input.ReadValueU8();
                this.TexCoord2D = input.ReadValueU8();
            }

            public override string ToString()
            {
                return string.Format("{0},{1},{2}",
                    this.PositionX,
                    this.PositionY,
                    this.PositionZ);
            }
        }

        public class VectorDataBig
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;

            public byte TexCoord1A;
            public byte TexCoord1B;
            public byte TexCoord1C;
            public byte TexCoord1D;

            public byte TexCoord2A;
            public byte TexCoord2B;
            public byte TexCoord2C;
            public byte TexCoord2D;

            public byte TexCoord3A;
            public byte TexCoord3B;
            public byte TexCoord3C;
            public byte TexCoord3D;

            public byte TexCoord4A;
            public byte TexCoord4B;
            public byte TexCoord4C;
            public byte TexCoord4D;

            public void Deserialize(Stream input)
            {
                this.PositionX = input.ReadValueF32();
                this.PositionY = input.ReadValueF32();
                this.PositionZ = input.ReadValueF32();

                this.TexCoord1A = input.ReadValueU8();
                this.TexCoord1B = input.ReadValueU8();
                this.TexCoord1C = input.ReadValueU8();
                this.TexCoord1D = input.ReadValueU8();

                this.TexCoord2A = input.ReadValueU8();
                this.TexCoord2B = input.ReadValueU8();
                this.TexCoord2C = input.ReadValueU8();
                this.TexCoord2D = input.ReadValueU8();

                this.TexCoord3A = input.ReadValueU8();
                this.TexCoord3B = input.ReadValueU8();
                this.TexCoord3C = input.ReadValueU8();
                this.TexCoord3D = input.ReadValueU8();

                this.TexCoord4A = input.ReadValueU8();
                this.TexCoord4B = input.ReadValueU8();
                this.TexCoord4C = input.ReadValueU8();
                this.TexCoord4D = input.ReadValueU8();
            }
        }

        public class ExtraData
        {
            public byte Color1R;
            public byte Color1G;
            public byte Color1B;
            public byte Color1A;

            public byte Color2R;
            public byte Color2G;
            public byte Color2B;
            public byte Color2A;

            public byte Color3R;
            public byte Color3G;
            public byte Color3B;
            public byte Color3A;

            public float U;
            public float V;

            public void Deserialize(Stream input)
            {
                this.Color1R = input.ReadValueU8();
                this.Color1G = input.ReadValueU8();
                this.Color1B = input.ReadValueU8();
                this.Color1A = input.ReadValueU8();

                this.Color2R = input.ReadValueU8();
                this.Color2G = input.ReadValueU8();
                this.Color2B = input.ReadValueU8();
                this.Color2A = input.ReadValueU8();

                this.Color3R = input.ReadValueU8();
                this.Color3G = input.ReadValueU8();
                this.Color3B = input.ReadValueU8();
                this.Color3A = input.ReadValueU8();

                this.U = input.ReadValueF32();
                this.V = input.ReadValueF32();
            }
        }

        public class SkinBatch
        {
            public int Unknown0;
            public int Unknown1;
            public List<short> Unknown2 = new List<short>();

            public void Deserialize(Stream input)
            {
                this.Unknown0 = input.ReadValueS32();
                this.Unknown1 = input.ReadValueS32();
                this.Unknown2.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        this.Unknown2.Add(input.ReadValueS16());
                    }
                }
            }
        }

        public uint Flags;
        public float Unknown01;
        public float Unknown02;
        public float Unknown03;
        public float Unknown04;
        public float Unknown05;
        public float Unknown06;

        public List<string> Unknown07 = new List<string>();
        public uint Unknown08;
        public List<VectorDataSmall> Vertices_0 = new List<VectorDataSmall>();
        public List<VectorDataBig> Vertices_1 = new List<VectorDataBig>();
        public List<ExtraData> Unknown10 = new List<ExtraData>();
        public List<SkinBatch> Unknown11 = new List<SkinBatch>();
        public List<short> Unknown12 = new List<short>();

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream input)
        {
            if (input.ReadValueU8() != 3)
            {
                throw new FormatException();
            }

            this.Flags = input.ReadValueU32();
            this.Unknown01 = input.ReadValueF32();
            this.Unknown02 = input.ReadValueF32();
            this.Unknown03 = input.ReadValueF32();
            this.Unknown04 = input.ReadValueF32();
            this.Unknown05 = input.ReadValueF32();
            this.Unknown06 = input.ReadValueF32();

            this.Unknown07.Clear();
            for (int i = 0; i < 8; i++)
            {
                this.Unknown07.Add(input.ReadStringASCIIUInt32());
            }

            this.Unknown08 = input.ReadValueU32();

            if ((this.Flags & 0x80000) == 0x80000)
            {
                this.Vertices_1.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        var data = new VectorDataBig();
                        data.Deserialize(input);
                        this.Vertices_1.Add(data);
                    }
                }
            }
            else
            {
                this.Vertices_0.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        var data = new VectorDataSmall();
                        data.Deserialize(input);
                        this.Vertices_0.Add(data);
                    }
                }
            }

            this.Unknown10.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    var data = new ExtraData();
                    data.Deserialize(input);
                    this.Unknown10.Add(data);
                }
            }

            this.Unknown11.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    var batch = new SkinBatch();
                    batch.Deserialize(input);
                    this.Unknown11.Add(batch);
                }
            }

            this.Unknown12.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    this.Unknown12.Add(input.ReadValueS16());
                }
            }
        }
    }
}
