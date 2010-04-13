using System.IO;
using Gibbed.Avalanche.FileFormats.RenderBlock;
using Microsoft.Xna.Framework.Graphics;


namespace Gibbed.Avalanche.ModelViewer.Renderers
{
    internal class CarPaintRenderer : GenericBlockRenderer<CarPaint>
    {
        #region Vertex Elements
        public static VertexElement[] VertexElements =
        {
            // first one "should" be Vertex4
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 16, VertexElementFormat.NormalizedShort4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(1, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(1, 12, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2),
        };
        #endregion

        private Texture TextureDif;
        private Texture TextureNrm;
        private VertexDeclaration VertexDeclaration;

        public override void Setup(GraphicsDevice device, CarPaint block, string basePath)
        {
            this.VertexDeclaration = new VertexDeclaration(device, VertexElements);

            string texturePath;

            texturePath = Path.Combine(basePath, block.Textures[0]);
            if (File.Exists(texturePath) == false)
            {
                this.TextureDif = null;
            }
            else
            {
                this.TextureDif = Texture.FromFile(device, texturePath);
            }

            texturePath = Path.Combine(basePath, block.Textures[1]);
            if (File.Exists(texturePath) == false)
            {
                this.TextureNrm = null;
            }
            else
            {
                this.TextureNrm = Texture.FromFile(device, texturePath);
            }
        }

        public override void Render(GraphicsDevice device, CarPaint block)
        {
            VertexBuffer vertices;
            int vertexSize;

                device.VertexDeclaration = this.VertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.Vertices.Count * 24,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.Vertices.ToArray());

            VertexBuffer extras = new VertexBuffer(
                device,
                block.Unknown18.Count * 28,
                BufferUsage.WriteOnly);
            extras.SetData(block.Unknown18.ToArray());

            device.Vertices[0].SetSource(vertices, 0, 24);
            device.Vertices[1].SetSource(extras, 0, 28);

            var indices = new IndexBuffer(
                    device,
                    typeof(short),
                    block.Faces.Count,
                    BufferUsage.WriteOnly);
            indices.SetData(block.Faces.ToArray(), 0, block.Faces.Count);
            device.Indices = indices;

            device.Textures[0] = this.TextureDif;
            device.Textures[1] = this.TextureNrm; // not "working" yet (needs shader~)

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, block.Faces.Count, 0, block.Faces.Count / 3);
        }
    }
}
