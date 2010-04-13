using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.RenderBlock
{
    public class CarPaint : IRenderBlock
    {
        public struct Vertex
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float PositionW;

            public short TexCoordA;
            public short TexCoordB;
            public short TexCoordC;
            public short TexCoordD;

            public void Deserialize(Stream input)
            {
                this.PositionX = input.ReadValueF32();
                this.PositionY = input.ReadValueF32();
                this.PositionZ = input.ReadValueF32();
                this.PositionW = input.ReadValueF32();

                this.TexCoordA = input.ReadValueS16();
                this.TexCoordB = input.ReadValueS16();
                this.TexCoordC = input.ReadValueS16();
                this.TexCoordD = input.ReadValueS16();
            }

            public void Serialize(Stream output)
            {
                output.WriteValueF32(this.PositionX);
                output.WriteValueF32(this.PositionY);
                output.WriteValueF32(this.PositionZ);
                output.WriteValueF32(this.PositionW);

                output.WriteValueS16(this.TexCoordA);
                output.WriteValueS16(this.TexCoordB);
                output.WriteValueS16(this.TexCoordC);
                output.WriteValueS16(this.TexCoordD);
            }

            public override string ToString()
            {
                return string.Format("{0},{1},{2}",
                    this.PositionX,
                    this.PositionY,
                    this.PositionZ);
            }
        }

        public struct ExtraData
        {
            public float TexCoord1A;
            public float TexCoord1B;
            public float TexCoord1C;
            public float TexCoord2A;
            public float TexCoord2B;
            public float TexCoord2C;
            public float TexCoord2D;

            public void Deserialize(Stream input)
            {
                this.TexCoord1A = input.ReadValueF32();
                this.TexCoord1B = input.ReadValueF32();
                this.TexCoord1C = input.ReadValueF32();
                this.TexCoord2A = input.ReadValueF32();
                this.TexCoord2B = input.ReadValueF32();
                this.TexCoord2C = input.ReadValueF32();
                this.TexCoord2D = input.ReadValueF32();
            }
        }

        public byte Version;
        public float Unknown01;
        public float Unknown02;
        public float Unknown03;
        public float Unknown04;
        public float Unknown05;
        public float Unknown06;
        public float Unknown07;
        public float Unknown08;
        public float Unknown09;
        public uint Unknown10;
        public uint Unknown11;
        public uint Unknown12;
        public uint Unknown13;
        public uint Unknown14;
        public List<string> Textures = new List<string>();
        public uint Unknown16;
        public List<Vertex> Vertices = new List<Vertex>();
        public List<ExtraData> Unknown18 = new List<ExtraData>();
        public List<short> Faces = new List<short>();

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream input)
        {
            this.Version = input.ReadValueU8();
            if (this.Version < 1 || this.Version > 4)
            {
                throw new FormatException("unhandled version for CarPaint");
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
            this.Unknown10 = input.ReadValueU32();
            this.Unknown11 = input.ReadValueU32();
            this.Unknown12 = input.ReadValueU32();
            this.Unknown13 = input.ReadValueU32();
            this.Unknown14 = input.ReadValueU32();

            this.Textures.Clear();
            for (int i = 0; i < 8; i++)
            {
                this.Textures.Add(input.ReadStringASCIIUInt32());
            }
            this.Unknown16 = input.ReadValueU32();

            if (this.Version == 1 || this.Version == 2)
            {
                // 52 * count
                // faces
                throw new NotImplementedException();
            }
            else if (this.Version == 3 || this.Version == 4)
            {
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

                this.Unknown18.Clear();
                {
                    uint count = input.ReadValueU32();
                    for (uint i = 0; i < count; i++)
                    {
                        var data = new ExtraData();
                        data.Deserialize(input);
                        this.Unknown18.Add(data);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            this.Faces.Clear();
            {
                uint count = input.ReadValueU32();
                for (uint i = 0; i < count; i++)
                {
                    this.Faces.Add(input.ReadValueS16());
                }
            }

            // mystic array of crap
            input.Seek(256 * 4, SeekOrigin.Current);
        }
    }
}
