using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Avalanche.FileFormats;
using Gibbed.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using NDesk.Options;

namespace Gibbed.Avalanche.SmallUnpack
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool verbose = false;
            bool overwriteFiles = false;
            bool decompress = false;
            bool listing = false;
            bool showHelp = false;

            var options = new OptionSet()
            {
                {
                    "v|verbose",
                    "be verbose (list files)",
                    v => verbose = v != null
                },
                {
                    "l|list",
                    "just list files (don't extract)", 
                    v => listing = v != null
                },
                {
                    "o|overwrite",
                    "overwrite files if they already exist", 
                    v => overwriteFiles = v != null
                },
                {
                    "d|decompress",
                    "decompress a zlib compressed small archive.",
                    v => decompress = v != null
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
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_sarc [output_directory]", GetExecutableName());
                Console.WriteLine("Unpack specified small archive.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extra[0];
            string outputPath = extra.Count > 1 ?
                extra[1] : Path.ChangeExtension(inputPath, null) + "_unpack";

            using (var input = File.OpenRead(inputPath))
            {
                if (input.ReadValueU8() == 0x78)
                {
                    Console.WriteLine("Detected compressed SARC.");
                    decompress = true;
                }
                input.Seek(-1, SeekOrigin.Current);

                Stream data;
                if (decompress == false)
                {
                    data = input;
                }
                else
                {
                    var decompressed = new MemoryStream();

                    //var zlib = new ZlibStream(input, CompressionMode.Decompress);
                    var zlib = new InflaterInputStream(input);

                    byte[] buffer = new byte[0x4000];
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
                        decompressed.Write(buffer, 0, read);
                    }

                    zlib.Close();
                    input.Close();
                    decompressed.Position = 0;

                    data = decompressed;
                }

                if (listing == false)
                {
                    Directory.CreateDirectory(outputPath);
                }

                var smallArchive = new SmallArchiveFile();
                smallArchive.Deserialize(data);

                long counter = 0;
                long skipped = 0;
                long totalCount = smallArchive.Entries.Count;
                Console.WriteLine("{0} files in small archive.", totalCount);
                {
                    foreach (var entry in smallArchive.Entries)
                    {
                        counter++;

                        string entryName = Path.GetFileName(entry.Name);
                        string entryPath = Path.Combine(outputPath, entryName);

                        if (overwriteFiles == false && File.Exists(entryPath) == true)
                        {
                            Console.WriteLine("{1:D4}/{2:D4} !! {0}", entry.Name, counter, totalCount);
                            skipped++;
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("{1:D4}/{2:D4} => {0}", entry.Name, counter, totalCount);
                        }

                        if (listing == false)
                        {
                            using (var output = File.Open(entryPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                data.Seek(entry.Offset, SeekOrigin.Begin);
                                output.WriteFromStream(data, entry.Size);
                            }
                        }
                    }

                    if (skipped > 0)
                    {
                        Console.WriteLine("{0} files not overwritten.", skipped);
                    }
                }
            }
        }
    }
}
