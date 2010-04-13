using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using RbModel = Gibbed.Avalanche.FileFormats.RbModel;
using RenderBlock = Gibbed.Avalanche.FileFormats.RenderBlock;

namespace Gibbed.Avalanche.ModelViewer
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            this.InitializeComponent();
            this.Renderer = new Renderer(this.viewportPanel);
        }

        private void OnShown(object sender, EventArgs e)
        {
            this.RenderViewport();
        }

        private Renderer Renderer;
        private RbModel Model;
        private string ModelPath;
        private List<RenderBlock.IRenderBlock> SelectedBlocks =
            new List<RenderBlock.IRenderBlock>();

        private void RenderViewport()
        {
            this.Renderer.CreateDevice();
            while (!this.IsDisposed)
            {
                this.Renderer.UpdateCamera(this.viewportPanel.Focused);
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
            if (this.openModelFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.ModelPath = Path.GetDirectoryName(this.openModelFileDialog.FileName);

            Stream input = File.Open(this.openModelFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
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

            this.Renderer.ResetDevice();
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
    }
}
