namespace Gibbed.Avalanche.ArchiveViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveAllButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveOnlyknownFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressUnknownFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileList = new System.Windows.Forms.TreeView();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveAllFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.mainToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.saveAllButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(640, 25);
            this.mainToolStrip.TabIndex = 0;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // openButton
            // 
            this.openButton.Image = global::Gibbed.Avalanche.ArchiveViewer.Properties.Resources.OpenArchive;
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(56, 22);
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.OnOpen);
            // 
            // saveAllButton
            // 
            this.saveAllButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveOnlyknownFilesMenuItem,
            this.decompressUnknownFilesMenuItem});
            this.saveAllButton.Image = global::Gibbed.Avalanche.ArchiveViewer.Properties.Resources.SaveAllFiles;
            this.saveAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllButton.Name = "saveAllButton";
            this.saveAllButton.Size = new System.Drawing.Size(80, 22);
            this.saveAllButton.Text = "Save All";
            this.saveAllButton.ButtonClick += new System.EventHandler(this.OnSaveAll);
            // 
            // saveOnlyknownFilesMenuItem
            // 
            this.saveOnlyknownFilesMenuItem.CheckOnClick = true;
            this.saveOnlyknownFilesMenuItem.Name = "saveOnlyknownFilesMenuItem";
            this.saveOnlyknownFilesMenuItem.Size = new System.Drawing.Size(216, 22);
            this.saveOnlyknownFilesMenuItem.Text = "Save only &known files";
            // 
            // decompressUnknownFilesMenuItem
            // 
            this.decompressUnknownFilesMenuItem.Checked = true;
            this.decompressUnknownFilesMenuItem.CheckOnClick = true;
            this.decompressUnknownFilesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.decompressUnknownFilesMenuItem.Name = "decompressUnknownFilesMenuItem";
            this.decompressUnknownFilesMenuItem.Size = new System.Drawing.Size(216, 22);
            this.decompressUnknownFilesMenuItem.Text = "&Decompress unknown files";
            // 
            // fileList
            // 
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.Location = new System.Drawing.Point(0, 25);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(640, 295);
            this.fileList.TabIndex = 1;
            // 
            // openDialog
            // 
            this.openDialog.Filter = "Just Cause 2 Archive Tables (*.tab)|*.tab|All Files (*.*)|*.*";
            // 
            // saveAllFolderDialog
            // 
            this.saveAllFolderDialog.Description = "Select a directory to save all files from the archive to.";
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 320);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.mainToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Viewer";
            this.Text = "Gibbed\'s Just Cause 2 Archive Viewer";
            this.Load += new System.EventHandler(this.OnLoad);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.TreeView fileList;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.FolderBrowserDialog saveAllFolderDialog;
        private System.Windows.Forms.ToolStripSplitButton saveAllButton;
        private System.Windows.Forms.ToolStripMenuItem saveOnlyknownFilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompressUnknownFilesMenuItem;
    }
}

