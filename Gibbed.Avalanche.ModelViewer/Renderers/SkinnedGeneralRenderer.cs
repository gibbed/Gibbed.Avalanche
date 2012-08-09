using System.IO;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using Microsoft.Xna.Framework.Graphics;

namespace Gibbed.Avalanche.ModelViewer.Renderers
{
    internal class SkinnedGeneralRenderer : GenericBlockRenderer<SkinnedGeneral>
    {
        #region Vertex Elements
        public static VertexElement[] SmallVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            //new VertexElement(0, 12, VertexElementFormat.???, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            //new VertexElement(0, 16, VertexElementFormat.Byte4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            
            //new VertexElement(1, 0, VertexElementFormat.Rgba32, VertexElementMethod.Default, VertexElementUsage.Color, 0),
            //new VertexElement(1, 4, VertexElementFormat.Rgba32, VertexElementMethod.Default, VertexElementUsage.Color, 1),
            //new VertexElement(1, 8, VertexElementFormat.Rgba32, VertexElementMethod.Default, VertexElementUsage.Color, 2),
            new VertexElement(1, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
        };

        public static VertexElement[] BigVertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            //new VertexElement(0, 12, VertexElementFormat.???, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            //new VertexElement(0, 16, VertexElementFormat.Byte4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
            //new VertexElement(0, 20, VertexElementFormat.Byte4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            //new VertexElement(0, 24, VertexElementFormat.Byte4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
                    
            //new VertexElement(1, 0, VertexElementFormat.Rgba32, VertexElementMethod.Default, VertexElementUsage.Color, 0),
            //new VertexElement(1, 4, VertexElementFormat.Rgba32, VertexElementMethod.Default, VertexElementUsage.Color, 1),
            //new VertexElement(1, 8, VertexElementFormat.Rgba32, VertexElementMethod.Default, VertexElementUsage.Color, 2),
            new VertexElement(1, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
        };
        #endregion

        private Texture TextureDif;
        private Texture TextureNrm;
        private VertexDeclaration SmallVertexDeclaration;
        private VertexDeclaration BigVertexDeclaration;

        public override void Setup(GraphicsDevice device, SkinnedGeneral block, string basePath)
        {
            this.SmallVertexDeclaration = new VertexDeclaration(device, SmallVertexElements);
            this.BigVertexDeclaration = new VertexDeclaration(device, BigVertexElements);

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

        public override void Render(GraphicsDevice device, SkinnedGeneral block)
        {
            VertexBuffer vertices;
            int vertexSize;

            if (block.HasBigVertices == false)
            {
                vertexSize = 20;
                device.VertexDeclaration = this.SmallVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.SmallVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.SmallVertices.ToArray());
            }
            else
            {
                vertexSize = 28;
                device.VertexDeclaration = this.BigVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.BigVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.BigVertices.ToArray());
            }

            VertexBuffer extras = new VertexBuffer(device, block.Extras.Count * 20, BufferUsage.WriteOnly);
            extras.SetData(block.Extras.ToArray());

            device.Vertices[0].SetSource(vertices, 0, vertexSize);
            device.Vertices[1].SetSource(extras, 0, 20);

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
