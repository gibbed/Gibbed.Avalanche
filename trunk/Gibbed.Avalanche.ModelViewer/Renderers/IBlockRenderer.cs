using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using Microsoft.Xna.Framework.Graphics;

namespace Gibbed.Avalanche.ModelViewer.Renderers
{
    public interface IBlockRenderer
    {
        void Setup(GraphicsDevice device, IRenderBlock block, string basePath);
        void Render(GraphicsDevice device, IRenderBlock block);
    }
}
