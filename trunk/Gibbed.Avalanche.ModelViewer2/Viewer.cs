using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using RbModel = Gibbed.Avalanche.FileFormats.RbModel;
using RenderBlock = Gibbed.Avalanche.FileFormats.RenderBlock;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;
using Registry = Microsoft.Win32.Registry;

using SlimDX;
using D3D10 = SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;

namespace Gibbed.Avalanche.ModelViewer2
{
    public partial class Viewer : Form, IDisposable
    {
        #region Render Types
        private static class RendererTypes
        {
            private static Dictionary<Type, Type> Types;

            static RendererTypes()
            {
                Types = new Dictionary<Type, Type>();
                AddRendererType<RenderBlock.CarPaint, Renderers.CarPaintRenderer>();
                AddRendererType<RenderBlock.CarPaintSimple, Renderers.CarPaintSimpleRenderer>();
                AddRendererType<RenderBlock.DeformableWindow, Renderers.DeformableWindowRenderer>();
                AddRendererType<RenderBlock.General, Renderers.GeneralRenderer>();
                AddRendererType<RenderBlock.SkinnedGeneral, Renderers.SkinnedGeneralRenderer>();
            }

            private static void AddRendererType<TRenderBlock, TBlockRenderer>()
                where TRenderBlock : RenderBlock.IRenderBlock
                where TBlockRenderer : Renderers.IRenderer, new()
            {
                Types.Add(typeof(TRenderBlock), typeof(TBlockRenderer));
            }

            public static Renderers.IRenderer Instantiate(RenderBlock.IRenderBlock block)
            {
                Type type = block.GetType();

                if (Types.ContainsKey(type) == false)
                {
                    return null;
                }

                return (Renderers.IRenderer)Activator.CreateInstance(Types[type]);
            }
        }
        #endregion

        private InputCamera Camera;
        private Clock Clock;

        private Dictionary<RenderBlock.IRenderBlock, Renderers.IRenderer>
            BlockRenderers = new Dictionary<RenderBlock.IRenderBlock, Renderers.IRenderer>();

        private ShaderLibrary ShaderBundle;
        private ShaderLibrary SpecialShaderBundle;

        public Viewer()
        {
            this.InitializeComponent();

            this.Clock = new Clock();
            this.Clock.Start();

            this.Camera = new InputCamera(this.renderPanel);

            this.BlockRenderers.Clear();

            this.LoadShaderBundles();
        }

        private void LoadShaderBundles()
        {
            try
            {
                string gamePath = (string)Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 8190",
                        "InstallLocation",
                        null);

                if (gamePath == null)
                {
                    throw new InvalidOperationException("could not find Just Cause 2 install location");
                }

                string shaderBundlePath = Path.Combine(gamePath, "DX10_Shaders_F.shader_bundle");
                if (File.Exists(shaderBundlePath) == false)
                {
                    throw new FileNotFoundException("shader bundle is missing", shaderBundlePath);
                }

                string specialShaderBundlePath = Path.Combine(gamePath, "DX10_SpecialShaders_F.shader_bundle");
                if (File.Exists(specialShaderBundlePath) == false)
                {
                    throw new FileNotFoundException("special shader bundle is missing", specialShaderBundlePath);
                }

                using (Stream input = File.OpenRead(shaderBundlePath))
                {
                    this.ShaderBundle = new ShaderLibrary();
                    this.ShaderBundle.Deserialize(input);
                }

                using (Stream input = File.OpenRead(specialShaderBundlePath))
                {
                    this.SpecialShaderBundle = new ShaderLibrary();
                    this.SpecialShaderBundle.Deserialize(input);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format(
                        "Failed to load shader bundles.\n\nError: {0}\n\n{1}",
                        e.Message,
                        e.ToString()),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        ~Viewer()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            foreach (var kvp in this.BlockRenderers)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Dispose();
                }
            }
            this.BlockRenderers.Clear();
        }        

        private bool HandlingViewportInput = false;

