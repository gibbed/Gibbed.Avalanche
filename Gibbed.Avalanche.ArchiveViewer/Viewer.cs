using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gibbed.Avalanche.FileFormats;

namespace Gibbed.Avalanche.ArchiveViewer
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            this.InitializeComponent();

            this.FileNames = new Dictionary<uint, string>();
            if (Directory.Exists(Path.Combine(Application.StartupPath, "lists")))
            {
                //this.saveFileListDialog.InitialDirectory = Path.Combine(Application.StartupPath, "filelists");

                var listPaths = Directory.GetFiles(Path.Combine(Application.StartupPath, "lists"), "*.filelist", SearchOption.AllDirectories);
                foreach (string listPath in listPaths)
                {
                    this.LoadFileNames(listPath);
                }
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            string path;
            path = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 35110", "InstallLocation", null);
            if (string.IsNullOrEmpty(path) != true)
            {
                this.openDialog.InitialDirectory = Path.Combine(path, "archives_win32");
            }
        }

        private Dictionary<uint, string> FileNames;
        private void LoadFileNames(string path)
        {
            if (File.Exists(path))
            {
                TextReader reader = new StreamReader(path);

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (line.Length <= 0)
                    {
                        continue;
                    }

                    line = line.Replace('/', '\\');
                    line = line.ToLowerInvariant();
                    uint hash = Path.GetFileName(line).HashJenkinsFileName();
                    this.FileNames.Add(hash, line);
                }

                reader.Close();
            }
        }

        private ArchiveTableFile Table;
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

            Dictionary<string, TreeNode> dirNodes = new Dictionary<string, TreeNode>();

            this.fileList.Nodes.Clear();
            this.fileList.BeginUpdate();

            TreeNode baseNode = new TreeNode(Path.GetFileName(this.openDialog.FileName), 0, 0);
            TreeNode knownNode = new TreeNode("Known", 1, 1);
            TreeNode unknownNode = new TreeNode("Unknown", 1, 1);

            foreach (uint hash in table.Keys
                .OrderBy(k => k, new FileNameHashComparer(this.FileNames)))
            {
                ArchiveTableFile.Entry entry = table[hash];
                TreeNode node = null;

                if (this.FileNames.ContainsKey(hash) == true)
                {
                    string fileName = this.FileNames[hash];
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
                unknownNode.Expand();
            }

            baseNode.Expand();

            this.fileList.Nodes.Add(baseNode);
            //this.fileList.Sort();
            this.fileList.EndUpdate();
        }

        private void OnSaveAll(object sender, EventArgs e)
        {
            if (this.saveAllFolderDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Stream input = File.OpenRead(Path.ChangeExtension(this.openDialog.FileName, ".arc"));

            if (input == null)
            {
                return;
            }

            string basePath = this.saveAllFolderDialog.SelectedPath;

            SaveAllProgress progress = new SaveAllProgress();
            progress.ShowSaveProgress(this, input, this.Table, null, this.FileNames, basePath, this.saveOnlyknownFilesMenuItem.Checked, this.decompressUnknownFilesMenuItem.Checked);

            input.Close();
        }
    }
}
