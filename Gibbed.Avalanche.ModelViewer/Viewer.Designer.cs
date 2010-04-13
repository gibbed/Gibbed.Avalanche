namespace Gibbed.Avalanche.ModelViewer
{
    partial class Viewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.blockListView = new System.Windows.Forms.ListView();
            this.viewportPanel = new Gibbed.Avalanche.ModelViewer.RenderPanel();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.openModelFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.blockListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.viewportPanel);
            this.splitContainer1.Size = new System.Drawing.Size(640, 455);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 0;
            // 
            // blockListView
            // 
            this.blockListView.CheckBoxes = true;
            this.blockListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockListView.Location = new System.Drawing.Point(0, 0);
            this.blockListView.Name = "blockListView";
            this.blockListView.Size = new System.Drawing.Size(213, 455);
            this.blockListView.TabIndex = 0;
            this.blockListView.UseCompatibleStateImageBehavior = false;
            this.blockListView.View = System.Windows.Forms.View.List;
            this.blockListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnItemChecked);
            // 
            // viewportPanel
            // 
            this.viewportPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewportPanel.Location = new System.Drawing.Point(0, 0);
            this.viewportPanel.Name = "viewportPanel";
            this.viewportPanel.Size = new System.Drawing.Size(423, 455);
            this.viewportPanel.TabIndex = 0;
            this.viewportPanel.Resize += new System.EventHandler(this.OnViewportResize);
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(640, 25);
            this.mainToolStrip.TabIndex = 1;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.Image = global::Gibbed.Avalanche.ModelViewer.Properties.Resources.OpenModel;
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(23, 22);
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.OnOpenModelFile);
            // 
            // openModelFileDialog
            // 
            this.openModelFileDialog.DefaultExt = "rbm";
            this.openModelFileDialog.Filter = "Render Block Model Files (*.rbm, *.rbx360, *.rb3)|*.rbm;*.rbx360;*.rb3|All Files " +
                "(*.*)|*.*";
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainToolStrip);
            this.DoubleBuffered = true;
            this.Name = "Viewer";
            this.Text = "Gibbed\'s Avalanche Model Viewer";
            this.Shown += new System.EventHandler(this.OnShown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView blockListView;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.OpenFileDialog openModelFileDialog;
        private RenderPanel viewportPanel;
    }
}