        private RbModel Model = new RbModel();
        private string ModelPath;
        private List<RenderBlock.IRenderBlock> SelectedBlocks =
            new List<RenderBlock.IRenderBlock>();

        private void RenderViewport()
        {
            var clock = new Clock();
            clock.Start();

            /*
            while (!this.IsDisposed)
            {
                this.Renderer.UpdateCamera(clock.Update(), this.HandlingViewportInput);
                Application.DoEvents();
                this.Renderer.UpdateScene(this.ModelPath, this.Model, this.SelectedBlocks);
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }
            */
        }

        private void OnOpenModelFile(object sender, EventArgs e)
        {
            if (this.modelOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.ModelPath = Path.GetDirectoryName(this.modelOpenFileDialog.FileName);

            Stream input = File.Open(this.modelOpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.Model = new RbModel();
            this.Model.Deserialize(input);
            input.Close();

            this.blockListView.Items.Clear();

            int i = 0;
            foreach (RenderBlock.IRenderBlock block in this.Model.Blocks)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString() + ": " + block.GetType().Name;
                item.Tag = block;
                this.blockListView.Items.Add(item);
                i++;
            }

            this.SetupCamera();
        }

        private void OnAddModelFile(object sender, EventArgs e)
        {
            if (this.modelOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.ModelPath = Path.GetDirectoryName(this.modelOpenFileDialog.FileName);

            Stream input = File.Open(this.modelOpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var model = new RbModel();
            model.Deserialize(input);
            input.Close();

            int i = this.Model.Blocks.Count;
            foreach (RenderBlock.IRenderBlock block in model.Blocks)
            {
                this.Model.Blocks.Add(block);

                ListViewItem item = new ListViewItem();
                item.Text = i.ToString() + ": " + block.GetType().Name;
                item.Tag = block;
                this.blockListView.Items.Add(item);
                i++;
            }

            this.SetupCamera();
        }

        private void SetupCamera()
        {
            var min = new SlimDX.Vector3(
                this.Model.MinX,
                this.Model.MinY,
                this.Model.MinZ);

            var max = new SlimDX.Vector3(
                this.Model.MaxX,
                this.Model.MaxY,
                this.Model.MaxZ);

            var center = SlimDX.Vector3.Add(min, max);
            center = SlimDX.Vector3.Divide(center, 2.0f);

            var distance = SlimDX.Vector3.Distance(center, max);

            var position = center;
            position.X -= distance / 1.75f;
            position.Y += distance / 1.75f;
            position.Z -= distance / 0.75f;

            this.Camera.Stop();
            this.Camera.LookAt(position, center, new SlimDX.Vector3(0.0f, 1.0f, 0.0f));
            this.Camera.OrbitTarget = center;
            this.Camera.OrbitOffsetDistance = distance * 1.55f;
            this.Camera.PreferTargetYAxisOrbiting = true;
            this.Camera.UndoRoll();
        }

        private void OnItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked == true)
            {
                this.SelectedBlocks.Add((RenderBlock.IRenderBlock)e.Item.Tag);
            }
            else
            {
                this.SelectedBlocks.Remove((RenderBlock.IRenderBlock)e.Item.Tag);
            }
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            if (this.blockListView.SelectedItems.Count != 1)
            {
                return;
            }

            var item = this.blockListView.SelectedItems[0];
            var block = item.Tag as RenderBlock.IRenderBlock;
            
            if (block == null)
            {
                return;
            }

            if (this.Model.DebugInfos.ContainsKey(block) == false)
            {
                return;
            }

            var debugInfo = this.Model.DebugInfos[block];

            this.blockStatusLabel.Text = string.Format(
                "{0} @ offset {1} (0x{1:X}), size {2}",
                item.Text,
                debugInfo.Offset,
                debugInfo.Size);
        }

        private void SetCameraBehavior(BasicCamera.Behavior behavior)
        {
            this.Camera.CurrentBehavior = behavior;
            this.cameraBehaviorSpectatorButton.Checked = behavior == BasicCamera.Behavior.Spectator;
            this.cameraBehaviorFirstPersonButton.Checked = behavior == BasicCamera.Behavior.FirstPerson;
            this.cameraBehaviorFlightButton.Checked = behavior == BasicCamera.Behavior.Flight;
            this.cameraBehaviorOrbitButton.Checked = behavior == BasicCamera.Behavior.Orbit;
            this.SetupCamera();
        }

        private void OnSetCameraBehaviorSpectator(object sender, EventArgs e)
        {
            this.SetCameraBehavior(BasicCamera.Behavior.Spectator);
        }

        private void OnSetCameraBehaviorFirstPerson(object sender, EventArgs e)
        {
            this.SetCameraBehavior(BasicCamera.Behavior.FirstPerson);
        }

        private void OnSetCameraBehaviorFlight(object sender, EventArgs e)
        {
            this.SetCameraBehavior(BasicCamera.Behavior.Flight);
        }

        private void OnSetCameraBehaviorOrbit(object sender, EventArgs e)
        {
            this.SetCameraBehavior(BasicCamera.Behavior.Orbit);
        }

        private void OnResetCameraView(object sender, EventArgs e)
        {
            this.SetupCamera();
        }

        private void OnViewportMouseEnter(object sender, EventArgs e)
        {
            this.HandlingViewportInput = true;
        }

        private void OnViewportMouseLeave(object sender, EventArgs e)
        {
            this.HandlingViewportInput = false;
        }

        private void OnBlockCopy(object sender, EventArgs e)
        {

        }

        private void OnBlockDelete(object sender, EventArgs e)
        {
            var selectedItems = this.blockListView.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                var block = item.Tag as RenderBlock.IRenderBlock;
                if (block == null)
                {
                    continue;
                }
                this.Model.Blocks.Remove(block);
                this.blockListView.Items.Remove(item);
            }
        }

