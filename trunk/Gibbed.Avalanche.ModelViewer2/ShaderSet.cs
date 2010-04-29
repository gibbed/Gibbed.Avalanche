using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using D3D10 = SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class ShaderSet : IDisposable
    {
        public D3D10.GeometryShader GeometryShader;
        public D3D10.VertexShader VertexShader;
        public D3D10.PixelShader PixelShader;
        public D3D10.InputLayout InputLayout;

        public void Load(D3D10.Device device, byte[] vertexShaderData, byte[] pixelShaderData, D3D10.InputElement[] inputElements)
        {
            if (vertexShaderData == null)
            {
                throw new ArgumentNullException("vertexShaderPath");
            }
            else if (inputElements == null)
            {
                throw new ArgumentNullException("inputElements");
            }

            if (vertexShaderData != null)
            {
                var byteCode = new D3D10.ShaderBytecode(vertexShaderData);
                var shader = new D3D10.VertexShader(device, byteCode);

                var inputLayout = new D3D10.InputLayout(
                    device,
                    byteCode,
                    inputElements);

                this.VertexShader = shader;
                this.InputLayout = inputLayout;
            }

            if (pixelShaderData != null)
            {
                var byteCode = new D3D10.ShaderBytecode(pixelShaderData);
                var shader = new D3D10.PixelShader(device, byteCode);

                this.PixelShader = shader;
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
