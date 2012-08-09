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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
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
