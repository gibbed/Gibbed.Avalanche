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
using Gibbed.Avalanche.FileFormats;
using Gibbed.IO;
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
            var endian = Endian.Little;
            bool verbose = false;
            bool compress = false;
            bool showHelp = false;

            var options = new OptionSet
            {
                {
                    "v|verbose", "be verbose (list files)", v => verbose = v != null
                    },
                {
                    "l|little-endian", "write in little endian mode", v =>
                    {
                        if (v != null)
                        {
                            endian = Endian.Little;
                        }
                    }
                    },
                {
                    "b|big-endian", "write in big endian mode", v =>
                    {
                        if (v != null)
                        {
                            endian = Endian.Big;
                        }
                    }
                    },
                {
                    "c|compress", "compress small archive with zlib.", v => compress = v != null
                    },
                {
                    "h|help", "show this message and exit", v => showHelp = v != null
                    }
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

            using (var output = File.Create(outputPath))
            {
                Stream data = output;

                if (compress == true)
                {
                    data = new DeflaterOutputStream(output, new Deflater(Deflater.BEST_SPEED, false));
                }

                var paths = new List<string>();
                paths.AddRange(Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories));
                paths.Sort(new NameComparer());

                int headerSize = 0;
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (string path in paths) // ReSharper restore LoopCanBeConvertedToQuery
                {
                    headerSize += 4 + (Path.GetFileName(path) ?? "").Length + 4 + 4;
                }
                headerSize = headerSize.Align(16);

                // TODO: rewrite this terrible, terrible code.
                var smallArchive = new SmallArchiveFile();
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

                smallArchive.Endian = endian;
                smallArchive.Serialize(data);

                var buffer = new byte[0x4000];
                foreach (string path in paths)
                {
                    if (verbose == true)
                    {
                        Console.WriteLine("Adding {0}...", Path.GetFileName(path));
                    }

                    int total = 0;
                    using (var input = File.OpenRead(path))
                    {
                        while (true)
                        {
                            int read = input.Read(buffer, 0, 0x4000);
                            if (read == 0)
                            {
                                break;
                            }
                            data.Write(buffer, 0, read);
                            total += read;
                        }
                    }

                    int dummySize = total.Align(alignment) - total;
                    if (dummySize > 0)
                    {
                        var dummyBlock = new byte[dummySize];
                        data.Write(dummyBlock, 0, dummySize);
                    }
                }

                var deflaterOutputStream = data as DeflaterOutputStream;
                if (deflaterOutputStream != null)
                {
                    (deflaterOutputStream).Finish();
                }

                data.Flush();
            }
        }
    }
}
