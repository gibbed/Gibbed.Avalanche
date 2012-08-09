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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class ViewportControl : UserControl
    {
        public ViewportControl()
        {
            this._Clock = new Clock();
            this._Clock.Start();

            this._Camera = new InputCamera(this);

            this.ClearColor = Color.SteelBlue;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Selectable, true);

            this._RenderEventArgs = new RenderEventArgs();
            this._UpdateSceneEventArgs = new UpdateSceneEventArgs();

            this.Enter += this.OnEnter;
            this.Leave += this.OnLeave;
            this.Resize += this.OnResize;
            this.MouseDown += this.OnMouseDown;
            this.MouseUp += this.OnMouseUp;
            this.MouseWheel += OnMouseWheel;
            this.MouseMove += this.OnMouseMove;
            this.MouseEnter += this.OnMouseEnter;
            this.MouseLeave += this.OnMouseLeave;
        }

        public enum CameraModes
        {
            None,
            Lookaround,
            Panning,
        }

        private readonly InputCamera _Camera;
        private readonly Clock _Clock;

        [DefaultValue(typeof(Color), "SteelBlue")]
        public Color ClearColor { get; set; }

        public event RenderEventHandler Initialize;
        public event UpdateSceneEventHandler UpdateScene;
        public event RenderEventHandler Render;
        public event EventHandler Uninitialize;

        private Device _Device;
        private DXGI.SwapChain _SwapChain;
        private Texture2D _DepthStencilBuffer;
        private RenderTargetView _RenderTargetView;
        private DepthStencilView _DepthStencilView;

        private readonly RenderEventArgs _RenderEventArgs;
        private readonly UpdateSceneEventArgs _UpdateSceneEventArgs;

        private Cursor _InvisibleCursor = new Cursor(new Bitmap(1, 1).GetHicon());
        private Cursor _DefaultCursor = Cursors.Default;

        private bool _BlockNextKeyRepeats = false;
        private bool _CameraEnabled = true;
        private CameraModes _CameraMode;
        private bool _CaptureMouse;
        private bool _CaptureWheel;
        private Point _CapturedMousePosition;
        private bool _MouseOver;

        public bool BlockNextKeyRepeats
        {
            get { return this._BlockNextKeyRepeats; }
            set { this._BlockNextKeyRepeats = value; }
        }

        public bool CameraEnabled
        {
            get { return this._CameraEnabled; }
            set { this._CameraEnabled = value; }
        }

        public CameraModes CameraMode
        {
            get { return this._CameraMode; }
            set
            {
                if (this._CameraMode == value)
                {
                    return;
                }

                this._CameraMode = value;
                this.UpdateCameraMode();
            }
        }

        public bool CaptureMouse
        {
            get { return this._CaptureMouse; }
            set
            {
                if (this._CaptureMouse == value)
                {
                    return;
                }
                this._CaptureMouse = value;
                this.UpdateCaptureMouse();
            }
        }

        public bool CaptureWheel
        {
            get { return this._CaptureWheel; }

            set { this._CaptureWheel = value; }
        }

        private void ResetCameraState()
        {
            this._Camera.ForwardInput = 0f;
            this._Camera.LateralInput = 0f;
            this._Camera.SpeedFactor = 1f;
        }

        private void UpdateCameraState()
        {
            if (this.Focused == false)
            {
                this.ResetCameraState();
                return;
            }

            var keyboardLayout = Native.GetKeyboardLayout(0);
            int keyForward = Native.MapVirtualKeyEx(17, 1, keyboardLayout);
            int keyBackward = Native.MapVirtualKeyEx(31, 1, keyboardLayout);
            int keyLeft = Native.MapVirtualKeyEx(30, 1, keyboardLayout);
            int keyRight = Native.MapVirtualKeyEx(32, 1, keyboardLayout);

            if (Native.IsKeyDown(keyForward) == true)
            {
                this._Camera.ForwardInput = 1f;
            }
            else if (Native.IsKeyDown(keyBackward) == true)
            {
                this._Camera.ForwardInput = -1f;
            }
            else
            {
                this._Camera.ForwardInput = 0f;
            }

            if (Native.IsKeyDown(keyLeft) == true)
            {
                this._Camera.LateralInput = -1f;
            }
            else if (Native.IsKeyDown(keyRight) == true)
            {
                this._Camera.LateralInput = 1f;
            }
            else
            {
                this._Camera.LateralInput = 0f;
            }

            if (Native.IsKeyDown(Native.VirtualKeys.LeftShift) == true ||
                Native.IsKeyDown(Native.VirtualKeys.RightShift) == true)
            {
                this._Camera.SpeedFactor = 5f;
            }
            else
            {
                this._Camera.SpeedFactor = 1f;
            }
        }

        private void UpdateCameraMode()
        {
            if (this.CameraMode != CameraModes.None)
            {
                this.CaptureMouse = true;
                this.UpdateCameraState();
                return;
            }

            this.CaptureMouse = false;
            this._Camera.ForwardInput = 0f;
            this._Camera.LateralInput = 0f;
        }

        private void UpdateCaptureMouse()
        {
            if (this.CaptureMouse == true)
            {
                this.Cursor = this._InvisibleCursor;
                this._CapturedMousePosition = Cursor.Position;
                Cursor.Position = this.PointToScreen(new Point(this.Width / 2, this.Height / 2));
            }
            else
            {
                Cursor.Position = this._CapturedMousePosition;
                this.Cursor = this._DefaultCursor;
            }
        }

        private void OnEnter(object sender, EventArgs eventArgs)
        {
        }

        private void OnLeave(object sender, EventArgs eventArgs)
        {
            this.CameraMode = CameraModes.None;
            this.ResetCameraState();
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (this.CameraMode == CameraModes.None)
            {
                var button = e.Button;

                if (button == MouseButtons.Left)
                {
                    if (this.CameraEnabled == true)
                    {
                        this.CameraMode = CameraModes.Lookaround;
                    }
                }
                else if (button == MouseButtons.Right)
                {
                    if (this.CameraEnabled == true)
                    {
                        this.CameraMode = CameraModes.Panning;
                    }
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (this.CameraMode != CameraModes.None)
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    this.CameraMode = CameraModes.None;
                }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            if (this._CaptureWheel == true)
            {
                //this._Camera.Position += this._Camera.FrontVector * (float)e.Delta * 0.0625f;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (this.CaptureMouse == true)
            {
                if (Native.IsWindowActive(this.Parent.Handle) == true)
                {
                    var position = this.PointToScreen(new Point(this.Width / 2,
                                                                this.Height / 2));
                    int rx = Cursor.Position.X - position.X;
                    int ry = Cursor.Position.Y - position.Y;

                    if (rx != 0 || ry != 0)
                    {
                        switch (this.CameraMode)
                        {
                            case CameraModes.Lookaround:
                            {
                                /*Camera.Rotate((float)(EditorSettings.InvertMouseView ? ry : (-(float)ry)) * 0.005f,
                                              0f,
                                              (float)(-(float)rx) * 0.005f);*/
                                break;
                            }

                            case CameraModes.Panning:
                            {
                                /*Camera.Position += Camera.RightVector * (float)rx * 0.125f +
                                                   Camera.UpVector *
                                                   (float)(EditorSettings.InvertMousePan ? ry : (-(float)ry)) *
                                                   0.125f;*/
                                break;
                            }
                        }

                        Cursor.Position = position;
                    }
                }
            }
        }

        private void OnMouseEnter(object sender, EventArgs eventArgs)
        {
            if (Native.IsWindowActive(this.Parent.Handle) == true)
            {
                this.Focus();
            }

            this._MouseOver = true;
        }

        private void OnMouseLeave(object sender, EventArgs eventArgs)
        {
            this.CameraMode = CameraModes.None;
            this._MouseOver = false;
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            if (msg.Msg == Native.WM_KEYDOWN ||
                msg.Msg == Native.WM_SYSKEYDOWN)
            {
                if ((msg.LParam.ToInt32() & 0x40000000) != 0)
                {
                    if (this.BlockNextKeyRepeats == true)
                    {
                        return true;
                    }
                }
                else
                {
                    this.BlockNextKeyRepeats = false;
                }
            }

            return base.PreProcessMessage(ref msg);
        }

        protected override bool ProcessKeyMessage(ref Message m)
        {
            this.UpdateCameraState();
            return base.ProcessKeyMessage(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            this.DestroyDevice();
            base.Dispose(disposing);
        }

        public void SetCameraBehavior(BasicCamera.Behavior behavior)
        {
            this._Camera.CurrentBehavior = behavior;
        }

        public void SetupCamera(Vector3 position, Vector3 center, float distance)
        {
            this._Camera.Stop();
            this._Camera.LookAt(position, center, new Vector3(0.0f, 1.0f, 0.0f));
            this._Camera.OrbitTarget = center;
            this._Camera.OrbitOffsetDistance = distance * 1.55f;
            this._Camera.PreferTargetYAxisOrbiting = true;
            this._Camera.UndoRoll();
        }

        private void CreateDevice()
        {
            var sd = new DXGI.SwapChainDescription
            {
                BufferCount = 1,
                Flags = DXGI.SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new DXGI.ModeDescription(this.ClientSize.Width,
                                                           this.ClientSize.Height,
                                                           new Rational(60, 1),
                                                           DXGI.Format.R8G8B8A8_UNorm),
                OutputHandle = this.Handle,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                SwapEffect = DXGI.SwapEffect.Discard,
                Usage = DXGI.Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(
                null,
                DriverType.Hardware,
                DeviceCreationFlags.Debug,
                sd,
                out this._Device,
                out this._SwapChain);

            this._RenderEventArgs.Device = this._Device;
            this._RenderEventArgs.SwapChain = this._SwapChain;

            this.ResizeBackbuffer();
        }

        private void DestroyTargets()
        {
            if (this._RenderTargetView != null)
            {
                this._RenderTargetView.Dispose();
            }

            if (this._DepthStencilView != null)
            {
                this._DepthStencilView.Dispose();
            }

            if (this._DepthStencilBuffer != null)
            {
                this._DepthStencilBuffer.Dispose();
            }
        }

        private void DestroyDevice()
        {
            if (this.Uninitialize != null)
            {
                this.Uninitialize(this, null);
            }

            this.DestroyTargets();

            if (this._SwapChain != null)
            {
                this._SwapChain.Dispose();
            }

            if (this._Device != null)
            {
                this._Device.Dispose();
            }
        }

        private void ResizeBackbuffer()
        {
            this.DestroyTargets();

            this._SwapChain.ResizeBuffers(
                1,
                this.Width,
                this.Height,
                DXGI.Format.R8G8B8A8_UNorm,
                DXGI.SwapChainFlags.None);

            using (var swapChainBuffer = Resource.FromSwapChain<Texture2D>(this._SwapChain, 0))
            {
                this._RenderTargetView = new RenderTargetView(this._Device, swapChainBuffer);
            }

            var depthStencilDesc = new Texture2DDescription();
            depthStencilDesc.Width = this.Width;
            depthStencilDesc.Height = this.Height;
            depthStencilDesc.MipLevels = 1;
            depthStencilDesc.ArraySize = 1;
            depthStencilDesc.Format = DXGI.Format.D24_UNorm_S8_UInt;
            depthStencilDesc.SampleDescription = new DXGI.SampleDescription(1, 0);
            depthStencilDesc.Usage = ResourceUsage.Default;
            depthStencilDesc.BindFlags = BindFlags.DepthStencil;
            depthStencilDesc.CpuAccessFlags = 0;
            depthStencilDesc.OptionFlags = ResourceOptionFlags.None;

            this._DepthStencilBuffer = new Texture2D(this._Device, depthStencilDesc);
            this._DepthStencilView = new DepthStencilView(this._Device,
                                                          this._DepthStencilBuffer);
            this._Device.OutputMerger.SetTargets(this._DepthStencilView,
                                                 this._RenderTargetView);

            this._RenderEventArgs.RenderTargetView = this._RenderTargetView;
            this._RenderEventArgs.DepthStencilBuffer = this._DepthStencilBuffer;
            this._RenderEventArgs.DepthStencilView = this._DepthStencilView;

            var vp = new Viewport(0,
                                  0,
                                  this.Width,
                                  this.Height,
                                  0.0f,
                                  1.0f);

            this._Device.Rasterizer.SetViewports(vp);

            if (this.Initialize != null)
            {
                this.Initialize(this, this._RenderEventArgs);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible == true)
            {
                this.CreateDevice();
            }
            else
            {
                this.DestroyDevice();
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (this._Device != null &&
                this.Width > 0 &&
                this.Height > 0)
            {
                this.ResizeBackbuffer();
                this.Invalidate();
            }

            this._Camera.Resized();
        }

        internal void DoUpdateScene()
        {
            float elapsedTime = this._Clock.Update();
            this._Camera.Update(elapsedTime, false);

            if (this.UpdateScene != null)
            {
                this._UpdateSceneEventArgs.ElapsedTime = elapsedTime;
                this.UpdateScene(this, this._UpdateSceneEventArgs);
            }
        }

        internal void DoRender()
        {
            if (this.DesignMode == true || this._Device == null)
            {
                return;
            }

            this._Device.ClearRenderTargetView(this._RenderTargetView,
                                               this.ClearColor);
            this._Device.ClearDepthStencilView(this._DepthStencilView,
                                               DepthStencilClearFlags.Depth |
                                               DepthStencilClearFlags.Stencil,
                                               1.0f,
                                               0);

            if (this.Render != null)
            {
                this._RenderEventArgs.ViewProjectionMatrix = this._Camera.ViewProjectionMatrix;
                this.Render(this, this._RenderEventArgs);
            }

            this._SwapChain.Present(0, DXGI.PresentFlags.None);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode == true)
            {
                base.OnPaint(e);
                e.Graphics.Clear(this.ClearColor);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(this.ClearColor);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        public void UpdateFocus()
        {
            if (Native.IsWindowActive(this.Parent.Handle) == true)
            {
                if (this.CaptureMouse == true)
                {
                    var position = this.PointToScreen(new Point(this.Width / 2, this.Height / 2));
                    Cursor.Position = position;
                    this.Cursor = this._InvisibleCursor;
                }
            }
            else
            {
                if (this.CaptureMouse == true)
                {
                    this.CaptureMouse = false;
                    this.Cursor = this._DefaultCursor;
                    Cursor.Position = this._CapturedMousePosition;
                }
            }
        }
    }

    public delegate void UpdateSceneEventHandler(object sender, UpdateSceneEventArgs e);

    public class UpdateSceneEventArgs : EventArgs
    {
        public float ElapsedTime;
    }

    public delegate void RenderEventHandler(object sender, RenderEventArgs e);

    public class RenderEventArgs : EventArgs
    {
        public Device Device;
        public DXGI.SwapChain SwapChain;
        public Texture2D DepthStencilBuffer;
        public RenderTargetView RenderTargetView;
        public DepthStencilView DepthStencilView;
        public Matrix ViewProjectionMatrix;
    }
}
