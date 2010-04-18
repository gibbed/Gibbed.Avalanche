using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using DXGI = SlimDX.DXGI;
using D3D10 = SlimDX.Direct3D10;
using System.Runtime.InteropServices;

namespace Gibbed.Avalanche.ModelViewer2
{
    public class RenderPanel : Control
    {
        [DefaultValue(typeof(Color), "SteelBlue")]
        public Color ClearColor { get; set; }

        public event RenderEventHandler Initialize;
        public event RenderEventHandler UpdateScene;
        public event RenderEventHandler Render;
        public event EventHandler Uninitialize;

        private D3D10.Device Device;
        private DXGI.SwapChain SwapChain;
        private D3D10.Texture2D DepthStencilBuffer;
        private D3D10.RenderTargetView RenderTargetView;
        private D3D10.DepthStencilView DepthStencilView;

        private RenderEventArgs RenderEventArgs;

        public RenderPanel() : base()
        {
            this.ClearColor = Color.SteelBlue;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Selectable, true);

            this.RenderEventArgs = new RenderEventArgs();

            Magic.RegisterPanel(this);
        }

        protected override void Dispose(bool disposing)
        {
            Magic.UnregisterPanel(this);

            this.DestroyDevice();
            base.Dispose(disposing);
        }

        private void CreateDevice()
        {
            var sd = new SlimDX.DXGI.SwapChainDescription
                {
                    BufferCount = 1,
                    Flags = SlimDX.DXGI.SwapChainFlags.None,
                    IsWindowed = true,
                    ModeDescription = new SlimDX.DXGI.ModeDescription(
                        this.ClientSize.Width,
                        this.ClientSize.Height,
                        new SlimDX.Rational(60, 1),
                        SlimDX.DXGI.Format.R8G8B8A8_UNorm),
                    OutputHandle = this.Handle,
                    SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                    SwapEffect = SlimDX.DXGI.SwapEffect.Discard,
                    Usage = SlimDX.DXGI.Usage.RenderTargetOutput
                };

            D3D10.Device.CreateWithSwapChain(
                null,
                D3D10.DriverType.Hardware,
                D3D10.DeviceCreationFlags.None,
                sd,
                out this.Device,
                out this.SwapChain);

            this.RenderEventArgs.Device = this.Device;
            this.RenderEventArgs.SwapChain = this.SwapChain;

            this.ResizeBackbuffer();
        }

        private void DestroyTargets()
        {
            if (this.RenderTargetView != null)
            {
                this.RenderTargetView.Dispose();
            }

            if (this.DepthStencilView != null)
            {
                this.DepthStencilView.Dispose();
            }

            if (this.DepthStencilBuffer != null)
            {
                this.DepthStencilBuffer.Dispose();
            }
        }

        private void DestroyDevice()
        {
            if (this.Uninitialize != null)
            {
                this.Uninitialize(this, null);
            }

            this.DestroyTargets();

            if (this.SwapChain != null)
            {
                this.SwapChain.Dispose();
            }

            if (this.Device != null)
            {
                this.Device.Dispose();
            }
        }

