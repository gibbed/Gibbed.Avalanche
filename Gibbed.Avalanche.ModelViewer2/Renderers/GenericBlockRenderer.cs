using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Avalanche.FileFormats.RenderBlock;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    public abstract class GenericBlockRenderer<TRenderBlock> : IBlockRenderer, IDisposable
        where TRenderBlock: IRenderBlock
    {
        public abstract void Setup(SlimDX.Direct3D10.Device device, TRenderBlock block, string basePath);
        public void Setup(SlimDX.Direct3D10.Device device, IRenderBlock block, string basePath)
        {
            if (block != null && !(block is TRenderBlock))
            {
                throw new ArgumentException("wrong block type", "block");
            }

            this.Setup(device, (TRenderBlock)block, basePath);
        }

        public abstract void Render(SlimDX.Direct3D10.Device device, TRenderBlock block);
        public void Render(SlimDX.Direct3D10.Device device, IRenderBlock block)
        {
            if (block != null && !(block is TRenderBlock))
            {
                throw new ArgumentException("wrong block type", "block");
            }

            this.Render(device, (TRenderBlock)block);
        }

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
