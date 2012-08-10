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

using System;
using System.IO;
using System.Runtime.InteropServices;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using SlimDX.Direct3D10;
using Buffer = SlimDX.Direct3D10.Buffer;
using DXGI = SlimDX.DXGI;
using IC = SlimDX.Direct3D10.InputClassification;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    internal class SkinnedGeneralRenderer : GenericBlockRenderer<SkinnedGeneral>
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

            string vertexShaderName;
            string pixelShaderName;

            var small = this.Block.HasBigVertices == false;

            if (this.Block.Mode == 4)
            {
                vertexShaderName = small ? "skinnedgeneraleyegloss" : "skinnedgeneraleyegloss8";
                pixelShaderName = "skinnedgeneraleyegloss";
            }
            else if (this.Block.Mode == 1)
            {
                vertexShaderName = small ? "skinnedgeneral" : "skinnedgeneral8";
                pixelShaderName = "skinnedgeneralhair";
            }
            else
            {
                vertexShaderName = small ? "skinnedgeneral" : "skinnedgeneral8";
                pixelShaderName = "skinnedgeneral";
            }

            if (this.Block.HasBigVertices == false)
            {
                this._ShaderLoader.Setup(
                    device,
                    shaderLibrary.GetVertexShaderData(vertexShaderName),
                    shaderLibrary.GetFragmentShaderData(pixelShaderName),
                    new[]
                    {
                        new InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 1, DXGI.Format.R8G8B8A8_UNorm, 12, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 2, DXGI.Format.R8G8B8A8_UInt, 16, 0, IC.PerVertexData, 0),
                        new InputElement("COLOR", 0, DXGI.Format.R8G8B8A8_UNorm, 0, 1, IC.PerVertexData, 0),
                        new InputElement("COLOR", 1, DXGI.Format.R8G8B8A8_UNorm, 4, 1, IC.PerVertexData, 0),
                        new InputElement("COLOR", 2, DXGI.Format.R8G8B8A8_UNorm, 8, 1, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 0, DXGI.Format.R32G32_Float, 12, 1, IC.PerVertexData, 0),
                    });

                var vertexBuffer = new Buffer(device,
                                                    20 * this.Block.VertexData0Small.Count,
                                                    ResourceUsage.Dynamic,
                                                    BindFlags.VertexBuffer,
                                                    CpuAccessFlags.Write,
                                                    ResourceOptionFlags.None);
                using (var stream = vertexBuffer.Map(MapMode.WriteDiscard,
                                                     MapFlags.None))
                {
                    stream.WriteRange(this.Block.VertexData0Small.ToArray());
                    vertexBuffer.Unmap();
                }
                this._VertexData0Buffer = vertexBuffer;
            }
            else
            {
                this._ShaderLoader.Setup(
                    device,
                    shaderLibrary.GetVertexShaderData(vertexShaderName),
                    shaderLibrary.GetFragmentShaderData(pixelShaderName),
                    new[]
                    {
                        new InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 1, DXGI.Format.R8G8B8A8_UNorm, 12, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 2, DXGI.Format.R8G8B8A8_UNorm, 16, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 3, DXGI.Format.R8G8B8A8_UInt, 20, 0, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 4, DXGI.Format.R8G8B8A8_UInt, 24, 0, IC.PerVertexData, 0),
                        new InputElement("COLOR", 0, DXGI.Format.R8G8B8A8_UNorm, 0, 1, IC.PerVertexData, 0),
                        new InputElement("COLOR", 1, DXGI.Format.R8G8B8A8_UNorm, 4, 1, IC.PerVertexData, 0),
                        new InputElement("COLOR", 2, DXGI.Format.R8G8B8A8_UNorm, 8, 1, IC.PerVertexData, 0),
                        new InputElement("TEXCOORD", 0, DXGI.Format.R32G32_Float, 12, 1, IC.PerVertexData, 0),
                    });

                var vertexBuffer = new Buffer(device,
                    28 * this.Block.VertexData0Big.Count,
                    ResourceUsage.Dynamic,
                    BindFlags.VertexBuffer,
                    CpuAccessFlags.Write,
                    ResourceOptionFlags.None);
                using (var stream = vertexBuffer.Map(MapMode.WriteDiscard,
                    MapFlags.None))
                {
                    stream.WriteRange(this.Block.VertexData0Big.ToArray());
                    vertexBuffer.Unmap();
                }
                this._VertexData0Buffer = vertexBuffer;
            }

            // Extra Buffer
            {
                var extraBuffer = new Buffer(device,
                                                   20 * this.Block.VertexData1.Count,
                                                   ResourceUsage.Dynamic,
                                                   BindFlags.VertexBuffer,
                                                   CpuAccessFlags.Write,
                                                   ResourceOptionFlags.None);
                using (var stream = extraBuffer.Map(MapMode.WriteDiscard,
                                                    MapFlags.None))
                {
                    stream.WriteRange(this.Block.VertexData1.ToArray());
                    extraBuffer.Unmap();
                }
                this._VertexData1Buffer = extraBuffer;
            }

            // Index Buffer
            {
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
            }

            // Constant Buffer
            {
                this._VertexShaderConstantBuffer1 = new ConstantBuffer<VertexShaderGlobals>(device);
                this._PixelShaderConstantBuffer0 = new ConstantBuffer<PixelShaderGlobalConstants>(device);
                this._PixelShaderConstantBuffer1 = new ConstantBuffer<PixelShaderInstanceConstants>(device);
                this._PixelShaderConstantBuffer2 = new ConstantBuffer<PixelShaderMaterialConstants>(device);
                this._PixelShaderConstantBuffer4 = new ConstantBuffer<PixelShaderBooleans>(device);
            }
        }

        public override void Render(Device device, SlimDX.Matrix viewMatrix)
        {
            foreach (var batch in this.Block.SkinBatches)
            {
                var globals = new VertexShaderGlobals();
                globals.MatrixPalette = new ShaderNatives.float3x4[18];

                int paletteIndex = 0;
                foreach (var boneIndex in batch.BoneIndices)
                {
                    if (boneIndex != -1)
                    {
                        globals.MatrixPalette[paletteIndex] = SlimDX.Matrix.Identity; //.Data[TestIndex % 18];
                    }

                    paletteIndex++;
                }
                globals.World = SlimDX.Matrix.Identity;
                globals.WorldViewProj = viewMatrix;
                this._VertexShaderConstantBuffer1.Update(globals);

                device.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
                device.InputAssembler.SetVertexBuffers(1, new VertexBufferBinding(this._VertexData1Buffer, 20, 0));

                device.VertexShader.Set(this._ShaderLoader.VertexShader);
                //device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 0);
                //device.VertexShader.SetConstantBuffer(this.ConstantBuffer1.Buffer, 0);
                device.VertexShader.SetConstantBuffer(this._VertexShaderConstantBuffer1.Buffer, 1);
                //device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 3);
                //device.VertexShader.SetConstantBuffer(this.ConstantBuffer, 4);

                var globalConsts = new PixelShaderGlobalConstants();
                globalConsts.Globals = new ShaderNatives.float4[15];

                globalConsts.LightPositions = new ShaderNatives.float4[65];
                for (int i = 0; i < 65; i++)
                {
                    globalConsts.LightPositions[i] = new ShaderNatives.float4(0.0f, 0.0f, 100.0f, 0.0f);
                }

                globalConsts.LightColors = new ShaderNatives.float4[65];
                for (int i = 0; i < 65; i++)
                {
                    globalConsts.LightColors[i] = new ShaderNatives.float4(1.0f, 0.0f, 0.0f, 1.0f);
                }

                //globalConsts.Globals[TestIndex % 15] = new ShaderNatives.float4(0.0f, 0.0f, 100.0f, 0.0f);
                //globalConsts.Globals[0] = new ShaderNatives.float4(11.0f, 1.0f, 1.0f, 1.0f);

                //globalConsts.Globals[1] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                //globalConsts.Globals[2] = new ShaderNatives.float4(0.0f, 1.0f, 0.0f, 1.0f);

                globalConsts.Globals[3] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                globalConsts.Globals[4] = new ShaderNatives.float4(0.0f, 25.0f, -25.0f, 0.0f);

                //globalConsts.Globals[1] = new ShaderNatives.float4(0.0f, 2.0f, 0.0f, 1.0f);
                //globalConsts.Globals[5] = new ShaderNatives.float4(0.0f, 0.0f, 25.0f, 0.0f);

                //globalConsts.Globals[7] = new ShaderNatives.float4(1.0f, 0.0f, 0.0f, 1.0f);
                //globalConsts.Globals[8] = new ShaderNatives.float4(1.0f, 0.0f, 0.0f, 1.0f);
                globalConsts.Globals[9] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                globalConsts.Globals[10] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                //globalConsts.Globals[11] = new ShaderNatives.float4(-1.0f, 1.0f, 1.0f, 1.0f);
                globalConsts.Globals[12] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                //globalConsts.Globals[14] = new ShaderNatives.float4(0.0f, 0.0f, 1.0f, 1.0f);
                //globalConsts.Globals[3] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);

                this._PixelShaderConstantBuffer0.Update(globalConsts);

                var instanceConsts = new PixelShaderInstanceConstants();
                instanceConsts.InstanceConstants = new ShaderNatives.float4[6];
                for (int i = 0; i < 6; i++)
                {
                    instanceConsts.InstanceConstants[i] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                }
                instanceConsts.InstanceConstants[1] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                this._PixelShaderConstantBuffer1.Update(instanceConsts);

                var materialConsts = new PixelShaderMaterialConstants();
                materialConsts.MaterialConstants = new ShaderNatives.float4[1];
                for (int i = 0; i < 1; i++)
                {
                    materialConsts.MaterialConstants[i] = new ShaderNatives.float4(1.0f, 1.0f, 1.0f, 1.0f);
                }
                this._PixelShaderConstantBuffer2.Update(materialConsts);

                var bs = new PixelShaderBooleans();
                bs.Bools = new ShaderNatives.bool4[4];
                bs.Bools[3].X = true;
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

                if (this.Block.HasBigVertices == false)
                {
                    device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this._VertexData0Buffer, 20, 0));
                }
                else
                {
                    device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this._VertexData0Buffer, 28, 0));
                }

                device.InputAssembler.SetIndexBuffer(this._IndexBuffer, DXGI.Format.R16_UInt, 0);
                device.DrawIndexed(batch.FaceCount, batch.FaceIndex, 0);
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 992)]
        private struct VertexShaderGlobals
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public ShaderNatives.float3x4[] MatrixPalette;

            public ShaderNatives.float4x4 WorldViewProj;
            public ShaderNatives.float4x4 World;
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

        [StructLayout(LayoutKind.Sequential, Size = 96)]
        private struct PixelShaderInstanceConstants
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public ShaderNatives.float4[] InstanceConstants;
        }

        [StructLayout(LayoutKind.Sequential, Size = 16)]
        private struct PixelShaderMaterialConstants
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
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
            if (this._PixelShaderConstantBuffer4 != null)
            {
                this._PixelShaderConstantBuffer4.Dispose();
            }

            if (this._PixelShaderConstantBuffer2 != null)
            {
                this._PixelShaderConstantBuffer2.Dispose();
            }

            if (this._PixelShaderConstantBuffer1 != null)
            {
                this._PixelShaderConstantBuffer1.Dispose();
            }

            if (this._PixelShaderConstantBuffer0 != null)
            {
                this._PixelShaderConstantBuffer0.Dispose();
            }

            if (this._VertexShaderConstantBuffer1 != null)
            {
                this._VertexShaderConstantBuffer1.Dispose();
            }

            if (this._IndexBuffer != null)
            {
                this._IndexBuffer.Dispose();
            }

            if (this._VertexData1Buffer != null)
            {
                this._VertexData1Buffer.Dispose();
            }

            if (this._VertexData0Buffer != null)
            {
                this._VertexData0Buffer.Dispose();
            }

            this._MaterialLoader.Dispose();
            this._ShaderLoader.Dispose();
        }
    }
}
