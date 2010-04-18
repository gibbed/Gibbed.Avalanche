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

        private static byte[] LoadBytes(string path)
        {
            using (Stream input = File.OpenRead(path))
            {
                byte[] data = new byte[input.Length];
                input.Read(data, 0, data.Length);
                return data;
            }
        }

        public void Load(D3D10.Device device, string vertexShaderPath, string pixelShaderPath, D3D10.InputElement[] inputElements)
        {
            if (vertexShaderPath == null)
            {
                throw new ArgumentNullException("vertexShaderPath");
            }

            if (vertexShaderPath != null && File.Exists(vertexShaderPath) == false)
            {
                throw new FileNotFoundException("vertex shader file missing", vertexShaderPath);
            }

            if (pixelShaderPath != null && File.Exists(pixelShaderPath) == false)
            {
                throw new FileNotFoundException("pixel shader file missing", pixelShaderPath);
            }

            //if (vertexShaderPath != null)
            {
                var byteCode = new D3D10.ShaderBytecode(LoadBytes(vertexShaderPath));
                var shader = new D3D10.VertexShader(device, byteCode);

                var inputLayout = new D3D10.InputLayout(
                    device,
                    byteCode,
                    inputElements);

                this.VertexShader = shader;
                this.InputLayout = inputLayout;
            }

            if (pixelShaderPath != null)
            {
                var byteCode = new D3D10.ShaderBytecode(LoadBytes(pixelShaderPath));
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
