using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX;
using DXGI = SlimDX.DXGI;
using SlimDX.Direct3D10;

using RbModel = Gibbed.Avalanche.FileFormats.RbModel;
using RenderBlock = Gibbed.Avalanche.FileFormats.RenderBlock;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class ModelViewRenderer
    {
        public void UpdateScene(string basePath, RbModel model, List<RenderBlock.IRenderBlock> selectedBlocks)
        {
            //this.GraphicsDevice.ClearRenderTargetView(this.RenderTargetView, new Color4(-12156236));
            //this.SwapChain.Present(0, DXGI.PresentFlags.None);
            System.Threading.Thread.Sleep(5);
        }

        public void UpdateCamera(float elapsedTime, bool handleInput)
        {
            //this.Camera.Update(elapsedTime, handleInput);
            //TODO: FIXME
            //this.BasicEffect.View = this.Camera.ViewMatrix;
        }
    }
}