        #region Rendering

        private D3D10.Effect BasicEffect;

        private void OnViewportInitialized(object sender, RenderEventArgs e)
        {
            var rsd = new D3D10.RasterizerStateDescription();
            rsd.FillMode = D3D10.FillMode.Solid;
            rsd.CullMode = D3D10.CullMode.Back;
            rsd.IsDepthClipEnabled = true;
            rsd.IsMultisampleEnabled = true;

            e.Device.Rasterizer.State = D3D10.RasterizerState.FromDescription(
                e.Device, rsd);

            //this.BasicEffect.GetVariableByName("World").AsMatrix().SetMatrix(Matrix.Identity);
        }

        private void OnViewportUpdateScene(object sender, RenderEventArgs e)
        {
            float elapsedTime = this.Clock.Update();
            this.Camera.Update(elapsedTime, false);
            //this.BasicEffect.GetVariableByName("View").AsMatrix().SetMatrix(this.Camera.ViewMatrix);
        }

        private void OnViewportRender(object sender, RenderEventArgs e)
        {
            foreach (RenderBlock.IRenderBlock block in this.Model.Blocks)
            {
                //var oldMode = this.Device.RenderState.FillMode;
                //this.Device.RenderState.FillMode =
                //    selectedBlocks.Contains(block) == true ?
                //        FillMode.WireFrame : FillMode.Solid;

                if (this.BlockRenderers.ContainsKey(block) == false)
                {
                    var renderer = RendererTypes.Instantiate(block);
                    if (renderer == null)
                    {
                        continue;
                    }

                    renderer.Setup(
                        e.Device, block,
                        this.ShaderBundle,
                        this.ModelPath);
                    this.BlockRenderers.Add(block, renderer);
                    this.BlockRenderers[block].Render(e.Device, this.Camera.ViewProjectionMatrix);
                }
                else
                {
                    this.BlockRenderers[block].Render(e.Device, this.Camera.ViewProjectionMatrix);
                }

                //e.Device.RenderState.FillMode = oldMode;
            }
        }

        private void OnViewportUnitialize(object sender, EventArgs e)
        {
            if (this.BasicEffect != null)
            {
                this.BasicEffect.Dispose();
            }
        }
        #endregion
    }
}
