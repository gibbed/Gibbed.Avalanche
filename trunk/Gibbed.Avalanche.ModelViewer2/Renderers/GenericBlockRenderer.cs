using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Avalanche.FileFormats.RenderBlock;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2.Renderers
{
    public abstract class GenericBlockRenderer<TRenderBlock> : IRenderer, IDisposable
        where TRenderBlock: IRenderBlock
    {
        protected TRenderBlock Block;

        public abstract void Setup(
            SlimDX.Direct3D10.Device device,
            ShaderLibrary shaderLibrary,
            string basePath);
        public void Setup(
            SlimDX.Direct3D10.Device device,
            IRenderBlock block,
            ShaderLibrary shaderLibrary,
            string basePath)
        {
            if (block != null && !(block is TRenderBlock))
            {
                throw new ArgumentException("wrong block type", "block");
            }

            this.Block = (TRenderBlock)block;
            this.Setup(device, shaderLibrary, basePath);
        }

        public abstract void Render(SlimDX.Direct3D10.Device device, SlimDX.Matrix viewMatrix);

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
