using System;
using System.IO;
using Gibbed.Avalanche.FileFormats.RenderBlock;
using D3D10 = SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    internal class SkinnedGeneralRenderer : GenericBlockRenderer<SkinnedGeneral>, IDisposable
    {
        /*
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
        private VertexShader SmallVertexShader;
        */

        private ShaderSet ShaderSet;

        private D3D10.Texture2D TextureDif;
        private D3D10.Texture2D TextureNrm;
        private D3D10.Texture2D TextureMpm;
        private D3D10.ShaderResourceView TextureDifResource;
        private D3D10.ShaderResourceView TextureNrmResource;
        private D3D10.ShaderResourceView TextureMpmResource;

        private D3D10.Buffer VertexBuffer;
        private D3D10.Buffer ExtraBuffer;
        private D3D10.Buffer IndexBuffer;
        private D3D10.Buffer ConstantBuffer;

        public override void Dispose()
        {
            if (this.TextureMpm != null)
            {
                this.TextureMpm.Dispose();
            }

            if (this.TextureNrm != null)
            {
                this.TextureNrm.Dispose();
            }

            if (this.TextureDif != null)
            {
                this.TextureDif.Dispose();
            }

            if (this.ConstantBuffer != null)
            {
                this.ConstantBuffer.Dispose();
            }

            if (this.IndexBuffer != null)
            {
                this.IndexBuffer.Dispose();
            }

            if (this.ExtraBuffer != null)
            {
                this.ExtraBuffer.Dispose();
            }

            if (this.VertexBuffer != null)
            {
                this.VertexBuffer.Dispose();
            }

            if (this.ShaderSet != null)
            {
                this.ShaderSet.Dispose();
            }
        }

        private static D3D10.Texture2D LoadTextureFrom(D3D10.Device device, string basePath, string fileName)
        {
            string filePath = Path.Combine(basePath, fileName);

            if (string.IsNullOrEmpty(fileName) == true ||
                File.Exists(filePath) == false)
            {
                return null;
            }

            return D3D10.Texture2D.FromFile(device, filePath);
        }

        public override void Setup(SlimDX.Direct3D10.Device device, SkinnedGeneral block, string basePath)
        {
            this.TextureDif = LoadTextureFrom(device, basePath, block.Textures[0]);
            this.TextureNrm = LoadTextureFrom(device, basePath, block.Textures[1]);
            this.TextureMpm = LoadTextureFrom(device, basePath, block.Textures[2]);

            if (this.TextureDif != null)
            {
                this.TextureDifResource = new D3D10.ShaderResourceView(device, this.TextureDif);
            }

            if (this.TextureNrm != null)
            {
                this.TextureNrmResource = new D3D10.ShaderResourceView(device, this.TextureNrm);
            }

            if (this.TextureMpm != null)
            {
                this.TextureMpmResource = new D3D10.ShaderResourceView(device, this.TextureMpm);
            }


            this.ShaderSet = new ShaderSet();

            if (block.HasBigVertices == false)
            {
                this.ShaderSet.Load(
                    device,
                    @"shaders\skinnedgeneral.vsb",
                    @"shaders\skinnedgeneral.psb",
                    new[]
                {
                    // "Small"
                    new D3D10.InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0, 0, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 1, DXGI.Format.R8G8B8A8_UNorm, 12, 0, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 2, DXGI.Format.R8G8B8A8_UInt, 16, 0, D3D10.InputClassification.PerVertexData, 0),

                    // "Extra"
                    new D3D10.InputElement("COLOR", 0, DXGI.Format.R8G8B8A8_UNorm, 0, 1, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("COLOR", 1, DXGI.Format.R8G8B8A8_UNorm, 4, 1, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("COLOR", 2, DXGI.Format.R8G8B8A8_UNorm, 8, 1, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 0, DXGI.Format.R32G32_Float, 12, 1, D3D10.InputClassification.PerVertexData, 0),
                });

                var vertexBuffer = new D3D10.Buffer(
                    device,
                    20 * block.SmallVertices.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.VertexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = vertexBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(block.SmallVertices.ToArray());
                vertexBuffer.Unmap();
                this.VertexBuffer = vertexBuffer;
            }
            else
            {
                this.ShaderSet.Load(
                    device,
                    @"shaders\skinnedgeneral8.vsb",
                    @"shaders\skinnedgeneral.psb",
                    new[]
                {
                    // "Big"
                    new D3D10.InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0, 0, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 1, DXGI.Format.R8G8B8A8_UNorm, 12, 0, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 2, DXGI.Format.R8G8B8A8_UNorm, 16, 0, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 3, DXGI.Format.R8G8B8A8_UInt, 20, 0, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 4, DXGI.Format.R8G8B8A8_UInt, 24, 0, D3D10.InputClassification.PerVertexData, 0),

                    // "Extra"
                    new D3D10.InputElement("COLOR", 0, DXGI.Format.R8G8B8A8_UNorm, 0, 1, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("COLOR", 1, DXGI.Format.R8G8B8A8_UNorm, 4, 1, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("COLOR", 2, DXGI.Format.R8G8B8A8_UNorm, 8, 1, D3D10.InputClassification.PerVertexData, 0),
                    new D3D10.InputElement("TEXCOORD", 0, DXGI.Format.R32G32_Float, 12, 1, D3D10.InputClassification.PerVertexData, 0),
                });

                var vertexBuffer = new D3D10.Buffer(
                    device,
                    28 * block.BigVertices.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.VertexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = vertexBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(block.BigVertices.ToArray());
                vertexBuffer.Unmap();
                this.VertexBuffer = vertexBuffer;
            }

            // Extra Buffer
            {
                var extraBuffer = new D3D10.Buffer(
                    device,
                    20 * block.Extras.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.VertexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = extraBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(block.Extras.ToArray());
                extraBuffer.Unmap();
                this.ExtraBuffer = extraBuffer;
            }

            // Index Buffer
            {
                var indexBuffer = new D3D10.Buffer(
                    device,
                    2 * block.Faces.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.IndexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = indexBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(block.Faces.ToArray());
                indexBuffer.Unmap();
                this.IndexBuffer = indexBuffer;
            }

            // Constant Buffer
            {
                var bytes = new byte[]
                {
                    0x90, 0x3F, 0x63, 0xBF, 0x88, 0xA5, 0xA5, 0xBE, 0x7F,
                    0xBD, 0xA7, 0x3E, 0x6D, 0xC7, 0x89, 0x3F, 0xF9, 0xA6, 0x65, 0xBE, 0x1F, 0x9E,
                    0xA2, 0xBE, 0x51, 0xDB, 0x6B, 0xBF, 0xAF, 0x75, 0xDD, 0x3F, 0xBE, 0xE3, 0xCD,
                    0x3E, 0x6D, 0x2D, 0x64, 0xBF, 0x66, 0x69, 0x56, 0x3E, 0x9A, 0xE4, 0x36, 0x3F,
                    0xAE, 0x97, 0x6D, 0xBF, 0xCF, 0x37, 0xAF, 0xBC, 0x31, 0x52, 0xBE, 0x3E, 0x39,
                    0xF9, 0x29, 0x3F, 0x42, 0x26, 0xBA, 0xBE, 0xF7, 0x2B, 0x27, 0xBE, 0x33, 0xCA,
                    0x6A, 0xBF, 0x3E, 0x9A, 0xCE, 0x3F, 0x6B, 0x75, 0xA4, 0x3D, 0x7D, 0x81, 0x7C,
                    0xBF, 0xDB, 0x30, 0x13, 0x3E, 0x70, 0x33, 0x8E, 0x3F, 0x1E, 0x32, 0x46, 0xBE,
                    0x37, 0xB4, 0x6D, 0xBF, 0x2C, 0x34, 0xA2, 0x3E, 0x21, 0x0A, 0xB2, 0x3F, 0x1B,
                    0x32, 0x14, 0x3E, 0x23, 0x5B, 0xB1, 0xBE, 0x5C, 0x46, 0x6D, 0xBF, 0x0B, 0xD4,
                    0xBB, 0x3F, 0x3E, 0x69, 0x78, 0x3F, 0x45, 0xC0, 0x08, 0xBE, 0xE6, 0x42, 0x4E,
                    0x3E, 0x90, 0x03, 0x5E, 0xBF, 0x1A, 0x21, 0x61, 0xBF, 0xF5, 0xED, 0x99, 0xBE,
                    0x67, 0xFC, 0xBC, 0x3E, 0xF4, 0x6B, 0x84, 0x3F, 0x27, 0x82, 0x22, 0xBE, 0x7D,
                    0xBD, 0x0B, 0xBF, 0xF2, 0x9C, 0x52, 0xBF, 0x0A, 0x83, 0x00, 0x40, 0xA2, 0xCC,
                    0xE5, 0x3E, 0x46, 0x36, 0x48, 0xBF, 0xF4, 0x59, 0xDD, 0x3E, 0x5E, 0xD8, 0x01,
                    0x3F, 0xEF, 0xF7, 0x22, 0x3F, 0xC0, 0x70, 0x35, 0xBF, 0xDE, 0xA5, 0x9B, 0x3E,
                    0xBA, 0x38, 0xBA, 0x3E, 0x75, 0xA6, 0xB8, 0x3E, 0x8F, 0x44, 0x99, 0xBD, 0x72,
                    0x00, 0x6E, 0xBF, 0xF2, 0x9A, 0x62, 0x3F, 0x37, 0x82, 0x2E, 0x3F, 0xDF, 0x93,
                    0x33, 0x3F, 0x56, 0xF5, 0x54, 0x3E, 0xD9, 0x81, 0xEC, 0xBF, 0xD6, 0x88, 0x5C,
                    0xBF, 0x2B, 0x5D, 0x23, 0x3D, 0x68, 0x99, 0x01, 0x3F, 0xC8, 0xA4, 0x06, 0x3F,
                    0x40, 0xFF, 0x01, 0xBF, 0xFE, 0x23, 0x76, 0xBD, 0xF2, 0xFF, 0x5B, 0xBF, 0x7C,
                    0x05, 0xC9, 0x3F, 0xFE, 0x7B, 0x7C, 0xBB, 0x3A, 0x55, 0x7F, 0xBF, 0xF7, 0x80,
                    0x93, 0x3D, 0x47, 0x1A, 0x98, 0x3F, 0x66, 0x30, 0x63, 0xBF, 0x3F, 0x4A, 0x2A,
                    0x3D, 0xD6, 0x02, 0xEB, 0x3E, 0x44, 0xD5, 0x0A, 0x3F, 0x12, 0xF9, 0xEB, 0xBE,
                    0xFF, 0x74, 0xA1, 0xBD, 0x8A, 0x4A, 0x62, 0xBF, 0xA4, 0xEA, 0xC7, 0x3F, 0xFF,
                    0x98, 0x13, 0xBA, 0x18, 0xFB, 0x7E, 0xBF, 0x7F, 0x88, 0xB6, 0x3D, 0x31, 0x91,
                    0x97, 0x3F, 0xEB, 0xB5, 0x2D, 0xBF, 0xFE, 0x24, 0x4F, 0x3C, 0xFB, 0x03, 0x3C,
                    0x3F, 0xFB, 0x8F, 0xDB, 0x3E, 0xFB, 0xC2, 0x3B, 0xBF, 0x5F, 0x58, 0x30, 0x3D,
                    0x85, 0xAA, 0x2D, 0xBF, 0x7B, 0x43, 0xCD, 0x3F, 0x2F, 0xA4, 0x24, 0xBD, 0xE6,
                    0xBD, 0x7F, 0xBF, 0x7F, 0x56, 0xA3, 0xBC, 0xFE, 0xB0, 0x9B, 0x3F, 0xAE, 0x45,
                    0x73, 0xBF, 0x68, 0xD9, 0x43, 0xBE, 0xD9, 0x9E, 0x7B, 0x3E, 0x25, 0x42, 0x6E,
                    0x3F, 0xDC, 0x8B, 0x7E, 0xBE, 0xFF, 0x39, 0x18, 0xBC, 0xE8, 0xF3, 0x77, 0xBF,
                    0x2E, 0x1D, 0xA6, 0x3F, 0xFB, 0x07, 0x40, 0x3E, 0x38, 0x43, 0x7B, 0xBF, 0x7F,
                    0x90, 0x1E, 0xBD, 0x29, 0x80, 0x82, 0x3F, 0xF6, 0x71, 0x49, 0xBF, 0x36, 0xB7,
                    0x12, 0xBF, 0x94, 0x44, 0x6A, 0x3E, 0x14, 0x9E, 0xAD, 0x3F, 0xE1, 0x05, 0x63,
                    0xBE, 0x1F, 0xD8, 0xAA, 0xBD, 0xBC, 0xB6, 0x78, 0xBF, 0x43, 0x33, 0xB1, 0x3F,
                    0x01, 0x6D, 0x13, 0x3F, 0x56, 0xB2, 0x50, 0xBF, 0xDE, 0x8D, 0x7B, 0xBD, 0x3F,
                    0x04, 0xEA, 0x3E, 0xB7, 0x0D, 0x17, 0xBF, 0x5E, 0xF0, 0x46, 0xBF, 0x96, 0x35,
                    0x60, 0x3E, 0x91, 0xAD, 0xBE, 0x3F, 0x24, 0xC1, 0x49, 0xBE, 0x6E, 0xC4, 0xFE,
                    0xBD, 0xF8, 0xF3, 0x78, 0xBF, 0xDF, 0x3A, 0xB6, 0x3F, 0x90, 0x6F, 0x48, 0x3F,
                    0x91, 0xF0, 0x1D, 0xBF, 0x1F, 0x3D, 0xA3, 0xBD, 0xE0, 0x71, 0x03, 0xBC, 0x16,
                    0xB2, 0xE9, 0xBE, 0xD4, 0x2E, 0x4A, 0xBF, 0xBE, 0xCD, 0xD1, 0x3E, 0xDE, 0x8D,
                    0xB3, 0x3F, 0xCC, 0xA0, 0x84, 0x3E, 0x07, 0xFD, 0x0E, 0xBF, 0x4A, 0xB9, 0x49,
                    0xBF, 0xC8, 0x70, 0xD7, 0x3F, 0xF6, 0xE8, 0x59, 0x3F, 0x36, 0xCD, 0x81, 0xBE,
                    0x52, 0x47, 0xEB, 0x3E, 0xEE, 0xE7, 0x18, 0xBF, 0x1D, 0x00, 0x06, 0x3F, 0x17,
                    0xCD, 0x31, 0xBF, 0xB5, 0xB7, 0xFC, 0x3E, 0xA9, 0xB8, 0xDF, 0x3E, 0x2B, 0x7D,
                    0x26, 0x3F, 0xBE, 0x1B, 0x47, 0xBD, 0x68, 0x11, 0x42, 0xBF, 0x1C, 0x4A, 0x1B,
                    0x3F, 0x0A, 0xEE, 0x0C, 0x3F, 0x8D, 0xC2, 0x37, 0x3F, 0xEE, 0x3C, 0xDA, 0x3E,
                    0x0E, 0x01, 0xE2, 0xBF, 0x44, 0x5E, 0x6B, 0xBF, 0xE1, 0x0C, 0xFB, 0xBD, 0xDD,
                    0x56, 0xBF, 0x3E, 0xA0, 0xFE, 0x4D, 0x3F, 0x16, 0x6A, 0x36, 0xBE, 0x97, 0xA3,
                    0x37, 0xBF, 0x8C, 0x6E, 0x2C, 0xBF, 0xE4, 0xB4, 0x10, 0x40, 0xC1, 0x87, 0xB3,
                    0x3E, 0xF8, 0x93, 0x2F, 0xBF, 0xF7, 0x3F, 0x23, 0x3F, 0x22, 0x10, 0xDB, 0x3E,
                    0xC9, 0x7E, 0x3C, 0xBF, 0xDB, 0x96, 0x09, 0xBF, 0x59, 0x78, 0xD2, 0x3E, 0xF0,
                    0x60, 0xA1, 0x3F, 0x8C, 0xE8, 0x53, 0x3E, 0x12, 0xDD, 0x41, 0xBF, 0x33, 0x93,
                    0x1E, 0xBF, 0x58, 0x7A, 0x00, 0x40, 0x8F, 0xEB, 0x24, 0x3F, 0x58, 0xF7, 0xBD,
                    0xBE, 0xB8, 0x37, 0x2B, 0x3F, 0xEC, 0x6C, 0x89, 0xBE, 0x32, 0x8F, 0x85, 0xBF,
                    0xD5, 0x43, 0xBD, 0xBE, 0xB8, 0x17, 0x02, 0x3E, 0xD0, 0x93, 0xB3, 0x3F, 0xB4,
                    0xBC, 0x86, 0xBD, 0xA8, 0x7A, 0x43, 0xBE, 0x06, 0x43, 0x8C, 0xBF, 0x9D, 0xC2,
                    0xC8, 0x3F, 0x85, 0x46, 0xC5, 0x3E, 0xE9, 0x4E, 0x84, 0xBF, 0xDF, 0xB4, 0x20,
                    0x3E, 0x1E, 0x98, 0x6E, 0x3F, 0x43, 0x75, 0x59, 0xBF, 0x6A, 0xBF, 0x9F, 0x3E,
                    0x0F, 0x6D, 0x26, 0x3F, 0x7C, 0xAD, 0x7F, 0x3E, 0x69, 0x5E, 0x0E, 0xBF, 0xFC,
                    0x44, 0xB6, 0x3E, 0x4D, 0xC3, 0x65, 0xBF, 0x5A, 0x42, 0x93, 0x3F, 0x06, 0x05,
                    0xEB, 0xBE, 0xC8, 0x1C, 0x81, 0xBF, 0x50, 0xE3, 0xEC, 0xBD, 0xE3, 0xC4, 0xCB,
                    0x3F, 0x3F, 0x72, 0x40, 0xBF, 0xC5, 0xC6, 0x97, 0x3E, 0x0A, 0x65, 0x44, 0x3F,
                    0xEB, 0x53, 0x4D, 0x3E, 0x26, 0xBD, 0x28, 0xBF, 0x6D, 0x16, 0xCF, 0x3E, 0x42,
                    0x5B, 0x4D, 0xBF, 0x75, 0x6B, 0x94, 0x3F, 0x84, 0xD9, 0xFB, 0xBE, 0xBC, 0xB9,
                    0x7E, 0xBF, 0x57, 0xB9, 0xC7, 0xBD, 0x2E, 0x72, 0xCC, 0x3F, 0x54, 0x36, 0x04,
                    0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x71, 0x48, 0x30, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0xC6,
                    0x27, 0x37, 0x00, 0x00, 0x80, 0xBF, 0xBD, 0x61, 0x91, 0xBF, 0x9C, 0x9E, 0x88,
                    0xC0, 0xB9, 0xFC, 0xFF, 0x3E, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x80, 0x3F,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80,
                    0x3F, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xDA, 0x0D, 0x46, 0x31, 0xFF, 0x4A, 0x43,
                    0x14, 0xBD, 0xB9, 0x44, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00,
                };

                var constantBuffer = new D3D10.Buffer(
                    device,
                    bytes.Length,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.ConstantBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = constantBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(bytes);
                constantBuffer.Unmap();
                this.ConstantBuffer = constantBuffer;
            }

            var sd = new D3D10.RasterizerStateDescription();
            sd.FillMode = SlimDX.Direct3D10.FillMode.Solid;
            sd.CullMode = SlimDX.Direct3D10.CullMode.None;

            device.Rasterizer.State = D3D10.RasterizerState.FromDescription(device, sd);
        }

        public override void Render(D3D10.Device device, SkinnedGeneral block)
        {
            device.InputAssembler.SetPrimitiveTopology(D3D10.PrimitiveTopology.TriangleList);
            device.InputAssembler.SetVertexBuffers(1, new D3D10.VertexBufferBinding(this.ExtraBuffer, 20, 0));

            device.VertexShader.Set(this.ShaderSet.VertexShader);
            device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 0);
            device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 1);
            device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 3);
            device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 4);
            
            device.PixelShader.Set(this.ShaderSet.PixelShader);
            device.PixelShader.SetShaderResource(this.TextureDifResource, 0);
            device.PixelShader.SetShaderResource(this.TextureNrmResource, 1);
            device.PixelShader.SetShaderResource(this.TextureMpmResource, 2);
            
            device.InputAssembler.SetInputLayout(this.ShaderSet.InputLayout);

            if (block.HasBigVertices == false)
            {
                device.InputAssembler.SetVertexBuffers(0, new D3D10.VertexBufferBinding(this.VertexBuffer, 20, 0));
            }
            else
            {
                device.InputAssembler.SetVertexBuffers(0, new D3D10.VertexBufferBinding(this.VertexBuffer, 28, 0));
            }

            device.InputAssembler.SetIndexBuffer(this.IndexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);
            device.DrawIndexed(block.Faces.Count, 0, 0);
        }
    }
}
