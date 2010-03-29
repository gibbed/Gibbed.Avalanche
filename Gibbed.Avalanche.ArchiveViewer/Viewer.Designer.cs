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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.projectComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.fileList = new System.Windows.Forms.TreeView();
            this.fileMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFilesDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveAllButton = new System.Windows.Forms.ToolStripButton();
            this.reloadListsButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.saveOnlyknownFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressUnknownFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolStrip.SuspendLayout();
            this.fileMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectComboBox,
            this.openButton,
            this.saveAllButton,
            this.reloadListsButton,
            this.settingsButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(640, 25);
            this.mainToolStrip.TabIndex = 0;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // projectComboBox
            // 
            this.projectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.projectComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.projectComboBox.Name = "projectComboBox";
            this.projectComboBox.Size = new System.Drawing.Size(121, 25);
            this.projectComboBox.Sorted = true;
            this.projectComboBox.SelectedIndexChanged += new System.EventHandler(this.OnProjectSelected);
            // 
            // fileList
            // 
            this.fileList.ContextMenuStrip = this.fileMenuStrip;
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.Location = new System.Drawing.Point(0, 25);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(640, 295);
            this.fileList.TabIndex = 1;
            // 
            // fileMenuStrip
            // 
            this.fileMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileMenuStrip.Name = "fileMenuStrip";
            this.fileMenuStrip.Size = new System.Drawing.Size(99, 26);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSave);
            // 
            // openDialog
            // 
            this.openDialog.Filter = "Avalanche Archives (*.tab)|*.tab|All Files (*.*)|*.*";
            // 
            // saveFilesDialog
            // 
            this.saveFilesDialog.Description = "Select a directory to save all files from the archive to.";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "All Files (*.*)|*.*";
            // 
            // openButton
            // 
            this.openButton.Image = global::Gibbed.Avalanche.ArchiveViewer.Properties.Resources.OpenArchive;
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(56, 22);
            this.openButton.Text = "&Open";
            this.openButton.Click += new System.EventHandler(this.OnOpen);
            // 
            // saveAllButton
            // 
            this.saveAllButton.Image = global::Gibbed.Avalanche.ArchiveViewer.Properties.Resources.SaveAllFiles;
            this.saveAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllButton.Name = "saveAllButton";
            this.saveAllButton.Size = new System.Drawing.Size(68, 22);
            this.saveAllButton.Text = "Save &All";
            // 
            // reloadListsButton
            // 
            this.reloadListsButton.Image = global::Gibbed.Avalanche.ArchiveViewer.Properties.Resources.ReloadLists;
            this.reloadListsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadListsButton.Name = "reloadListsButton";
            this.reloadListsButton.Size = new System.Drawing.Size(89, 22);
            this.reloadListsButton.Text = "&Reload Lists";
            this.reloadListsButton.Click += new System.EventHandler(this.OnReloadLists);
            // 
            // settingsButton
            // 
            this.settingsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveOnlyknownFilesMenuItem,
            this.decompressUnknownFilesMenuItem});
            this.settingsButton.Image = global::Gibbed.Avalanche.ArchiveViewer.Properties.Resources.Settings;
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(78, 22);
            this.settingsButton.Text = "Settings";
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
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 320);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.mainToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Viewer";
            this.Text = "Gibbed\'s Avalanche Archive Viewer";
            this.Load += new System.EventHandler(this.OnLoad);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.fileMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.TreeView fileList;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.FolderBrowserDialog saveFilesDialog;
        private System.Windows.Forms.ToolStripButton reloadListsButton;
        private System.Windows.Forms.ToolStripComboBox projectComboBox;
        private System.Windows.Forms.ContextMenuStrip fileMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripButton saveAllButton;
        private System.Windows.Forms.ToolStripDropDownButton settingsButton;
        private System.Windows.Forms.ToolStripMenuItem saveOnlyknownFilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompressUnknownFilesMenuItem;
    }
}

