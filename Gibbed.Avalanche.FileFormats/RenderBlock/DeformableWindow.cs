using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats.RenderBlock
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
