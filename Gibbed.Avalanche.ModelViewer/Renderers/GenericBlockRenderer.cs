using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;

namespace Gibbed.Avalanche.ModelViewer.Renderers
{
    public abstract class GenericBlockRenderer<TRenderBlock> : IBlockRenderer
        where TRenderBlock: IRenderBlock
    {
        public abstract void Setup(GraphicsDevice device, TRenderBlock block, string basePath);
        public void Setup(GraphicsDevice device, IRenderBlock block, string basePath)
        {
            if (block != null && !(block is TRenderBlock))
            {
                throw new ArgumentException("wrong block type", "block");
            }

            this.Setup(device, (TRenderBlock)block, basePath);
        }

        public abstract void Render(GraphicsDevice device, TRenderBlock block);
        public void Render(GraphicsDevice device, IRenderBlock block)
        {
            if (block != null && !(block is TRenderBlock))
            {
                throw new ArgumentException("wrong block type", "block");
            }

            this.Render(device, (TRenderBlock)block);
        }
    }
}
