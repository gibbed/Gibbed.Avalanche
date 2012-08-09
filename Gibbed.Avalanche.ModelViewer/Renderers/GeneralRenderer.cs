using System.IO;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using Microsoft.Xna.Framework.Graphics;

namespace Gibbed.Avalanche.ModelViewer.Renderers
{
    internal class GeneralRenderer : GenericBlockRenderer<General>
    {
        #region Vertex Elements
        public static VertexElement[] SmallVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.NormalizedShort4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, 8, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(0, 20, VertexElementFormat.NormalizedShort4, VertexElementMethod.Default, VertexElementUsage.Position, 0),
        };

        public static VertexElement[] HackToFixDumbVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
        };

        public static VertexElement[] BigVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, 28, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
        };
        #endregion

        private Texture TextureDif;
        private Texture TextureNrm;
        private VertexDeclaration SmallVertexDeclaration;
        private VertexDeclaration BigVertexDeclaration;
        private VertexDeclaration HackToFixDumbVertexDeclaration;

        public override void Setup(GraphicsDevice device, General block, string basePath)
        {
            this.SmallVertexDeclaration = new VertexDeclaration(device, SmallVertexElements);
            this.BigVertexDeclaration = new VertexDeclaration(device, BigVertexElements);
            this.HackToFixDumbVertexDeclaration = new VertexDeclaration(device, HackToFixDumbVertexElements);
            
            string texturePath;

            texturePath = Path.Combine(basePath, block.Material.DiffuseTexture);
            if (File.Exists(texturePath) == false)
            {
                this.TextureDif = null;
            }
            else
            {
                this.TextureDif = Texture.FromFile(device, texturePath);
            }

            texturePath = Path.Combine(basePath, block.Material.NormalMap);
            if (File.Exists(texturePath) == false)
            {
                this.TextureNrm = null;
            }
            else
            {
                this.TextureNrm = Texture.FromFile(device, texturePath);
            }
        }

        public override void Render(GraphicsDevice device, General block)
        {
            VertexBuffer vertices;
            int vertexSize;

            if (block.HasBigVertices == false)
            {
                vertexSize = 28;
                device.VertexDeclaration = this.SmallVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.SmallVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.SmallVertices.ToArray());
                /*
                vertexSize = 20;
                device.VertexDeclaration = this.HackToFixDumbVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.HackToFixDumbVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.HackToFixDumbVertices.ToArray());
                */
            }
            else
            {
                vertexSize = 40;
                device.VertexDeclaration = this.BigVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.BigVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.BigVertices.ToArray());
            }

            device.Vertices[0].SetSource(vertices, 0, vertexSize);

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
