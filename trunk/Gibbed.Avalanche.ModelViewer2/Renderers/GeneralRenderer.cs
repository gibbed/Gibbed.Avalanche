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

using System.IO;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    internal class GeneralRenderer : GenericBlockRenderer<General>
    {
        /*
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
        */

        public override void Setup(
            SlimDX.Direct3D10.Device device,
            ShaderLibrary shaderLibrary,
            string basePath)
        {
            /*
            this.SmallVertexDeclaration = new VertexDeclaration(device, SmallVertexElements);
            this.BigVertexDeclaration = new VertexDeclaration(device, BigVertexElements);
            this.HackToFixDumbVertexDeclaration = new VertexDeclaration(device, HackToFixDumbVertexElements);
            
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
            */
        }

        public override void Render(SlimDX.Direct3D10.Device device, SlimDX.Matrix viewMatrix)
        {
            /*
            VertexBuffer vertices;
            int vertexSize;

            if (block.HasBigVertices == false)
            {
                *
                vertexSize = 28;
                device.VertexDeclaration = this.SmallVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.SmallVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.SmallVertices.ToArray());
                *
                vertexSize = 20;
                device.VertexDeclaration = this.HackToFixDumbVertexDeclaration;
                vertices = new VertexBuffer(
                    device,
                    block.HackToFixDumbVertices.Count * vertexSize,
                    BufferUsage.WriteOnly);
                vertices.SetData(block.HackToFixDumbVertices.ToArray());
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
            */
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
