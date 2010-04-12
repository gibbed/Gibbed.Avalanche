using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Avalanche.FileFormats;
using Gibbed.Helpers;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using NDesk.Options;

namespace Gibbed.Avalanche.SmallPack
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            const int alignment = 4;
            bool littleEndian = true;
            bool verbose = false;
            bool compress = false;
            bool showHelp = false;

            OptionSet options = new OptionSet()
            {
                {
                    "v|verbose",
                    "be verbose (list files)",
                    v => verbose = v != null
                },
                {
                    "b|bigendian",
                    "write in big endian mode",
                    v => littleEndian = v == null
                },
                {
                    "c|compress",
                    "compress small archive with zlib.",
                    v => compress = v != null
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
            };

            List<string> extra;

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extra.Count < 1 || extra.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_directory [output_sarc]", GetExecutableName());
                Console.WriteLine("Pack specified directory.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extra[0];
            string outputPath = extra.Count > 1 ? extra[1] : inputPath + ".sarc";

            Stream output = File.Create(outputPath);

            if (compress == true)
            {
                output = new DeflaterOutputStream(output, new Deflater(Deflater.BEST_SPEED, false));
            }

            List<string> paths = new List<string>();
            paths.AddRange(Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories));
            paths.Sort(new NameComparer());

            int headerSize = 0;
            foreach (string path in paths)
            {
                headerSize += 4 + Path.GetFileName(path).Length + 4 + 4;
            }
            headerSize = headerSize.Align(16);

            // TODO: rewrite this terrible, terrible code.
            SmallArchiveFile smallArchive = new SmallArchiveFile();
            int offset = 16 + headerSize;
            foreach (string path in paths)
            {
                smallArchive.Entries.Add(new SmallArchiveFile.Entry()
                    {
                        Name = Path.GetFileName(path),
                        Offset = (uint)offset,
                        Size = (uint)((new FileInfo(path)).Length),
                    });
                offset += (int)((new FileInfo(path)).Length);
                offset = offset.Align(alignment);
            }

            smallArchive.LittleEndian = littleEndian;
            smallArchive.Serialize(output);

            byte[] buffer = new byte[0x4000];
            foreach (string path in paths)
            {
                Console.WriteLine("Adding {0}...", Path.GetFileName(path));
                Stream input = File.OpenRead(path);
                int total = 0;
                while (true)
                {
                    int read = input.Read(buffer, 0, 0x4000);
                    if (read == 0)
                    {
                        break;
                    }
                    output.Write(buffer, 0, read);
                    total += read;
                }
                input.Close();

                int dummySize = total.Align(alignment) - total;
                if (dummySize > 0)
                {
                    byte[] dummyBlock = new byte[dummySize];
                    output.Write(dummyBlock, 0, dummySize);
                }
            }

            output.Flush();
            output.Close();
        }
    }
}
