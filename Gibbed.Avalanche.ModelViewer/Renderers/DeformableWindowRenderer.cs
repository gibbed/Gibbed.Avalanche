﻿using System.IO;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using Microsoft.Xna.Framework.Graphics;


namespace Gibbed.Avalanche.ModelViewer.Renderers
{
    internal class DeformableWindowRenderer : GenericBlockRenderer<DeformableWindow>
    {
        #region Vertex Elements
        public static VertexElement[] VertexElements =
        {
            // first one "should" be Vertex4
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 16, VertexElementFormat.NormalizedShort4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(0, 24, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(0, 40, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
        };
        #endregion

        private Texture TextureDif;
        private Texture TextureNrm;
        private VertexDeclaration VertexDeclaration;

        public override void Setup(GraphicsDevice device, DeformableWindow block, string basePath)
        {
            this.VertexDeclaration = new VertexDeclaration(device, VertexElements);

            string texturePath;

            texturePath = Path.Combine(basePath, block.Material.UndeformedDiffuseTexture);
            if (File.Exists(texturePath) == false)
            {
                this.TextureDif = null;
            }
            else
            {
                this.TextureDif = Texture.FromFile(device, texturePath);
            }

            texturePath = Path.Combine(basePath, block.Material.UndeformedNormalMap);
            if (File.Exists(texturePath) == false)
            {
                this.TextureNrm = null;
            }
            else
            {
                this.TextureNrm = Texture.FromFile(device, texturePath);
            }
        }

        public override void Render(GraphicsDevice device, DeformableWindow block)
        {
            VertexBuffer vertices;

            device.VertexDeclaration = this.VertexDeclaration;
            vertices = new VertexBuffer(
                device,
                block.VertexData0.Count * 48,
                BufferUsage.WriteOnly);
            vertices.SetData(block.VertexData0.ToArray());

            device.Vertices[0].SetSource(vertices, 0, 48);

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
