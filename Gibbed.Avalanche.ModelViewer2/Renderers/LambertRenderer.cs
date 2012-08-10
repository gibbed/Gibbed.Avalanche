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

using System.Runtime.InteropServices;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;
using IC = SlimDX.Direct3D10.InputClassification;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    public class LambertRenderer : GenericBlockRenderer<Lambert>
    {
        private readonly ShaderLoader _ShaderLoader = new ShaderLoader();
        private readonly MaterialLoader _MaterialLoader = new MaterialLoader();

        private Buffer _VertexData0Buffer;
        private Buffer _IndexBuffer;

        private ConstantBuffer<VertexShaderGlobals> _VertexShaderConstantBuffer1;

        public override void Setup(Device device,
                                   ShaderLibrary shaderLibrary,
                                   string basePath)
        {
            this._MaterialLoader.Setup(device, basePath, this.Block.Material);

            string vertexShaderName = "lambert";
            string pixelShaderName = "lambert";

            if (this.Block.Unknown28 == 0)
            {
                this._ShaderLoader.Setup(
                    device,
                    shaderLibrary.GetVertexShaderData(vertexShaderName),
                    shaderLibrary.GetFragmentShaderData(pixelShaderName),
                    new[]
                    {
                        new InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 0, DXGI.Format.R32G32B32A32_Float, 12, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 1, DXGI.Format.R32G32B32_Float, 28, 0, IC.PerVertexData, 0),
                    });

                var vertexData0Buffer = new Buffer(device,
                                                   40 * this.Block.VertexData0Big.Count,
                                                   ResourceUsage.Dynamic,
                                                   BindFlags.VertexBuffer,
                                                   CpuAccessFlags.Write,
                                                   ResourceOptionFlags.None);
                using (var stream = vertexData0Buffer.Map(MapMode.WriteDiscard,
                                                          MapFlags.None))
                {
                    stream.WriteRange(this.Block.VertexData0Big.ToArray());
                    vertexData0Buffer.Unmap();
                }
                this._VertexData0Buffer = vertexData0Buffer;
            }
            else if (this.Block.Unknown28 == 1)
            {
                this._ShaderLoader.Setup(
                    device,
                    shaderLibrary.GetVertexShaderData(vertexShaderName),
                    shaderLibrary.GetFragmentShaderData(pixelShaderName),
                    new[]
                    {
                        new InputElement("TEXCOORD", 0, DXGI.Format.R16G16B16A16_SNorm, 0, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 1, DXGI.Format.R32G32B32_Float, 8, 0, IC.PerVertexData, 0),
                        new InputElement("POSITION", 0, DXGI.Format.R16G16B16A16_SNorm, 20, 0, IC.PerVertexData, 0),
                    });

                var vertexData0Buffer = new Buffer(device,
                                                   28 * this.Block.VertexData0Small.Count,
                                                   ResourceUsage.Dynamic,
                                                   BindFlags.VertexBuffer,
                                                   CpuAccessFlags.Write,
                                                   ResourceOptionFlags.None);
                using (var stream = vertexData0Buffer.Map(MapMode.WriteDiscard,
                                                          MapFlags.None))
                {
                    stream.WriteRange(this.Block.VertexData0Small.ToArray());
                    vertexData0Buffer.Unmap();
                }
                this._VertexData0Buffer = vertexData0Buffer;
            }

            var indexBuffer = new Buffer(device,
                                         2 * this.Block.Faces.Count,
                                         ResourceUsage.Dynamic,
                                         BindFlags.IndexBuffer,
                                         CpuAccessFlags.Write,
                                         ResourceOptionFlags.None);
            using (var stream = indexBuffer.Map(MapMode.WriteDiscard,
                                                MapFlags.None))
            {
                stream.WriteRange(this.Block.Faces.ToArray());
                indexBuffer.Unmap();
            }
            this._IndexBuffer = indexBuffer;

            this._VertexShaderConstantBuffer1 = new ConstantBuffer<VertexShaderGlobals>(device);
        }

        public override void Render(Device device,
                                    SlimDX.Matrix viewMatrix)
        {
            var globals = new VertexShaderGlobals();
            globals.World = SlimDX.Matrix.Identity;
            globals.WorldViewProj = viewMatrix;
            this._VertexShaderConstantBuffer1.Update(globals);

            device.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);

            device.VertexShader.Set(this._ShaderLoader.VertexShader);
            device.VertexShader.SetConstantBuffer(this._VertexShaderConstantBuffer1.Buffer, 1);

            device.PixelShader.Set(this._ShaderLoader.PixelShader);

            this._MaterialLoader.SetShaderResource(device);
            device.InputAssembler.SetInputLayout(this._ShaderLoader.InputLayout);

            if (this.Block.Unknown28 == 0)
            {
                device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this._VertexData0Buffer, 40, 0));
            }
            else if (this.Block.Unknown28 == 1)
            {
                device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this._VertexData0Buffer, 28, 0));
            }

            device.InputAssembler.SetIndexBuffer(this._IndexBuffer, DXGI.Format.R16_UInt, 0);
            device.DrawIndexed(this.Block.Faces.Count, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential, Size = 32)]
        private struct VertexShaderGlobals
        {
            public ShaderNatives.float4x4 WorldViewProj;
            public ShaderNatives.float4x4 World;
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
