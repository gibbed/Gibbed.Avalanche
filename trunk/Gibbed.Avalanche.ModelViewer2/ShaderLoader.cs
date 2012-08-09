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
using SlimDX.D3DCompiler;
using SlimDX.Direct3D10;
using DataStream = SlimDX.DataStream;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class ShaderLoader : IDisposable
    {
        public GeometryShader GeometryShader;
        public VertexShader VertexShader;
        public PixelShader PixelShader;
        public InputLayout InputLayout;

        public void Setup(Device device,
                          byte[] vertexShaderData,
                          byte[] pixelShaderData,
                          InputElement[] inputElements)
        {
            if (inputElements == null)
            {
                throw new ArgumentNullException("inputElements");
            }

            if (vertexShaderData != null)
            {
                using (var dataStream = new DataStream(vertexShaderData, true, false))
                {
                    var byteCode = new ShaderBytecode(dataStream);
                    var shader = new VertexShader(device, byteCode);
                    var inputLayout = new InputLayout(device,
                                                      byteCode,
                                                      inputElements);
                    this.VertexShader = shader;
                    this.InputLayout = inputLayout;
                }
            }

            if (pixelShaderData != null)
            {
                using (var dataStream = new DataStream(pixelShaderData, true, false))
                {
                    var byteCode = new ShaderBytecode(dataStream);
                    var shader = new PixelShader(device, byteCode);
                    this.PixelShader = shader;
                }
            }
        }

        public void Dispose()
        {
            if (this.GeometryShader != null)
            {
                this.GeometryShader.Dispose();
            }

            if (this.VertexShader != null)
            {
                this.VertexShader.Dispose();
            }

            if (this.PixelShader != null)
            {
                this.PixelShader.Dispose();
            }

            if (this.InputLayout != null)
            {
                this.InputLayout.Dispose();
            }
        }
    }
}
