using System;
using System.IO;
using Gibbed.Avalanche.FileFormats.RenderBlock;
using D3D10 = SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;
using System.Runtime.InteropServices;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    internal class SkinnedGeneralRenderer : GenericBlockRenderer<SkinnedGeneral>, IDisposable
    {
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

        private ConstantBuffer<SkinnedGeneralBuffer1> ConstantBuffer1;

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

            if (this.ConstantBuffer1 != null)
            {
                this.ConstantBuffer1.Dispose();
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

        public override void Setup(
            SlimDX.Direct3D10.Device device,
            ShaderLibrary shaderLibrary,
            string basePath)
        {
            this.TextureDif = LoadTextureFrom(device, basePath, this.Block.Textures[0]);
            this.TextureNrm = LoadTextureFrom(device, basePath, this.Block.Textures[1]);
            this.TextureMpm = LoadTextureFrom(device, basePath, this.Block.Textures[2]);

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

            if (this.Block.HasBigVertices == false)
            {
                this.ShaderSet.Load(
                    device,
                    shaderLibrary.GetVertexShaderData("skinnedgeneral"),
                    shaderLibrary.GetFragmentShaderData("skinnedgeneral"),
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
                    20 * this.Block.SmallVertices.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.VertexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = vertexBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(this.Block.SmallVertices.ToArray());
                vertexBuffer.Unmap();
                this.VertexBuffer = vertexBuffer;
            }
            else
            {
                this.ShaderSet.Load(
                    device,
                    shaderLibrary.GetVertexShaderData("skinnedgeneral8"),
                    shaderLibrary.GetFragmentShaderData("skinnedgeneral"),
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
                    28 * this.Block.BigVertices.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.VertexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = vertexBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(this.Block.BigVertices.ToArray());
                vertexBuffer.Unmap();
                this.VertexBuffer = vertexBuffer;
            }
            
            // Extra Buffer
            {
                var extraBuffer = new D3D10.Buffer(
                    device,
                    20 * this.Block.Extras.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.VertexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = extraBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(this.Block.Extras.ToArray());
                extraBuffer.Unmap();
                this.ExtraBuffer = extraBuffer;
            }

            // Index Buffer
            {
                var indexBuffer = new D3D10.Buffer(
                    device,
                    2 * this.Block.Faces.Count,
                    D3D10.ResourceUsage.Dynamic,
                    D3D10.BindFlags.IndexBuffer,
                    D3D10.CpuAccessFlags.Write,
                    D3D10.ResourceOptionFlags.None);
                SlimDX.DataStream stream = indexBuffer.Map(
                    D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
                stream.WriteRange(this.Block.Faces.ToArray());
                indexBuffer.Unmap();
                this.IndexBuffer = indexBuffer;
            }

            // Constant Buffer
            {
                this.ConstantBuffer1 = new ConstantBuffer<SkinnedGeneralBuffer1>(device);
            }

            var sd = new D3D10.RasterizerStateDescription();
            sd.FillMode = SlimDX.Direct3D10.FillMode.Solid;
            sd.CullMode = SlimDX.Direct3D10.CullMode.None;

            device.Rasterizer.State = D3D10.RasterizerState.FromDescription(device, sd);
        }

        public override void Render(D3D10.Device device, SlimDX.Matrix viewMatrix)
        {
            ShaderNatives.float3x4 junk = new ShaderNatives.float3x4();
            junk.V11 = 0.0f;
            junk.V12 = 0.0f;
            junk.V13 = 0.0f;
            junk.V14 = 0.0f;
            junk.V21 = 0.0f;
            junk.V22 = 0.0f;
            junk.V23 = 0.0f;
            junk.V24 = 0.0f;
            junk.V31 = 0.0f;
            junk.V32 = 0.0f;
            junk.V33 = 0.0f;
            junk.V34 = 0.0f;

            SkinnedGeneralBuffer1 buffer1 = new SkinnedGeneralBuffer1();
            buffer1.Unknown = new ShaderNatives.float3x4[18];
            for (int i = 0; i < buffer1.Unknown.Length; i++)
            {
                buffer1.Unknown[i] = junk;
            }
            buffer1.World = SlimDX.Matrix.Identity;
            buffer1.WorldViewProj = viewMatrix;
            this.ConstantBuffer1.Update(buffer1);

            device.InputAssembler.SetPrimitiveTopology(D3D10.PrimitiveTopology.TriangleList);
            device.InputAssembler.SetVertexBuffers(1, new D3D10.VertexBufferBinding(this.ExtraBuffer, 20, 0));

            device.VertexShader.Set(this.ShaderSet.VertexShader);
            //device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 0);
            device.VertexShader.SetConstantBuffer(this.ConstantBuffer1.Buffer, 1);
            //device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 3);
            //device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 4);
            
            device.PixelShader.Set(this.ShaderSet.PixelShader);
            device.PixelShader.SetShaderResource(this.TextureDifResource, 0);
            device.PixelShader.SetShaderResource(this.TextureNrmResource, 1);
            device.PixelShader.SetShaderResource(this.TextureMpmResource, 2);
            
            device.InputAssembler.SetInputLayout(this.ShaderSet.InputLayout);

            if (this.Block.HasBigVertices == false)
            {
                device.InputAssembler.SetVertexBuffers(0, new D3D10.VertexBufferBinding(this.VertexBuffer, 20, 0));
            }
            else
            {
                device.InputAssembler.SetVertexBuffers(0, new D3D10.VertexBufferBinding(this.VertexBuffer, 28, 0));
            }

            device.InputAssembler.SetIndexBuffer(this.IndexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);
            device.DrawIndexed(this.Block.Faces.Count, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential, Size = 1024)]
        private struct SkinnedGeneralBuffer1
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public ShaderNatives.float3x4[] Unknown;
            public ShaderNatives.float4x4 World;
            public ShaderNatives.float4x4 WorldViewProj;
        }
    }
}
