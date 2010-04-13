using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RbModel = Gibbed.Avalanche.FileFormats.RbModel;
using RenderBlock = Gibbed.Avalanche.FileFormats.RenderBlock;
using XInput = Microsoft.Xna.Framework.Input;
using XnaColor = Microsoft.Xna.Framework.Graphics.Color;

namespace Gibbed.Avalanche.ModelViewer
{
    internal class Renderer
    {
        private GraphicsDevice Device;
        private BasicEffect BasicEffect;
        private Camera Camera;
        private Control Control;
        private Dictionary<Type, Type> RendererTypes = new Dictionary<Type, Type>();
        private Dictionary<RenderBlock.IRenderBlock, Renderers.IBlockRenderer>
            BlockRenderers = new Dictionary<RenderBlock.IRenderBlock, Renderers.IBlockRenderer>();

        public Renderer(Control control)
        {
            this.Control = control;
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
            pp.MultiSampleType = MultiSampleType.None;

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

            this.Camera = new Camera(this.Control);
            this.Camera.MoveTo(new Vector3(-0.4f, 1.6f, -1.1f));
            this.Camera.LookTo(new Vector2(1.3f, -0.4f));
            
            this.RendererTypes.Clear();
            this.AddRendererType<RenderBlock.SkinnedGeneral, Renderers.SkinnedGeneralRenderer>();

            this.BlockRenderers.Clear();
        }

        private void AddRendererType<TRenderBlock, TBlockRenderer>()
            where TRenderBlock : RenderBlock.IRenderBlock
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

        public void UpdateScene(string basePath, RbModel model, List<RenderBlock.IRenderBlock> selectedBlocks)
        {
            this.Device.Clear(XnaColor.SteelBlue);
            this.Device.RenderState.CullMode = CullMode.None;
            this.Device.RenderState.FillMode = FillMode.Solid;

            if (model != null)
            {
                this.BasicEffect.Begin();
                foreach (var pass in this.BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    foreach (RenderBlock.IRenderBlock block in model.Blocks)
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

        public void UpdateCamera(bool handleInput)
        {
            if (handleInput == true)
            {
                // get input states
                MouseState mouse = Mouse.GetState();
                KeyboardState keyboard = Keyboard.GetState();
                GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

                // apply movement
                if (keyboard.IsKeyDown(XInput.Keys.W))
                {
                    Camera.ApplyForce(Camera.ForwardDirection);
                }
                if (keyboard.IsKeyDown(XInput.Keys.A))
                {
                    Camera.ApplyForce(-Camera.HorizontalDirection);
                }
                if (keyboard.IsKeyDown(XInput.Keys.S))
                {
                    Camera.ApplyForce(-Camera.ForwardDirection);
                }
                if (keyboard.IsKeyDown(XInput.Keys.D))
                {
                    Camera.ApplyForce(Camera.HorizontalDirection);
                }
                if (keyboard.IsKeyDown(XInput.Keys.Q))
                {
                    Camera.ApplyForce(new Vector3(0, -1, 0));
                }
                if (keyboard.IsKeyDown(XInput.Keys.E))
                {
                    Camera.ApplyForce(new Vector3(0, 1, 0));
                }
                if (keyboard.IsKeyDown(XInput.Keys.Up))
                {
                    Camera.ApplyLookForce(new Vector2(0, 0.1f));
                }
                if (keyboard.IsKeyDown(XInput.Keys.Left))
                {
                    Camera.ApplyLookForce(new Vector2(-0.1f, 0));
                }
                if (keyboard.IsKeyDown(XInput.Keys.Down))
                {
                    Camera.ApplyLookForce(new Vector2(0, -0.1f));
                }
                if (keyboard.IsKeyDown(XInput.Keys.Right))
                {
                    Camera.ApplyLookForce(new Vector2(0.1f, 0));
                }

                if (mouse.ScrollWheelValue < 0)
                    Camera.ApplyZoomForce(0.02f);
                else if (mouse.ScrollWheelValue > 0)
                    Camera.ApplyZoomForce(-0.02f);

                //if (mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                //{
                //    Camera.ApplyLookForce(new Vector2(-mouse.
                //}
                // if windowed, you must click the left mouse button to look around
                //if (DXUTIsWindowed())
                //{
                //    if (mouseState->rgbButtons[0] > 0)
                //        cam->ApplyLookForce(Vector2(-mouseState->lX * .01f, -mouseState->lY * .01f));
                //}
                //else
                //    cam->ApplyLookForce(Vector2(-mouseState->lX * .01f, -mouseState->lY * .01f));
            }

            // do the calculations
            this.Camera.Update();

            // update views
            this.BasicEffect.View = this.Camera.View;
            this.BasicEffect.Projection = this.Camera.Projection;
            this.BasicEffect.World = this.Camera.World;
        }
    }
}
