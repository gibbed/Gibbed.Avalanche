namespace Gibbed.Avalanche.ModelViewer2
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.blockListView = new System.Windows.Forms.ListView();
            this.blockListToolStrip = new System.Windows.Forms.ToolStrip();
            this.blockCopyButton = new System.Windows.Forms.ToolStripButton();
            this.blockDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.renderPanel = new Gibbed.Avalanche.ModelViewer2.RenderPanel();
            this.viewportToolStrip = new System.Windows.Forms.ToolStrip();
            this.cameraModeButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.cameraBehaviorSpectatorButton = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraBehaviorFirstPersonButton = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraBehaviorFlightButton = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraBehaviorOrbitButton = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraResetButton = new System.Windows.Forms.ToolStripButton();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.modelOpenFileButton = new System.Windows.Forms.ToolStripSplitButton();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.blockStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.blockListToolStrip.SuspendLayout();
            this.viewportToolStrip.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 25);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.blockListView);
            this.splitContainer.Panel1.Controls.Add(this.blockListToolStrip);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.renderPanel);
            this.splitContainer.Panel2.Controls.Add(this.viewportToolStrip);
            this.splitContainer.Size = new System.Drawing.Size(800, 433);
            this.splitContainer.SplitterDistance = 240;
            this.splitContainer.TabIndex = 0;
            // 
            // blockListView
            // 
            this.blockListView.CheckBoxes = true;
            this.blockListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockListView.Location = new System.Drawing.Point(0, 0);
            this.blockListView.Name = "blockListView";
            this.blockListView.Size = new System.Drawing.Size(240, 408);
            this.blockListView.TabIndex = 0;
            this.blockListView.UseCompatibleStateImageBehavior = false;
            this.blockListView.View = System.Windows.Forms.View.List;
            this.blockListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnItemChecked);
            this.blockListView.SelectedIndexChanged += new System.EventHandler(this.OnItemSelected);
            // 
            // blockListToolStrip
            // 
            this.blockListToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.blockListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blockCopyButton,
            this.blockDeleteButton});
            this.blockListToolStrip.Location = new System.Drawing.Point(0, 408);
            this.blockListToolStrip.Name = "blockListToolStrip";
            this.blockListToolStrip.Size = new System.Drawing.Size(240, 25);
            this.blockListToolStrip.TabIndex = 1;
            this.blockListToolStrip.Text = "toolStrip1";
            // 
            // blockCopyButton
            // 
            this.blockCopyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.blockCopyButton.Image = global::Gibbed.Avalanche.ModelViewer2.Properties.Resources.BlockCopy;
            this.blockCopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.blockCopyButton.Name = "blockCopyButton";
            this.blockCopyButton.Size = new System.Drawing.Size(23, 22);
            this.blockCopyButton.Text = "Duplicate Block";
            this.blockCopyButton.Click += new System.EventHandler(this.OnBlockCopy);
            // 
            // blockDeleteButton
            // 
            this.blockDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.blockDeleteButton.Image = global::Gibbed.Avalanche.ModelViewer2.Properties.Resources.BlockDelete;
            this.blockDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.blockDeleteButton.Name = "blockDeleteButton";
            this.blockDeleteButton.Size = new System.Drawing.Size(23, 22);
            this.blockDeleteButton.Text = "Delete Block";
            this.blockDeleteButton.Click += new System.EventHandler(this.OnBlockDelete);
            // 
            // renderPanel
            // 
            this.renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel.Location = new System.Drawing.Point(0, 0);
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.Size = new System.Drawing.Size(556, 408);
            this.renderPanel.TabIndex = 2;
            this.renderPanel.Text = "renderPanel1";
            this.renderPanel.UpdateScene += new Gibbed.Avalanche.ModelViewer2.RenderEventHandler(this.OnViewportUpdateScene);
            this.renderPanel.Uninitialize += new System.EventHandler(this.OnViewportUnitialize);
            this.renderPanel.Initialize += new Gibbed.Avalanche.ModelViewer2.RenderEventHandler(this.OnViewportInitialized);
            this.renderPanel.Render += new Gibbed.Avalanche.ModelViewer2.RenderEventHandler(this.OnViewportRender);
            // 
            // viewportToolStrip
            // 
            this.viewportToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.viewportToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraModeButton,
            this.cameraResetButton});
            this.viewportToolStrip.Location = new System.Drawing.Point(0, 408);
            this.viewportToolStrip.Name = "viewportToolStrip";
            this.viewportToolStrip.Size = new System.Drawing.Size(556, 25);
            this.viewportToolStrip.TabIndex = 1;
            this.viewportToolStrip.Text = "toolStrip2";
            // 
            // cameraModeButton
            // 
            this.cameraModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cameraModeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraBehaviorSpectatorButton,
            this.cameraBehaviorFirstPersonButton,
            this.cameraBehaviorFlightButton,
            this.cameraBehaviorOrbitButton});
            this.cameraModeButton.Image = global::Gibbed.Avalanche.ModelViewer2.Properties.Resources.CameraMode;
            this.cameraModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cameraModeButton.Name = "cameraModeButton";
            this.cameraModeButton.Size = new System.Drawing.Size(29, 22);
            this.cameraModeButton.Text = "Camera Mode";
            // 
            // cameraBehaviorSpectatorButton
            // 
            this.cameraBehaviorSpectatorButton.Checked = true;
            this.cameraBehaviorSpectatorButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cameraBehaviorSpectatorButton.Name = "cameraBehaviorSpectatorButton";
            this.cameraBehaviorSpectatorButton.Size = new System.Drawing.Size(135, 22);
            this.cameraBehaviorSpectatorButton.Text = "Spectator";
            this.cameraBehaviorSpectatorButton.Click += new System.EventHandler(this.OnSetCameraBehaviorSpectator);
            // 
            // cameraBehaviorFirstPersonButton
            // 
            this.cameraBehaviorFirstPersonButton.Name = "cameraBehaviorFirstPersonButton";
            this.cameraBehaviorFirstPersonButton.Size = new System.Drawing.Size(135, 22);
            this.cameraBehaviorFirstPersonButton.Text = "First Person";
            this.cameraBehaviorFirstPersonButton.Click += new System.EventHandler(this.OnSetCameraBehaviorFirstPerson);
            // 
            // cameraBehaviorFlightButton
            // 
            this.cameraBehaviorFlightButton.Name = "cameraBehaviorFlightButton";
            this.cameraBehaviorFlightButton.Size = new System.Drawing.Size(135, 22);
            this.cameraBehaviorFlightButton.Text = "Flight";
            this.cameraBehaviorFlightButton.Click += new System.EventHandler(this.OnSetCameraBehaviorFlight);
            // 
            // cameraBehaviorOrbitButton
            // 
            this.cameraBehaviorOrbitButton.Name = "cameraBehaviorOrbitButton";
            this.cameraBehaviorOrbitButton.Size = new System.Drawing.Size(135, 22);
            this.cameraBehaviorOrbitButton.Text = "Orbit";
            this.cameraBehaviorOrbitButton.Click += new System.EventHandler(this.OnSetCameraBehaviorOrbit);
            // 
            // cameraResetButton
            // 
            this.cameraResetButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cameraResetButton.Image = global::Gibbed.Avalanche.ModelViewer2.Properties.Resources.CameraReset;
            this.cameraResetButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cameraResetButton.Name = "cameraResetButton";
            this.cameraResetButton.Size = new System.Drawing.Size(23, 22);
            this.cameraResetButton.Text = "Reset Camera";
            this.cameraResetButton.Click += new System.EventHandler(this.OnResetCameraView);
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modelOpenFileButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(800, 25);
            this.mainToolStrip.TabIndex = 1;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // modelOpenFileButton
            // 
            this.modelOpenFileButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.modelOpenFileButton.Image = global::Gibbed.Avalanche.ModelViewer2.Properties.Resources.OpenModel;
            this.modelOpenFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.modelOpenFileButton.Name = "modelOpenFileButton";
            this.modelOpenFileButton.Size = new System.Drawing.Size(68, 22);
            this.modelOpenFileButton.Text = "Open";
            this.modelOpenFileButton.ButtonClick += new System.EventHandler(this.OnOpenModelFile);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.OnAddModelFile);
            // 
            // modelOpenFileDialog
            // 
            this.modelOpenFileDialog.DefaultExt = "rbm";
            this.modelOpenFileDialog.Filter = "Render Block Model Files (*.rbm, *.rbx360, *.rb3)|*.rbm;*.rbx360;*.rb3|All Files " +
                "(*.*)|*.*";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blockStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 458);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // blockStatusLabel
            // 
            this.blockStatusLabel.Name = "blockStatusLabel";
            this.blockStatusLabel.Size = new System.Drawing.Size(79, 17);
            this.blockStatusLabel.Text = "Select a block";
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.mainToolStrip);
            this.Controls.Add(this.statusStrip);
            this.DoubleBuffered = true;
            this.Name = "Viewer";
            this.Text = "Gibbed\'s Avalanche Model Viewer";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.blockListToolStrip.ResumeLayout(false);
            this.blockListToolStrip.PerformLayout();
            this.viewportToolStrip.ResumeLayout(false);
            this.viewportToolStrip.PerformLayout();
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView blockListView;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.OpenFileDialog modelOpenFileDialog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripSplitButton modelOpenFileButton;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStrip blockListToolStrip;
        private System.Windows.Forms.ToolStripButton blockCopyButton;
        private System.Windows.Forms.ToolStripButton blockDeleteButton;
        private System.Windows.Forms.ToolStrip viewportToolStrip;
        private System.Windows.Forms.ToolStripDropDownButton cameraModeButton;
        private System.Windows.Forms.ToolStripMenuItem cameraBehaviorSpectatorButton;
        private System.Windows.Forms.ToolStripMenuItem cameraBehaviorFirstPersonButton;
        private System.Windows.Forms.ToolStripMenuItem cameraBehaviorFlightButton;
        private System.Windows.Forms.ToolStripMenuItem cameraBehaviorOrbitButton;
        private System.Windows.Forms.ToolStripButton cameraResetButton;
        private System.Windows.Forms.ToolStripStatusLabel blockStatusLabel;
        private RenderPanel renderPanel;
    }
}

