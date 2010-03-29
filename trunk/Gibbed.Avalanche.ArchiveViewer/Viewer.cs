﻿using System;
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
            this.LoadProject();
        }

        private Setup.Manager Manager;

        private void LoadProject()
        {
            this.Manager = Setup.Manager.Load();
            this.projectComboBox.Items.AddRange(this.Manager.ToArray());
            this.SetProject(this.Manager.ActiveProject);
        }

        private void SetProject(Setup.Project project)
        {
            if (project != this.Manager.ActiveProject)
            {
                this.Manager.ActiveProject = project;
            }

            this.projectComboBox.SelectedItem = project;
            if (project != null)
            {
                project.Load();
                this.openDialog.InitialDirectory = project.InstallPath;
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
        }

        private ArchiveTableFile Table;
        private void BuildFileTree()
        {
            this.fileList.Nodes.Clear();
            this.fileList.BeginUpdate();

            if (this.Table != null)
            {
                Dictionary<string, TreeNode> dirNodes = new Dictionary<string, TreeNode>();

                TreeNode baseNode = new TreeNode(Path.GetFileName(this.openDialog.FileName), 0, 0);
                TreeNode knownNode = new TreeNode("Known", 1, 1);
                TreeNode unknownNode = new TreeNode("Unknown", 1, 1);

                Dictionary<uint, string> lookup = 
                    this.Manager.ActiveProject == null ? null : this.Manager.ActiveProject.FileHashLookup;

                foreach (uint hash in this.Table.Keys
                    .OrderBy(k => k, new FileNameHashComparer(lookup)))
                {
                    ArchiveTableFile.Entry entry = this.Table[hash];
                    TreeNode node = null;

                    if (lookup != null && lookup.ContainsKey(hash) == true)
                    {
                        string fileName = lookup[hash];
                        string pathName = Path.GetDirectoryName(fileName);
                        TreeNodeCollection parentNodes = knownNode.Nodes;

                        if (pathName.Length > 0)
                        {
                            string[] dirs = pathName.Split(new char[] { '\\' });

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
                    unknownNode.Text = "Unknown (" + unknownNode.Nodes.Count.ToString() + ")";
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

            if (this.openDialog.InitialDirectory != null)
            {
                this.openDialog.InitialDirectory = null;
            }

            Stream input = this.openDialog.OpenFile();
            ArchiveTableFile table = new ArchiveTableFile();
            table.Deserialize(input);
            input.Close();

            this.Table = table;

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
            Dictionary<uint, string> lookup;
            List<uint> saving;

            var root = this.fileList.SelectedNode;
            if (root.Nodes.Count == 0)
            {
                this.saveFileDialog.FileName = root.Text;

                if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                saving = new List<uint>();
                saving.Add((uint)root.Tag);

                lookup = new Dictionary<uint, string>();
                lookup.Add((uint)root.Tag, Path.GetFileName(this.saveFileDialog.FileName));
                basePath = Path.GetDirectoryName(this.saveFileDialog.FileName);
            }
            else
            {
                if (this.saveFilesDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                saving = new List<uint>();
                
                List<TreeNode> nodes = new List<TreeNode>();
                nodes.Add(root);

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

                lookup = this.Manager.ActiveProject == null ? null : this.Manager.ActiveProject.FileHashLookup;
                basePath = this.saveFilesDialog.SelectedPath;
            }

            Stream input = File.OpenRead(Path.ChangeExtension(this.openDialog.FileName, ".arc"));

            if (input == null)
            {
                return;
            }

            SaveProgress progress = new SaveProgress();
            progress.ShowSaveProgress(
                this,
                input,
                this.Table,
                saving,
                lookup,
                basePath,
                false,
                this.decompressUnknownFilesMenuItem.Checked);

            input.Close();
        }

        private void OnSaveAll(object sender, EventArgs e)
        {
            if (this.saveFilesDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Stream input = File.OpenRead(Path.ChangeExtension(this.openDialog.FileName, ".arc"));

            if (input == null)
            {
                return;
            }

            string basePath = this.saveFilesDialog.SelectedPath;

            Dictionary<uint, string> lookup =
                this.Manager.ActiveProject == null ? null : this.Manager.ActiveProject.FileHashLookup;

            SaveProgress progress = new SaveProgress();
            progress.ShowSaveProgress(this, input, this.Table, null, lookup, basePath, this.saveOnlyknownFilesMenuItem.Checked, this.decompressUnknownFilesMenuItem.Checked);

            input.Close();
        }

        private void OnReloadLists(object sender, EventArgs e)
        {
            if (this.Manager.ActiveProject != null)
            {
                this.Manager.ActiveProject.Reload();
            }

            this.BuildFileTree();
        }

        private void OnProjectSelected(object sender, EventArgs e)
        {
            this.projectComboBox.Invalidate();
            var project = this.projectComboBox.SelectedItem as Setup.Project;
            if (project == null)
            {
                this.projectComboBox.Items.Remove(this.projectComboBox.SelectedItem);
            }

            this.SetProject(project);
            this.BuildFileTree();
        }
    }
}
