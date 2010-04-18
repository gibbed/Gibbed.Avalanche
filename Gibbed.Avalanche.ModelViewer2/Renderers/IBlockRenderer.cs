using System;
using Gibbed.Avalanche.FileFormats.RenderBlock;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    public interface IBlockRenderer : IDisposable
    {
        void Setup(SlimDX.Direct3D10.Device device, IRenderBlock block, string basePath);
        void Render(SlimDX.Direct3D10.Device device, IRenderBlock block);
    }
}
