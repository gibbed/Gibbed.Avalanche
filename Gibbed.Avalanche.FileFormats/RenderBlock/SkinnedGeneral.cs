using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Avalanche.FileFormats.RenderBlock
{
    public class SkinnedGeneral : IRenderBlock
    {
        public struct SmallVertex
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

            public void Serialize(Stream output)
            {
                output.WriteValueF32(this.PositionX);
                output.WriteValueF32(this.PositionY);
                output.WriteValueF32(this.PositionZ);

                output.WriteValueU8(this.TexCoord1A);
                output.WriteValueU8(this.TexCoord1B);
                output.WriteValueU8(this.TexCoord1C);
                output.WriteValueU8(this.TexCoord1D);

                output.WriteValueU8(this.TexCoord2A);
                output.WriteValueU8(this.TexCoord2B);
                output.WriteValueU8(this.TexCoord2C);
                output.WriteValueU8(this.TexCoord2D);
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

            public void Serialize(Stream output)
            {
                output.WriteValueF32(this.PositionX);
                output.WriteValueF32(this.PositionY);
                output.WriteValueF32(this.PositionZ);

                output.WriteValueU8(this.TexCoord1A);
                output.WriteValueU8(this.TexCoord1B);
                output.WriteValueU8(this.TexCoord1C);
                output.WriteValueU8(this.TexCoord1D);

                output.WriteValueU8(this.TexCoord2A);
                output.WriteValueU8(this.TexCoord2B);
                output.WriteValueU8(this.TexCoord2C);
                output.WriteValueU8(this.TexCoord2D);

                output.WriteValueU8(this.TexCoord3A);
                output.WriteValueU8(this.TexCoord3B);
                output.WriteValueU8(this.TexCoord3C);
                output.WriteValueU8(this.TexCoord3D);

                output.WriteValueU8(this.TexCoord4A);
                output.WriteValueU8(this.TexCoord4B);
                output.WriteValueU8(this.TexCoord4C);
                output.WriteValueU8(this.TexCoord4D);
            }
        }

        public struct ExtraData
        {
            public uint Color1;
            public uint Color2;
            public uint Color3;
            public float U;
            public float V;

            public void Deserialize(Stream input)
            {
                this.Color1 = input.ReadValueU32();
                this.Color2 = input.ReadValueU32();
                this.Color3 = input.ReadValueU32();
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

        public byte Version;
        public uint Flags;
        public bool HasBigVertices
        {
            get { return this.Version >= 3 && (this.Flags & 0x80000) == 0x80000; }
        }

        public float Unknown01;
        public float Unknown02;
        public float Unknown03;
        public float Unknown04;
        public float Unknown05;
        public float Unknown06;

        public List<string> Textures = new List<string>();
        public uint Unknown08;
        public List<SmallVertex> SmallVertices = new List<SmallVertex>();
        public List<BigVertex> BigVertices = new List<BigVertex>();
        public List<ExtraData> Extras = new List<ExtraData>();
        public List<SkinBatch> SkinBatches = new List<SkinBatch>();
        public List<short> Faces = new List<short>();

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream input)
        {
            this.Version = input.ReadValueU8();
            if (this.Version != 0 && this.Version != 3)
            {
                throw new FormatException("unhandled version for SkinnedGeneral");
            }

            this.Flags = input.ReadValueU32();
            this.Unknown01 = input.ReadValueF32();
            this.Unknown02 = input.ReadValueF32();
            this.Unknown03 = input.ReadValueF32();
            this.Unknown04 = input.ReadValueF32();
            this.Unknown05 = input.ReadValueF32();
            this.Unknown06 = input.ReadValueF32();

            this.Textures.Clear();
            for (int i = 0; i < 8; i++)
            {
                this.Textures.Add(input.ReadStringASCIIUInt32());
            }
            this.Unknown08 = input.ReadValueU32();

            if (this.HasBigVertices == false)
            {
                this.SmallVertices.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        var data = new SmallVertex();
                        data.Deserialize(input);
                        this.SmallVertices.Add(data);
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

            this.Extras.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    var data = new ExtraData();
                    data.Deserialize(input);
                    this.Extras.Add(data);
                }
            }

            this.SkinBatches.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    var batch = new SkinBatch();
                    batch.Deserialize(input);
                    this.SkinBatches.Add(batch);
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
