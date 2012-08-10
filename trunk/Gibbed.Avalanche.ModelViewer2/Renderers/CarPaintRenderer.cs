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
    internal class CarPaintRenderer : GenericBlockRenderer<CarPaint>
    {
        private readonly ShaderLoader _ShaderLoader = new ShaderLoader();
        private readonly MaterialLoader _MaterialLoader = new MaterialLoader();

        private Buffer _VertexData0Buffer;
        private Buffer _VertexData1Buffer;
        private Buffer _IndexBuffer;

        private ConstantBuffer<VertexShaderGlobals> _VertexShaderConstantBuffer1;
        private ConstantBuffer<PixelShaderGlobalConstants> _PixelShaderConstantBuffer0;
        private ConstantBuffer<PixelShaderInstanceConstants> _PixelShaderConstantBuffer1;
        private ConstantBuffer<PixelShaderMaterialConstants> _PixelShaderConstantBuffer2;
        private ConstantBuffer<PixelShaderBooleans> _PixelShaderConstantBuffer4;

        public override void Setup(Device device,
                                   ShaderLibrary shaderLibrary,
                                   string basePath)
        {
            this._MaterialLoader.Setup(device, basePath, this.Block.Material);

            string vertexShaderName = "carpaint";
            string pixelShaderName = "carpaint";

            this._ShaderLoader.Setup(
                device,
                shaderLibrary.GetVertexShaderData(vertexShaderName),
                shaderLibrary.GetFragmentShaderData(pixelShaderName),
                new[]
                {
                    new InputElement("POSITION", 0, DXGI.Format.R32G32B32A32_Float, 0, 0, IC.PerVertexData, 0),
                    new InputElement("TEXCOORD", 0, DXGI.Format.R16G16B16A16_SNorm, 16, 0, IC.PerVertexData, 0),
                    new InputElement("TEXCOORD", 1, DXGI.Format.R32G32B32_Float, 0, 1, IC.PerVertexData, 0),
                    new InputElement("TEXCOORD", 2, DXGI.Format.R32G32B32A32_Float, 12, 1, IC.PerVertexData, 0),
                });

            var vertexData0Buffer = new Buffer(device,
                                               24 * this.Block.VertexData0.Count,
                                               ResourceUsage.Dynamic,
                                               BindFlags.VertexBuffer,
                                               CpuAccessFlags.Write,
                                               ResourceOptionFlags.None);
            using (var stream = vertexData0Buffer.Map(MapMode.WriteDiscard,
                                                      MapFlags.None))
            {
                stream.WriteRange(this.Block.VertexData0.ToArray());
                vertexData0Buffer.Unmap();
            }
            this._VertexData0Buffer = vertexData0Buffer;

            var vertexData1Buffer = new Buffer(device,
                                               28 * this.Block.VertexData1.Count,
                                               ResourceUsage.Dynamic,
                                               BindFlags.VertexBuffer,
                                               CpuAccessFlags.Write,
                                               ResourceOptionFlags.None);
            using (var stream = vertexData1Buffer.Map(MapMode.WriteDiscard,
                                                      MapFlags.None))
            {
                stream.WriteRange(this.Block.VertexData1.ToArray());
                vertexData1Buffer.Unmap();
            }
            this._VertexData1Buffer = vertexData1Buffer;

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

            // Constant Buffer
            this._VertexShaderConstantBuffer1 = new ConstantBuffer<VertexShaderGlobals>(device);
            this._PixelShaderConstantBuffer0 = new ConstantBuffer<PixelShaderGlobalConstants>(device);
            this._PixelShaderConstantBuffer1 = new ConstantBuffer<PixelShaderInstanceConstants>(device);
            this._PixelShaderConstantBuffer2 = new ConstantBuffer<PixelShaderMaterialConstants>(device);
            this._PixelShaderConstantBuffer4 = new ConstantBuffer<PixelShaderBooleans>(device);
        }

        private int TestIndex = 0;

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

            var globalConsts = new PixelShaderGlobalConstants();
            globalConsts.Globals = new ShaderNatives.float4[15];

            globalConsts.LightPositions = new ShaderNatives.float4[65];
            for (int i = 0; i < 65; i++)
            {
                globalConsts.LightPositions[i] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            }

            globalConsts.LightColors = new ShaderNatives.float4[65];
            for (int i = 0; i < 65; i++)
            {
                globalConsts.LightColors[i] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            }

            //globalConsts.Globals[TestIndex % 15] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            //TestIndex++;

            globalConsts.Globals[0] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[1] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[2] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            
            globalConsts.Globals[3] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[4] = new ShaderNatives.float4(0.0f, 25.0f, -25.0f, 0.0f);

            globalConsts.Globals[5] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[6] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[7] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[8] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[9] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[10] = new ShaderNatives.float4(1, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[11] = new ShaderNatives.float4(1.0f, 1, 1.0f, 1.0f);
            globalConsts.Globals[12] = new ShaderNatives.float4(1.0f, 1.0f, 1, 1.0f);
            globalConsts.Globals[13] = new ShaderNatives.float4(111, 1.0f, 1.0f, 1.0f);
            globalConsts.Globals[14] = new ShaderNatives.float4(1.0f, 111, 1.0f, 1.0f);

            this._PixelShaderConstantBuffer0.Update(globalConsts);

            var instanceConsts = new PixelShaderInstanceConstants();
            instanceConsts.InstanceConstants = new ShaderNatives.float4[8];
            instanceConsts.InstanceConstants[0] = new ShaderNatives.float4(0.0f, 1.0f, 1.0f, 1.0f);
            instanceConsts.InstanceConstants[1] = new ShaderNatives.float4(1.0f, 1.0f, 0.0f, 1.0f);
            instanceConsts.InstanceConstants[2] = new ShaderNatives.float4(0.0f, 1.0f, 1.0f, 1.0f);
            instanceConsts.InstanceConstants[3] = new ShaderNatives.float4(1.0f, 0.0f, 1.0f, 1.0f);
            instanceConsts.InstanceConstants[4] = new ShaderNatives.float4(1.0f, 1.0f, 0.0f, 1.0f);
            instanceConsts.InstanceConstants[5] = new ShaderNatives.float4(0.0f, 0.0f, 1.0f, 1.0f);
            
            instanceConsts.InstanceConstants[6] = new ShaderNatives.float4(0.0f, 1.0f, 0.0f, 0.0f);
            instanceConsts.InstanceConstants[7] = new ShaderNatives.float4(1.0f, 0.0f, 0.0f, 0.0f);
            this._PixelShaderConstantBuffer1.Update(instanceConsts);

            var materialConsts = new PixelShaderMaterialConstants();
            materialConsts.MaterialConstants = new ShaderNatives.float4[3];
            for (int i = 0; i < 3; i++)
            {
                materialConsts.MaterialConstants[i] = new ShaderNatives.float4(1.0f, 1.0f, 0.0f, 1.0f);
            }
            this._PixelShaderConstantBuffer2.Update(materialConsts);

            var bs = new PixelShaderBooleans();
            bs.Bools = new ShaderNatives.bool4[4];
            bs.Bools[3].X = false;
            bs.Bools[3].Y = false;
            bs.Bools[3].Z = false;
            bs.AlphaRef = 0.0f;
            this._PixelShaderConstantBuffer4.Update(bs);

            device.PixelShader.Set(this._ShaderLoader.PixelShader);
            device.PixelShader.SetConstantBuffer(this._PixelShaderConstantBuffer0.Buffer, 0);
            device.PixelShader.SetConstantBuffer(this._PixelShaderConstantBuffer1.Buffer, 1);
            device.PixelShader.SetConstantBuffer(this._PixelShaderConstantBuffer2.Buffer, 2);
            device.PixelShader.SetConstantBuffer(this._PixelShaderConstantBuffer4.Buffer, 4);

            this._MaterialLoader.SetShaderResource(device);
            device.InputAssembler.SetInputLayout(this._ShaderLoader.InputLayout);

            device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this._VertexData0Buffer, 24, 0));
            device.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this._VertexData1Buffer, 28, 0));

            device.InputAssembler.SetIndexBuffer(this._IndexBuffer, DXGI.Format.R16_UInt, 0);
            device.DrawIndexed(this.Block.Faces.Count, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential, Size = 992)]
        private struct VertexShaderGlobals
        {
            public ShaderNatives.float4x4 World;
            public ShaderNatives.float3x4 dummy1;
            public ShaderNatives.float3x4 dummy2;
            public ShaderNatives.float4x4 WorldViewProj;
        }

        [StructLayout(LayoutKind.Sequential, Size = 2320)]
        private struct PixelShaderGlobalConstants
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public ShaderNatives.float4[] Globals;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
            public ShaderNatives.float4[] LightPositions;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
            public ShaderNatives.float4[] LightColors;
        }

        [StructLayout(LayoutKind.Sequential, Size = 128)]
        private struct PixelShaderInstanceConstants
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public ShaderNatives.float4[] InstanceConstants;
        }

        [StructLayout(LayoutKind.Sequential, Size = 48)]
        private struct PixelShaderMaterialConstants
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ShaderNatives.float4[] MaterialConstants;
        }

        [StructLayout(LayoutKind.Sequential, Size = 68)]
        private struct PixelShaderBooleans
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ShaderNatives.bool4[] Bools;
            public float AlphaRef;
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
