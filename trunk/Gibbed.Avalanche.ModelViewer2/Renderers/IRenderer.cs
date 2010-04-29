using System;
using Gibbed.Avalanche.FileFormats.RenderBlock;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    public interface IRenderer : IDisposable
    {
        void Setup(
            SlimDX.Direct3D10.Device device,
            IRenderBlock block,
            ShaderLibrary shaderLibrary,
            string basePath);
        void Render(SlimDX.Direct3D10.Device device, SlimDX.Matrix viewMatrix);
    }
}
