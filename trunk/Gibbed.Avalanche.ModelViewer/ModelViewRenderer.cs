using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using XInput = Microsoft.Xna.Framework.Input;
using XnaColor = Microsoft.Xna.Framework.Graphics.Color;

namespace Gibbed.Avalanche.ModelViewer
{
    internal class ModelViewRenderer
    {
        private GraphicsDevice Device;
        private BasicEffect BasicEffect;
        public ModelViewCamera Camera;
        private Control Control;
        private Dictionary<Type, Type> RendererTypes = new Dictionary<Type, Type>();
        private Dictionary<IRenderBlock, Renderers.IBlockRenderer>
            BlockRenderers = new Dictionary<IRenderBlock, Renderers.IBlockRenderer>();

        public ModelViewRenderer(Control control)
        {
            this.Control = control;
            this.Camera = new ModelViewCamera(this.Control);
        }

        public void CreateDevice()
        {
            var pp = new PresentationParameters();
            pp.BackBufferCount = 1;
            pp.IsFullScreen = false;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = this.Control.Width;
            pp.BackBufferHeight = this.Control.Height;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
            pp.EnableAutoDepthStencil = true;
            pp.PresentationInterval = PresentInterval.Default;
            pp.BackBufferFormat = SurfaceFormat.Unknown;
            pp.MultiSampleType = MultiSampleType.NonMaskable;

            this.Device = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                DeviceType.Hardware,
                this.Control.Handle,
                pp);

            this.BasicEffect = new BasicEffect(this.Device, null);
            this.BasicEffect.TextureEnabled = true;
            
            this.BasicEffect.Alpha = 1.0f;
            //this.BasicEffect.EnableDefaultLighting();

            this.BasicEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            this.BasicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            this.BasicEffect.SpecularPower = 15.0f;
            this.BasicEffect.AmbientLightColor = new Vector3(0.90f, 0.90f, 0.90f);

            this.BasicEffect.VertexColorEnabled = false;

            this.BasicEffect.World = Matrix.Identity;
            this.BasicEffect.Projection = this.Camera.ProjectionMatrix;

            this.RendererTypes.Clear();
            this.AddRendererType<CarPaint, Renderers.CarPaintRenderer>();
            this.AddRendererType<CarPaintSimple, Renderers.CarPaintSimpleRenderer>();
            this.AddRendererType<DeformableWindow, Renderers.DeformableWindowRenderer>();
            this.AddRendererType<General, Renderers.GeneralRenderer>();
            this.AddRendererType<SkinnedGeneral, Renderers.SkinnedGeneralRenderer>();
            this.BlockRenderers.Clear();
        }

        private void AddRendererType<TRenderBlock, TBlockRenderer>()
            where TRenderBlock : IRenderBlock
            where TBlockRenderer : Renderers.IBlockRenderer, new()
        {
            this.RendererTypes.Add(typeof(TRenderBlock), typeof(TBlockRenderer));
        }

        public void DestroyDevice()
        {
            if (this.Device != null)
            {
                this.Device.Dispose();
            }

            if (this.BasicEffect != null)
            {
                this.BasicEffect.Dispose();
            }
        }

        public void ResetDevice()
        {
            this.DestroyDevice();
            this.CreateDevice();
        }

        public void UpdateScene(string basePath, ModelFile model, List<IRenderBlock> selectedBlocks)
        {
            this.Device.Clear(XnaColor.SteelBlue);
            this.Device.RenderState.CullMode = CullMode.CullClockwiseFace;
            this.Device.RenderState.FillMode = FillMode.Solid;
            this.Device.RenderState.AlphaTestEnable = true;
            this.Device.RenderState.SourceBlend = Blend.One;
            this.Device.RenderState.DestinationBlend = Blend.One;

            if (model != null)
            {
                this.BasicEffect.Begin();
                foreach (var pass in this.BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    foreach (IRenderBlock block in model.Blocks)
                    {
                        if (this.BlockRenderers.ContainsKey(block) == false)
                        {
                            Type blockType = block.GetType();
                            
                            if (this.RendererTypes.ContainsKey(blockType) == false)
                            {
                                continue;
                            }

                            var blockRenderer = (Renderers.IBlockRenderer)Activator.CreateInstance(this.RendererTypes[blockType]);
                            blockRenderer.Setup(this.Device, block, basePath);
                            this.BlockRenderers.Add(block, blockRenderer);
                        }
                        
                        var oldMode = this.Device.RenderState.FillMode;
                        this.Device.RenderState.FillMode =
                            selectedBlocks.Contains(block) == true ?
                                FillMode.WireFrame : FillMode.Solid;

                        if (this.BlockRenderers.ContainsKey(block) != false)
                        {
                            Renderers.IBlockRenderer blockRenderer =
                                this.BlockRenderers[block];
                            blockRenderer.Render(this.Device, block);
                        }

                        this.Device.RenderState.FillMode = oldMode;
                    }


                    pass.End();
                }
                this.BasicEffect.End();
            }

            this.Device.Present();
            System.Threading.Thread.Sleep(5);
        }

        public void UpdateCamera(GameTime gameTime, bool handleInput)
        {
            this.Camera.Update(gameTime, handleInput);
            this.BasicEffect.View = this.Camera.ViewMatrix;
        }
    }
}
