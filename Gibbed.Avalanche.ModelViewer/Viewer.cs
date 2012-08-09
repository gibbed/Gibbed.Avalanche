using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gibbed.Avalanche.RenderBlockModel;
using Gibbed.Avalanche.RenderBlockModel.Blocks;
using GameTime = Microsoft.Xna.Framework.GameTime;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace Gibbed.Avalanche.ModelViewer
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            this.InitializeComponent();
            this.Renderer = new ModelViewRenderer(this.viewportPanel);
            Mouse.WindowHandle = this.viewportPanel.Handle;
        }

        private void OnShown(object sender, EventArgs e)
        {
            this.RenderViewport();
        }

        private bool HandlingViewportInput = false;

        private ModelViewRenderer Renderer;
        private ModelFile Model = new ModelFile();
        private string ModelPath;
        private List<IRenderBlock> SelectedBlocks =
            new List<IRenderBlock>();

        private void RenderViewport()
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            var lastUpdate = timer.Elapsed;

            this.Renderer.CreateDevice();
            while (!this.IsDisposed)
            {
                var total = timer.Elapsed;
                var elapsed = total - lastUpdate;
                var gameTime = new GameTime(total, elapsed, total, elapsed);
                lastUpdate = total;

                this.Renderer.UpdateCamera(gameTime, this.HandlingViewportInput);
                Application.DoEvents();
                this.Renderer.UpdateScene(this.ModelPath, this.Model, this.SelectedBlocks);
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }
        }

        private void OnViewportResize(object sender, EventArgs e)
        {
            if (this.Renderer != null)
            {
                this.Renderer.ResetDevice();
            }
        }

        private void OnOpenModelFile(object sender, EventArgs e)
        {
            if (this.modelOpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.ModelPath = Path.GetDirectoryName(this.modelOpenFileDialog.FileName);

            Stream input = File.Open(this.modelOpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.Model = new ModelFile();
            this.Model.Deserialize(input);
            input.Close();

            this.blockListView.Items.Clear();

            int i = 0;
            foreach (IRenderBlock block in this.Model.Blocks)
            {
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString() + ": " + block.GetType().Name;
                item.Tag = block;
                this.blockListView.Items.Add(item);
                i++;
            }

            this.Renderer.ResetDevice();
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
            var model = new ModelFile();
            model.Deserialize(input);
            input.Close();

            int i = this.Model.Blocks.Count;
            foreach (IRenderBlock block in model.Blocks)
            {
                this.Model.Blocks.Add(block);

                ListViewItem item = new ListViewItem();
                item.Text = i.ToString() + ": " + block.GetType().Name;
                item.Tag = block;
                this.blockListView.Items.Add(item);
                i++;
            }

            this.Renderer.ResetDevice();
            this.SetupCamera();
        }

        private void SetupCamera()
        {
            var min = new Microsoft.Xna.Framework.Vector3(
                this.Model.MinX,
                this.Model.MinY,
                this.Model.MinZ);

            var max = new Microsoft.Xna.Framework.Vector3(
                this.Model.MaxX,
                this.Model.MaxY,
                this.Model.MaxZ);

            var center = Microsoft.Xna.Framework.Vector3.Add(min, max);
            center = Microsoft.Xna.Framework.Vector3.Divide(center, 2.0f);

            var distance = Microsoft.Xna.Framework.Vector3.Distance(center, max);

            var position = center;
            position.X -= distance / 2.0f;
            position.Y += distance / 2.0f;
            position.Z -= distance / 1.0f;

            this.Renderer.Camera.Stop();
            this.Renderer.Camera.LookAt(position, center, Microsoft.Xna.Framework.Vector3.Up);
            this.Renderer.Camera.OrbitTarget = center;
            this.Renderer.Camera.OrbitOffsetDistance = distance * 1.25f;
            this.Renderer.Camera.UndoRoll();
        }

        private void OnItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked == true)
            {
                this.SelectedBlocks.Add((IRenderBlock)e.Item.Tag);
            }
            else
            {
                this.SelectedBlocks.Remove((IRenderBlock)e.Item.Tag);
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
                0,/*debugInfo.Offset,*/
                0/*debugInfo.Size*/);
        }

        private void SetCameraBehavior(Camera.Behavior behavior)
        {
            this.Renderer.Camera.CurrentBehavior = behavior;
            this.cameraBehaviorSpectatorButton.Checked = behavior == Camera.Behavior.Spectator;
            this.cameraBehaviorFirstPersonButton.Checked = behavior == Camera.Behavior.FirstPerson;
            this.cameraBehaviorFlightButton.Checked = behavior == Camera.Behavior.Flight;
            this.cameraBehaviorOrbitButton.Checked = behavior == Camera.Behavior.Orbit;
            this.SetupCamera();
        }

        private void OnSetCameraBehaviorSpectator(object sender, EventArgs e)
        {
            this.SetCameraBehavior(Camera.Behavior.Spectator);
        }

        private void OnSetCameraBehaviorFirstPerson(object sender, EventArgs e)
        {
            this.SetCameraBehavior(Camera.Behavior.FirstPerson);
        }

        private void OnSetCameraBehaviorFlight(object sender, EventArgs e)
        {
            this.SetCameraBehavior(Camera.Behavior.Flight);
        }

        private void OnSetCameraBehaviorOrbit(object sender, EventArgs e)
        {
            this.SetCameraBehavior(Camera.Behavior.Orbit);
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
                var block = item.Tag as IRenderBlock;
                if (block == null)
                {
                    continue;
                }
                this.Model.Blocks.Remove(block);
                this.blockListView.Items.Remove(item);
            }
        }
    }
}