        private void ResizeBackbuffer()
        {
            this.DestroyTargets();

            this.SwapChain.ResizeBuffers(
                1,
                this.Width,
                this.Height,
                DXGI.Format.R8G8B8A8_UNorm,
                DXGI.SwapChainFlags.None);

            using (var swapChainBuffer = D3D10.Resource.FromSwapChain<D3D10.Texture2D>(this.SwapChain, 0))
            {
                this.RenderTargetView = new D3D10.RenderTargetView(this.Device, swapChainBuffer);
            }

            var depthStencilDesc = new D3D10.Texture2DDescription();
            depthStencilDesc.Width = this.Width;
            depthStencilDesc.Height = this.Height;
            depthStencilDesc.MipLevels = 1;
            depthStencilDesc.ArraySize = 1;
            depthStencilDesc.Format = DXGI.Format.D24_UNorm_S8_UInt;
            depthStencilDesc.SampleDescription = new DXGI.SampleDescription(1, 0);
            depthStencilDesc.Usage = D3D10.ResourceUsage.Default;
            depthStencilDesc.BindFlags = D3D10.BindFlags.DepthStencil;
            depthStencilDesc.CpuAccessFlags = 0;
            depthStencilDesc.OptionFlags = D3D10.ResourceOptionFlags.None;

            this.DepthStencilBuffer = new D3D10.Texture2D(
                this.Device, depthStencilDesc);
            this.DepthStencilView = new D3D10.DepthStencilView(
                this.Device,
                this.DepthStencilBuffer);
            this.Device.OutputMerger.SetTargets(
                this.DepthStencilView,
                this.RenderTargetView);

            this.RenderEventArgs.RenderTargetView = this.RenderTargetView;
            this.RenderEventArgs.DepthStencilBuffer = this.DepthStencilBuffer;
            this.RenderEventArgs.DepthStencilView = this.DepthStencilView;

            var vp = new D3D10.Viewport(
                0,
                0,
                this.Width,
                this.Height,
                0.0f,
                1.0f);

            this.Device.Rasterizer.SetViewports(vp);

            if (this.Initialize != null)
            {
                this.Initialize(this, this.RenderEventArgs);
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

            if (this.Device != null &&
                this.Width > 0 &&
                this.Height > 0)
            {
                this.ResizeBackbuffer();
                this.Invalidate();
            }
        }

        protected void DoUpdateScene()
        {
            if (this.UpdateScene != null)
            {
                this.UpdateScene(this, this.RenderEventArgs);
            }
        }

        protected void DoRender()
        {
            if (this.DesignMode == true || this.Device == null)
            {
                return;
            }

            this.Device.ClearRenderTargetView(
                this.RenderTargetView,
                this.ClearColor);
            this.Device.ClearDepthStencilView(
                this.DepthStencilView,
                D3D10.DepthStencilClearFlags.Depth |
                D3D10.DepthStencilClearFlags.Stencil,
                1.0f, 0);

            if (this.Render != null)
            {
                this.Render(this, this.RenderEventArgs);
            }

            this.SwapChain.Present(0, DXGI.PresentFlags.None);
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

        #region Render Loop Magic~
        private static RenderLoopMagic Magic = new RenderLoopMagic();
        private class RenderLoopMagic
        {
            private EventHandler IdleEvent;
            private List<RenderPanel> Panels = new List<RenderPanel>();

            public RenderLoopMagic()
            {
                this.IdleEvent = new EventHandler(OnApplicationIdle);
                Application.Idle += this.IdleEvent;
            }

            ~RenderLoopMagic()
            {
                Application.Idle -= this.IdleEvent;
            }

            public void RegisterPanel(RenderPanel panel)
            {
                if (this.Panels.Contains(panel) == false)
                {
                    this.Panels.Add(panel);
                }
            }

            public void UnregisterPanel(RenderPanel panel)
            {
                this.Panels.Remove(panel);
            }

            protected void OnApplicationIdle(object sender, EventArgs e)
            {
                bool hasRendered;
                MessageWin32 msg;

                while (PeekMessage(out msg, IntPtr.Zero, 0, 0, 0) == false)
                {
                    hasRendered = false;

                    foreach (var panel in this.Panels)
                    {
                        if (panel.IsDisposed == false)
                        {
                            panel.DoUpdateScene();
                            panel.DoRender();
                            hasRendered = true;
                        }
                    }

                    if (hasRendered == false)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }

            #region Win32
            [StructLayout(LayoutKind.Sequential)]
            private struct MessageWin32
            {
                public IntPtr hWnd;
                public int msg;
                public IntPtr wParam;
                public IntPtr lParam;
                public uint time;
                public System.Drawing.Point p;
            }

            [System.Security.SuppressUnmanagedCodeSecurity]
            [DllImport("user32.dll")]
            private static extern bool PeekMessage(out MessageWin32 msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
            #endregion
        }

        #endregion
    }

    public delegate void RenderEventHandler(object sender, RenderEventArgs e);
    public class RenderEventArgs : EventArgs
    {
        public D3D10.Device Device;
        public DXGI.SwapChain SwapChain;
        public D3D10.Texture2D DepthStencilBuffer;
        public D3D10.RenderTargetView RenderTargetView;
        public D3D10.DepthStencilView DepthStencilView;
    }
}
