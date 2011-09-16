using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Gibbed.Avalanche.FileFormats;
using Gibbed.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Gibbed.Avalanche.ArchiveViewer
{
	public partial class SaveProgress : Form
	{
		public SaveProgress()
		{
			this.InitializeComponent();
		}

		delegate void SetStatusDelegate(string status, int percent);
		private void SetStatus(string status, int percent)
		{
			if (this.progressBar.InvokeRequired || this.statusLabel.InvokeRequired)
			{
				SetStatusDelegate callback = new SetStatusDelegate(SetStatus);
				this.Invoke(callback, new object[] { status, percent });
				return;
			}

			this.statusLabel.Text = status;
			this.progressBar.Value = percent;
		}

		delegate void SaveDoneDelegate();
		private void SaveDone()
		{
			if (this.InvokeRequired)
			{
				SaveDoneDelegate callback = new SaveDoneDelegate(SaveDone);
				this.Invoke(callback);
				return;
			}

			this.Close();
		}

		public void SaveAll(object oinfo)
		{
			SaveAllInformation info = (SaveAllInformation)oinfo;
			Dictionary<uint, string> UsedNames = new Dictionary<uint, string>();

            IEnumerable<uint> saving;

            if (info.Saving == null)
            {
                saving = info.Table.Keys;
            }
            else
            {
                saving = info.Saving;
            }

            this.SetStatus("", 0);

            int total = saving.Count();
            int current = 0;

            byte[] buffer = new byte[0x4000];
			foreach (var hash in saving)
			{
                current++;

                ArchiveTableFile.Entry index = info.Table.Get(hash);
				string fileName = null;

                bool decompressing = false;
                if (info.FileNames.Contains(hash) == true)
				{
					fileName = info.FileNames[hash];
					UsedNames[hash] = info.FileNames[hash];

                    if (info.Settings.DecompressSmallArchives == true)
                    {
                        string extension = Path.GetExtension(fileName);
                        if (extension == ".blz" || extension == ".blx360z" || extension == ".bl3z" ||
                            extension == ".eez" || extension == ".eex360z" || extension == ".ee3z" ||
                            extension == ".flz" ||
                            extension == ".nlz")
                        {
                            decompressing = true;
                        }
                    }
				}
				else
				{
					if (info.Settings.SaveOnlyKnownFiles)
					{
                        this.SetStatus("Skipping...", (int)(((float)current / (float)total) * 100.0f));
						continue;
					}

					fileName = hash.ToString("X8");

                    if (true)
                    {
                        info.Archive.Seek(index.Offset, SeekOrigin.Begin);
                        byte[] guess = new byte[16];
                        int read = info.Archive.Read(guess, 0, (int)Math.Min(guess.Length, index.Size));

                        if (read >= 2 && guess[0] == 0x78 && guess[1] == 0x01
                            && info.Settings.DecompressUnknownFiles == true)
                        {
                            info.Archive.Seek(index.Offset, SeekOrigin.Begin);

                            decompressing = true;
                            using (var memory = info.Archive.ReadToMemoryStream(index.Size))
                            {
                                var zlib = new InflaterInputStream(memory);
                                read = zlib.Read(guess, 0, guess.Length);
                                if (read < 0)
                                {
                                    throw new InvalidOperationException("zlib error");
                                }
                            }
                        }

                        var extension = FileExtensions.Detect(guess, read);

                        if (extension.Value != null)
                        {
                            fileName = Path.ChangeExtension(fileName, "." + extension.Value);
                        }

                        if (extension.Key != null)
                        {
                            fileName = Path.Combine(extension.Key, fileName);
                        }
                    }

                    fileName = Path.Combine("__UNKNOWN", fileName);
				}

				string path = Path.Combine(info.BasePath, fileName);
                if (File.Exists(path) == true &&
                    info.Settings.DontOverwriteFiles == true)
                {
                    this.SetStatus("Skipping...", (int)(((float)current / (float)total) * 100.0f));
                    continue;
                }

                this.SetStatus(fileName, (int)(((float)current / (float)total) * 100.0f));

                Directory.CreateDirectory(Path.Combine(info.BasePath, Path.GetDirectoryName(fileName)));

                info.Archive.Seek(index.Offset, SeekOrigin.Begin);

                using (var output = File.Create(path))
                {
                    if (decompressing == true)
                    {
                        using (var memory = info.Archive.ReadToMemoryStream(index.Size))
                        {
                            var zlib = new InflaterInputStream(memory);
                            while (true)
                            {
                                int read = zlib.Read(buffer, 0, buffer.Length);
                                if (read < 0)
                                {
                                    throw new InvalidOperationException("zlib error");
                                }
                                else if (read == 0)
                                {
                                    break;
                                }
                                output.Write(buffer, 0, read);
                            }
                        }
                    }
                    else
                    {
                        output.WriteFromStream(info.Archive, index.Size);
                    }
                }
			}

			this.SaveDone();
		}

        public struct SaveAllSettings
        {
            public bool SaveOnlyKnownFiles;
            public bool DecompressUnknownFiles;
            public bool DecompressSmallArchives;
            public bool DontOverwriteFiles;
        }

		private struct SaveAllInformation
		{
			public string BasePath;
			public Stream Archive;
			public ArchiveTableFile Table;
            public List<uint> Saving;
            public ProjectData.HashList<uint> FileNames;
            public SaveAllSettings Settings;
		}

		private Thread SaveThread;
		public void ShowSaveProgress(
            IWin32Window owner,
            Stream archive,
            ArchiveTableFile table,
            List<uint> saving,
            ProjectData.HashList<uint> fileNames,
            string basePath,
            SaveAllSettings settings)
		{
			SaveAllInformation info;
			info.BasePath = basePath;
			info.Archive = archive;
			info.Table = table;
            info.Saving = saving;
			info.FileNames = fileNames;
            info.Settings = settings;

			this.progressBar.Value = 0;
			this.progressBar.Maximum = 100;

			this.SaveThread = new Thread(new ParameterizedThreadStart(SaveAll));
			this.SaveThread.Start(info);
			this.ShowDialog(owner);
		}

		private void OnCancel(object sender, EventArgs e)
		{
			if (this.SaveThread != null)
			{
				this.SaveThread.Abort();
			}

			this.Close();
		}
	}
}
