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
using System.IO;
using System.Windows.Forms;
using Gibbed.Avalanche.RenderBlockModel;
using SlimDX;
using SlimDX.Direct3D10;
using Registry = Microsoft.Win32.Registry;
using ShaderLibrary = Gibbed.Avalanche.FileFormats.ShaderLibraryFile;

namespace Gibbed.Avalanche.ModelViewer2
{
    public partial class Viewer : Form
    {
        private readonly Dictionary<IRenderBlock, Renderers.IRenderer>
            _BlockRenderers = new Dictionary<IRenderBlock, Renderers.IRenderer>();

        private ShaderLibrary _ShaderBundle;
        private ShaderLibrary _SpecialShaderBundle;

        public Viewer()
        {
            this.InitializeComponent();
            this._BlockRenderers.Clear();
            this.LoadShaderBundles();
        }

        private void LoadShaderBundles()
        {
            try
            {
                string gamePath =
                    (string)
                    Registry.GetValue(
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

                using (var input = File.OpenRead(shaderBundlePath))
                {
                    this._ShaderBundle = new ShaderLibrary();
                    this._ShaderBundle.Deserialize(input);
                }

                using (var input = File.OpenRead(specialShaderBundlePath))
                {
                    this._SpecialShaderBundle = new ShaderLibrary();
                    this._SpecialShaderBundle.Deserialize(input);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Failed to load shader bundles.\n\nError: {0}\n\n{1}",
                                              e.Message,
                                              e),
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        ~Viewer()
        {
            this.DisposeX();
        }

        public void DisposeX()
        {
            foreach (var kvp in this._BlockRenderers)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Dispose();
                }
            }
            this._BlockRenderers.Clear();
        }

        private ModelFile _Model = new ModelFile();
        private string _ModelPath;
        private readonly List<IRenderBlock> _SelectedBlocks = new List<IRenderBlock>();

        private void OnOpenModelFile(object sender, EventArgs e)
        {
            if (this.modelOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this._ModelPath = Path.GetDirectoryName(this.modelOpenFileDialog.FileName);

            using (var input = File.OpenRead(this.modelOpenFileDialog.FileName))
            {
                this._Model = new ModelFile();
                this._Model.Deserialize(input);
            }

            this.blockListView.Items.Clear();

            int i = 0;
            foreach (IRenderBlock block in this._Model.Blocks)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString(System.Globalization.CultureInfo.InvariantCulture) + ": " + block.GetType().Name;
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

            this._ModelPath = Path.GetDirectoryName(this.modelOpenFileDialog.FileName);

            var model = new ModelFile();
            using (var input = File.OpenRead(this.modelOpenFileDialog.FileName))
            {
                model.Deserialize(input);
            }

            int i = this._Model.Blocks.Count;
            foreach (var block in model.Blocks)
            {
                this._Model.Blocks.Add(block);

                ListViewItem item = new ListViewItem();
                item.Text = i.ToString(System.Globalization.CultureInfo.InvariantCulture) + ": " + block.GetType().Name;
                item.Tag = block;
                this.blockListView.Items.Add(item);
                i++;
            }

            this.SetupCamera();
        }

        private void SetupCamera()
        {
            var min = new Vector3(this._Model.MinX,
                                  this._Model.MinY,
                                  this._Model.MinZ);

            var max = new Vector3(this._Model.MaxX,
                                  this._Model.MaxY,
                                  this._Model.MaxZ);

            var center = Vector3.Add(min, max);
            center = Vector3.Divide(center, 2.0f);

            var distance = Vector3.Distance(center, max);

            var position = center;
            position.X -= distance / 1.75f;
            position.Y += distance / 1.75f;
            position.Z -= distance / 0.75f;

            this.Viewport.SetupCamera(position, center, distance);
        }

        private void OnItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked == true)
            {
                this._SelectedBlocks.Add((IRenderBlock)e.Item.Tag);
            }
            else
            {
                this._SelectedBlocks.Remove((IRenderBlock)e.Item.Tag);
            }
        }

        private void OnItemSelected(object sender, EventArgs e)
        {
            if (this.blockListView.SelectedItems.Count != 1)
            {
                return;
            }

            var item = this.blockListView.SelectedItems[0];
            var block = item.Tag as IRenderBlock;

            if (block == null)
            {
                return;
            }

            /*if (this.Model.DebugInfos.ContainsKey(block) == false)
            {
                return;
            }

            var debugInfo = this.Model.DebugInfos[block];*/

            this.blockStatusLabel.Text = string.Format(
                "{0} @ offset {1} (0x{1:X}), size {2}",
                item.Text,
                0,
                /*debugInfo.Offset,*/
                0 /*debugInfo.Size*/);
        }

        private void SetCameraBehavior(BasicCamera.Behavior behavior)
        {
            this.Viewport.SetCameraBehavior(behavior);
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

        private void OnBlockCopy(object sender, EventArgs e)
        {
        }

        private void OnBlockDelete(object sender, EventArgs e)
        {
            var selectedItems = this.blockListView.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                var block = item.Tag as IRenderBlock;
                if (block == null)
                {
                    continue;
                }
                this._Model.Blocks.Remove(block);
                this.blockListView.Items.Remove(item);
            }
        }

        #region Rendering
        private void OnViewportInitialized(object sender, RenderEventArgs e)
        {
            var rsd = new RasterizerStateDescription();
            rsd.FillMode = FillMode.Solid;
            rsd.CullMode = CullMode.Back;
            rsd.IsDepthClipEnabled = true;
            rsd.IsMultisampleEnabled = true;

            e.Device.Rasterizer.State = RasterizerState.FromDescription(
                e.Device, rsd);
        }

        private void OnViewportUpdateScene(object sender, UpdateSceneEventArgs e)
        {
        }

        private void OnViewportRender(object sender, RenderEventArgs e)
        {
            foreach (IRenderBlock block in this._Model.Blocks)
            {
                var oldState = e.Device.Rasterizer.State.Description;
                var state = e.Device.Rasterizer.State.Description;
                state.FillMode = _SelectedBlocks.Contains(block) == true ?
                    FillMode.Wireframe : FillMode.Solid;
                state.CullMode = CullMode.None;
                e.Device.Rasterizer.State = RasterizerState.FromDescription(e.Device, state);

                if (this._BlockRenderers.ContainsKey(block) == false)
                {
                    var renderer = RendererTypes.Instantiate(block);
                    if (renderer == null)
                    {
                        continue;
                    }

                    renderer.Setup(e.Device,
                                   block,
                                   this._ShaderBundle,
                                   this._ModelPath);
                    this._BlockRenderers.Add(block, renderer);
                    this._BlockRenderers[block].Render(e.Device, e.ViewProjectionMatrix);
                }
                else
                {
                    this._BlockRenderers[block].Render(e.Device, e.ViewProjectionMatrix);
                }

                e.Device.Rasterizer.State = RasterizerState.FromDescription(e.Device, oldState);
            }
        }

        private void OnViewportUnitialize(object sender, EventArgs e)
        {
        }
        #endregion

        private void OnActivated(object sender, EventArgs e)
        {
            this.Viewport.UpdateFocus();
        }

        private void OnDeactivate(object sender, EventArgs e)
        {
            this.Viewport.UpdateFocus();
        }
    }
}
