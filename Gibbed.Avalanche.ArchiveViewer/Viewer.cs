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
using System.Linq;
using System.Windows.Forms;
using Gibbed.Avalanche.FileFormats;

namespace Gibbed.Avalanche.ArchiveViewer
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            this.InitializeComponent();
        }

        private ProjectData.Manager _Manager;
        private ProjectData.HashList<uint> _Hashes;

        private void OnLoad(object sender, EventArgs e)
        {
            this.LoadProject();
        }

        private void LoadProject()
        {
            try
            {
                this._Manager = ProjectData.Manager.Load();
                this.projectComboBox.Items.AddRange(this._Manager
                                                        .Cast<object>()
                                                        .ToArray());
                this.SetProject(this._Manager.ActiveProject);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "There was an error while loading project data." +
                    Environment.NewLine + Environment.NewLine +
                    e +
                    Environment.NewLine + Environment.NewLine +
                    "(You can press Ctrl+C to copy the contents of this dialog)",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void SetProject(ProjectData.Project project)
        {
            if (project != null)
            {
                try
                {
                    this.openDialog.InitialDirectory = project.InstallPath;
                    this.saveKnownFileListDialog.InitialDirectory = project.ListsPath;
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "There was an error while loading project data." +
                        Environment.NewLine + Environment.NewLine +
                        e +
                        Environment.NewLine + Environment.NewLine +
                        "(You can press Ctrl+C to copy the contents of this dialog)",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    project = null;
                }

                this._Hashes = project.LoadListsFileNames();
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (project != this._Manager.ActiveProject) // ReSharper restore RedundantCheckBeforeAssignment
            {
                this._Manager.ActiveProject = project;
            }

            this.projectComboBox.SelectedItem = project;
        }

        private ArchiveTableFile _Table;

        private void BuildFileTree()
        {
            this.fileList.Nodes.Clear();
            this.fileList.BeginUpdate();

            if (this._Table != null)
            {
                var dirNodes = new Dictionary<string, TreeNode>();

                var baseNode = new TreeNode(Path.GetFileName(this.openDialog.FileName), 0, 0);
                var knownNode = new TreeNode("Known", 1, 1);
                var unknownNode = new TreeNode("Unknown", 1, 1);

                foreach (uint hash in this._Table.Keys
                    .OrderBy(k => k, new FileNameHashComparer(this._Hashes)))
                {
                    ArchiveTableFile.Entry entry = this._Table[hash];
                    TreeNode node;

                    if (this._Hashes != null && this._Hashes.Contains(hash) == true)
                    {
                        string fileName = this._Hashes[hash];
                        string pathName = Path.GetDirectoryName(fileName);
                        TreeNodeCollection parentNodes = knownNode.Nodes;

                        if (string.IsNullOrEmpty(pathName) == false)
                        {
                            string[] dirs = pathName.Split(new[]
                            {
                                '\\'
                            });

                            foreach (string dir in dirs)
                            {
                                if (parentNodes.ContainsKey(dir))
                                {
                                    parentNodes = parentNodes[dir].Nodes;
                                }
                                else
                                {
                                    TreeNode parentNode = parentNodes.Add(dir, dir, 2, 2);
                                    parentNodes = parentNode.Nodes;
                                }
                            }
                        }

                        node = parentNodes.Add(null, Path.GetFileName(fileName), 3, 3);
                    }
                    else
                    {
                        node = unknownNode.Nodes.Add(null, hash.ToString("X8"), 3, 3);
                    }

                    node.Tag = hash;
                }

                if (knownNode.Nodes.Count > 0)
                {
                    baseNode.Nodes.Add(knownNode);
                }

                if (unknownNode.Nodes.Count > 0)
                {
                    baseNode.Nodes.Add(unknownNode);
                    unknownNode.Text = "Unknown (" +
                                       unknownNode.Nodes.Count.ToString(
                                           System.Globalization.CultureInfo.InvariantCulture) + ")";
                }

                if (knownNode.Nodes.Count > 0)
                {
                    knownNode.Expand();
                }
                else if (unknownNode.Nodes.Count > 0)
                {
                    //unknownNode.Expand();
                }

                baseNode.Expand();
                this.fileList.Nodes.Add(baseNode);
            }

            //this.fileList.Sort();
            this.fileList.EndUpdate();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            if (this.openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (this.openDialog.InitialDirectory != null) // ReSharper restore RedundantCheckBeforeAssignment
            {
                this.openDialog.InitialDirectory = null;
            }

            using (var input = this.openDialog.OpenFile())
            {
                var table = new ArchiveTableFile();
                table.Deserialize(input);
                this._Table = table;
            }

            /*
            TextWriter writer = new StreamWriter("all_file_hashes.txt");
            foreach (var hash in table.Keys.OrderBy(k => k))
            {
                writer.WriteLine(hash.ToString("X8"));
            }
            writer.Close();
            */

            this.BuildFileTree();
        }

        private void OnSave(object sender, EventArgs e)
        {
            if (this.fileList.SelectedNode == null)
            {
                return;
            }

            string basePath;
            List<uint> saving;

            SaveProgress.SaveAllSettings settings;
            settings.DecompressUnknownFiles = this.decompressUnknownFilesMenuItem.Checked;
            settings.DecompressSmallArchives = this.decompressSmallArchivesMenuItem.Checked;
            settings.SaveOnlyKnownFiles = false;
            settings.DontOverwriteFiles = this.dontOverwriteFilesMenuItem.Checked;

            var root = this.fileList.SelectedNode;
            if (root.Nodes.Count == 0)
            {
                this.saveFileDialog.FileName = root.Text;

                if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                saving = new List<uint>()
                {
                    (uint)root.Tag,
                };

                // ReSharper disable UseObjectOrCollectionInitializer
                var lookup = new Dictionary<uint, string>();
                // ReSharper restore UseObjectOrCollectionInitializer
                lookup.Add((uint)root.Tag, Path.GetFileName(this.saveFileDialog.FileName));
                basePath = Path.GetDirectoryName(this.saveFileDialog.FileName);

                settings.DontOverwriteFiles = false;
            }
            else
            {
                if (this.saveFilesDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                saving = new List<uint>();

                var nodes = new List<TreeNode>()
                {
                    root,
                };

                while (nodes.Count > 0)
                {
                    var node = nodes[0];
                    nodes.RemoveAt(0);

                    if (node.Nodes.Count > 0)
                    {
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (child.Nodes.Count > 0)
                            {
                                nodes.Add(child);
                            }
                            else
                            {
                                saving.Add((uint)child.Tag);
                            }
                        }
                    }
                }

                basePath = this.saveFilesDialog.SelectedPath;
            }

            var input = File.OpenRead(Path.ChangeExtension(this.openDialog.FileName, ".arc"));

            var progress = new SaveProgress();
            progress.ShowSaveProgress(
                this,
                input,
                this._Table,
                saving,
                this._Hashes,
                basePath,
                settings);

            input.Close();
        }

        private void OnSaveAll(object sender, EventArgs e)
        {
            if (this.saveFilesDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var input = File.OpenRead(Path.ChangeExtension(this.openDialog.FileName, ".arc"));

            string basePath = this.saveFilesDialog.SelectedPath;

            SaveProgress.SaveAllSettings settings;
            settings.DecompressUnknownFiles = this.decompressUnknownFilesMenuItem.Checked;
            settings.DecompressSmallArchives = this.decompressSmallArchivesMenuItem.Checked;
            settings.SaveOnlyKnownFiles = this.saveOnlyKnownFilesMenuItem.Checked;
            settings.DontOverwriteFiles = this.dontOverwriteFilesMenuItem.Checked;

            var progress = new SaveProgress();
            progress.ShowSaveProgress(
                this,
                input,
                this._Table,
                null,
                this._Hashes,
                basePath,
                settings);

            input.Close();
        }

        private void OnReloadLists(object sender, EventArgs e)
        {
            this._Hashes = this._Manager.ActiveProject != null
                               ? this._Manager.ActiveProject.LoadListsFileNames()
                               : null;

            this.BuildFileTree();
        }

        private void OnProjectSelected(object sender, EventArgs e)
        {
            this.projectComboBox.Invalidate();
            var project = this.projectComboBox.SelectedItem as ProjectData.Project;
            if (project == null)
            {
                this.projectComboBox.Items.Remove(this.projectComboBox.SelectedItem);
            }
            this.SetProject(project);
            this.BuildFileTree();
        }

        private void OnSaveKnownFileList(object sender, EventArgs e)
        {
            if (this.saveKnownFileListDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var names = new List<string>();

            if (this._Table != null &&
                this._Manager.ActiveProject != null)
            {
                names.AddRange(from hash in this._Table.Keys
                               where this._Hashes.Contains(hash) == true
                               select this._Hashes[hash]);
            }

            names.Sort();

            TextWriter output = new StreamWriter(this.saveKnownFileListDialog.OpenFile());
            foreach (string name in names)
            {
                output.WriteLine(name);
            }
            output.Close();
        }
    }
}
