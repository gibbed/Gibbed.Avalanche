using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.IO;
using Gibbed.Avalanche.FileFormats;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Gibbed.Avalanche.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            #region old crap
            /*
            Stream input = File.OpenRead(@"T:\JC2\demo\pc0\gui\frontend\FrontEnd.agui");
            AnarkFile anark = new AnarkFile();
            anark.Deserialize(input);
            input.Close();
            */

            /*
            List<uint> hashes = new List<uint>();

            foreach (string path in Directory.GetFiles(@"T:\JC2\demo\pc0\settings", "*.bin", SearchOption.AllDirectories))
            {
                var input = File.OpenRead(path);
                var propertyFile = new PropertyFile();
                propertyFile.Deserialize(input);
                input.Close();
            }

            hashes.Sort();

            /*
            TextWriter writer = new StreamWriter("all_hashes.txt");
            foreach (var hash in hashes)
            {
                writer.WriteLine(hash.ToString("X8"));
            }
            writer.Close();
            */

            //float x = BitConverter.ToSingle(new byte[] { 0x4D, 0x95, 0xF3, 0xC5 }, 0);
            //float y = BitConverter.ToSingle(new byte[] { 0xC7, 0x5F, 0x6E, 0x43 }, 0);
            //float z = BitConverter.ToSingle(new byte[] { 0xA8, 0xAB, 0xF8, 0x45 }, 0);

            //float u1 = BitConverter.ToSingle(new byte[] { 0xE4, 0xA2, 0xD4, 0x41 }, 0);
            //float w1 = BitConverter.ToSingle(new byte[] { 0xC1, 0x1C, 0x9A, 0x5A }, 0);
            //float w2 = BitConverter.ToSingle(new byte[] { 0xA6, 0xE2, 0xE7, 0x03 }, 0);

            //string hash1 = "dialog_fre.fev".HashJenkins().ToString("X8");
            /*
            string hash2 = "time".HashJenkins().ToString("X8");
            string hash3 = "skip".HashJenkins().ToString("X8");
            string hash4 = "after".HashJenkins().ToString("X8");
            string hash5 = "arve.v010_personal_propellerplane.ee".HashJenkins().ToString("X8");
            */
            //string hash2 = "{358C4085-E31C-47DF-981A-1E7DAE455A9B)".HashJenkins().ToString("X8");
            //string bbbb = BitConverter.ToString(BitConverter.GetBytes(4.0f)).Replace("-", "");

            //Stream input = File.OpenRead(@"T:\JC2\demo\pc0\characters\maincharacters\rico\mc01_lod1-rico.rbm");
            //Stream input = File.OpenRead(@"T:\JC2\demo\pc0\exported\cdoll\pd_ms_witness\pd_ms_witness_lod1-base.rbm");
            /*
            Stream input = File.OpenRead(@"T:\JC2\retail\pc0\exported\cdoll\pd_ninja\pd_ninja_lod1-base.rbm");
            var model = new RbModel();
            model.Deserialize(input);
            input.Close();

            TextWriter output = new StreamWriter("test_witness.obj");
            output.WriteLine("mtllib test_witness.mtl");

            int offset = 1;
            float x = 0.0f;
            foreach (var block in model.RenderBlocks)
            {
                var skinned = block as Gibbed.Avalanche.FileFormats.Model.SkinnedGeneral;
                if (skinned == null)
                {
                    continue;
                }

                output.WriteLine("g group_{0}", offset);
                output.WriteLine("usemtl {0}", Path.GetFileNameWithoutExtension(skinned.Unknown07[0]));

                if ((skinned.Flags & 0x80000) == 0x80000)
                {
                    throw new Exception();
                    //continue;

                    foreach (var data in skinned.Vertices_1)
                    {
                        output.WriteLine("v {0} {1} {2}",
                            data.PositionX + x,
                            data.PositionY,
                            data.PositionZ);
                    }
                }
                else
                {
                    foreach (var data in skinned.Vertices_0)
                    {
                        output.WriteLine("v {0} {1} {2}",
                            data.PositionX + x,
                            data.PositionY,
                            data.PositionZ);
                    }
                    foreach (var data in skinned.Vertices_0)
                    {
                        output.WriteLine("vn {0} {1} {2}",
                            data.TexCoord2A / 255.0f,
                            data.TexCoord2B / 255.0f,
                            data.TexCoord2C / 255.0f);
                    }
                }

                foreach (var data in skinned.Unknown10)
                {
                    output.WriteLine("vt {0} {1}",
                        data.U,
                        data.V);
                }

                for (int i = 0; i < skinned.Unknown12.Count; i += 3)
                {
                    output.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
                        skinned.Unknown12[i + 0] + offset,
                        skinned.Unknown12[i + 1] + offset,
                        skinned.Unknown12[i + 2] + offset);
                }

                //x += 2.0f;
                offset += skinned.Unknown10.Count;
                //break;
            }
            output.Close();
            */
            #endregion
            #region old crap 2
            /*
            Stream input = File.OpenRead(@"T:\JC2\retail\pc0\missions\missions.bmf");
            var missions = new MissionsFile();
            missions.Deserialize(input);
            input.Close();

            foreach (var mission in missions.Missions)
            {
                string type = null;
                switch (mission.Type)
                {
                    case MissionsFile.MissionType.Challenge: type = "challenges"; break;
                    case MissionsFile.MissionType.KeyMission: type = "keymissions"; break;
                    case MissionsFile.MissionType.StrongholdTakeover: type = "strongholds"; break;
                    case MissionsFile.MissionType.FactionTask: type = "factiontasks"; break;
                    case MissionsFile.MissionType.FactionMission: type = "factionmissions"; break;
                    case MissionsFile.MissionType.FactionInterest1: type = "factioninterests"; break;
                    case MissionsFile.MissionType.FactionInterest2: type = "factioninterests"; break;
                    case MissionsFile.MissionType.FactionInterest3: type = "factioninterests"; break;
                    default: throw new Exception();
                }
                Console.WriteLine("missions\\{0}\\{1}.bl", type, mission.Name);
                Console.WriteLine("missions\\{0}\\{1}.blz", type, mission.Name);
            }
            */
            #endregion
            #region old crap 3
            /*
            Stream input = File.OpenRead(@"T:\JC2\retail\pc0\exported\cdoll\pd_ms_strippers_male1\pd_ms_stripper_male_lod1-sunglasses.rbm");
            var model = new RbModel();
            model.Deserialize(input);
            input.Close();

            Stream output;

            output = File.OpenWrite(@"T:\JC2\retail\pc0\exported\cdoll\pd_ms_strippers_male1\shiftedvertices_0.bin");
            var mesh6 = model.RenderBlocks[0] as Gibbed.Avalanche.FileFormats.Model.SkinnedGeneral;
            foreach (var vertex in mesh6.Vertices_0)
            {
                vertex.PositionZ -= 0.007f;
                vertex.Serialize(output);
            }
            output.Close();
            */
            #endregion
            #region old crap 4
            /*
            Stream input = File.OpenRead(@"T:\JC2\retail\pc0\gui\frontend\frontend.agui");
            AnarkFile anark = new AnarkFile();
            anark.Deserialize(input);
            input.Close();
            */
            #endregion
            #region old crap 5
            /*
            foreach (var path in Directory.GetFiles(@"T:\JC2\retail", "*.bin", SearchOption.AllDirectories))
            {
                var propertyFile = new PropertyFile();

                try
                {
                    Console.WriteLine("Checking {0}...", path);
                    var input = File.OpenRead(path);
                    propertyFile.Deserialize(input, true);
                    input.Close();
                }
                catch (Exception)
                {
                    continue;
                }

                if (FindHash(propertyFile, 0x0BC7370E1) == true)
                {
                }
            }
            */
            #endregion
            #region old crap 6
            /*
            List<string> classes = new List<string>();

            foreach (var path in Directory.GetFiles(@"T:\JC2\retail", "*.bin", SearchOption.AllDirectories))
            {
                var propertyFile = new PropertyFile();

                try
                {
                    Console.WriteLine("Checking {0}...", path);
                    var input = File.OpenRead(path);
                    propertyFile.Deserialize(input, true);
                    input.Close();
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var node in propertyFile.Nodes)
                {
                    AddClasses(classes, node);
                }
            }

            List<string> allSmallArchives = new List<string>();

            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.blz", SearchOption.AllDirectories));
            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.nlz", SearchOption.AllDirectories));
            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.flz", SearchOption.AllDirectories));

            foreach (var path in allSmallArchives)
            {
                Stream input = File.OpenRead(path);

                if (input.ReadValueU8() != 0x78)
                {
                    input.Seek(-1, SeekOrigin.Current);
                }
                else
                {
                    Stream decompressed = new MemoryStream();
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

                    input = decompressed;
                }

                SmallArchiveFile smallArchive = new SmallArchiveFile();
                smallArchive.Deserialize(input);

                foreach (var entry in smallArchive.Entries
                    .Where(e =>
                        e.Name.EndsWith(".bl") ||
                        e.Name.EndsWith(".vdoll") ||
                        e.Name.EndsWith(".mvdoll") ||
                        e.Name.EndsWith(".cdoll") ||
                        e.Name.EndsWith(".nl") ||
                        e.Name.EndsWith(".fl")
                    ))
                {
                    input.Seek(entry.Offset, SeekOrigin.Begin);
                    MemoryStream data = input.ReadToMemoryStream(entry.Size);

                    var propertyFile = new PropertyFile();

                    try
                    {
                        Console.WriteLine("Checking {0}...", path);
                        propertyFile.Deserialize(data, true);
                        data.Close();
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    foreach (var node in propertyFile.Nodes)
                    {
                        AddClasses(classes, node);
                    }
                }

                input.Close();
            }
            */
            /*
            string butt = "weapons_pc.bin".HashJenkins().ToString("X8");

            List<uint> hashes = new List<uint>();

            var propertyFile = new PropertyFile();
            //Stream input = File.OpenRead(@"T:\JC2\retail\pc0\missions\keymissions\km01\km01.bl.bl");
            Stream input = File.OpenRead(@"T:\JC2\retail\pc0\settings\player.bin");
            propertyFile.Deserialize(input, true);
            input.Close();

            foreach (var node in propertyFile.Nodes)
            {
                AddHashes(hashes, node);
            }
            */

            /*
            propertyFile = new PropertyFile();
            input = File.OpenRead(@"T:\JC2\retail\pc0\settings\heatsystem.bin");
            propertyFile.Deserialize(input, true);
            input.Close();

            foreach (var node in propertyFile.Nodes)
            {
                AddHashes(hashes, node);
            }

            hashes.Sort();

            propertyFile = new PropertyFile();
            input = File.OpenRead(@"T:\JC2\retail\pc0\global\areasets\areasets.bin");
            propertyFile.Deserialize(input, true);
            input.Close();

            foreach (var node in propertyFile.Nodes)
            {
                AddHashes(hashes, node);
            }
            */
            /*
            classes.Sort();

            TextWriter writer = new StreamWriter(@"U:\Projects\Avalanche\private\all_classes.txt");
            foreach (var cls in classes)
            {
                writer.WriteLine(string.Format("{0:X8} {1}", cls.HashJenkins(), cls));
            }
            writer.Close();
            */
            #endregion

            List<string> allSmallArchives = new List<string>();
            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.eez", SearchOption.AllDirectories));
            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.blz", SearchOption.AllDirectories));
            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.nlz", SearchOption.AllDirectories));
            allSmallArchives.AddRange(Directory.GetFiles(@"T:\JC2\retail", "*.flz", SearchOption.AllDirectories));

            List<string> allExtensions = new List<string>();

            foreach (var path in allSmallArchives)
            {
                Console.WriteLine("Scanning {0}...", path);

                Stream input = File.OpenRead(path);

                if (input.ReadValueU8() != 0x78)
                {
                    input.Seek(-1, SeekOrigin.Current);
                }
                else
                {
                    Stream decompressed = new MemoryStream();
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

                    input = decompressed;
                }

                SmallArchiveFile smallArchive = new SmallArchiveFile();
                smallArchive.Deserialize(input);

                List<string> extensions = new List<string>();
                string lastExtension = null;

                foreach (var entry in smallArchive.Entries)
                {
                    string extension = Path.GetExtension(entry.Name);
                    if (lastExtension != null &&
                        lastExtension != extension &&
                        extensions.Contains(extension) == true)
                    {
                        throw new InvalidOperationException();
                    }

                    lastExtension = extension;
                    if (extensions.Contains(extension) == false)
                    {
                        extensions.Add(extension);
                    }
                }

                if (extensions.Count > 1)
                {
                    if (allExtensions.Count == 0)
                    {
                        foreach (var extension in extensions)
                        {
                            allExtensions.Add(extension);
                        }
                    }
                    else
                    {
                        for (int x1 = 0; x1 < extensions.Count; x1++)
                        {
                            for (int y1 = 0; y1 < extensions.Count; y1++)
                            {
                                if (x1 == y1)
                                {
                                    continue;
                                }

                                string a = extensions[x1];
                                string b = extensions[y1];

                                int x2 = allExtensions.IndexOf(extensions[x1]);
                                int y2 = allExtensions.IndexOf(extensions[y1]);

                                if (x2 == -1 || y2 == -1)
                                {
                                    continue;
                                }

                                string[] ignore = new string[]
                                {
                                    //".epe",
                                    ".vdoll",
                                    ".aiteam",
                                    ".aiprop",
                                    ".dcs",
                                    //".blo",
                                    ".psmb",
                                    ".pfs",
                                    ".batchlod",
                                };
                                
                                if (ignore.Contains(a) ||
                                    ignore.Contains(b))
                                {
                                    continue;
                                }

                                if (x1 < y1)
                                {
                                    if (x2 < y2)
                                    {
                                        // all good
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }
                                else if (x1 > y1)
                                {
                                    if (x2 > y2)
                                    {
                                        // all good
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }
                            }
                        }

                        for (int x = 0; x < extensions.Count; x++)
                        {
                            if (allExtensions.Contains(extensions[x]) == true)
                            {
                                continue;
                            }

                            if (x == 0)
                            {
                                // after
                                int before = -1;
                                for (int y = 1; y < extensions.Count; y++)
                                {
                                    if (y == x)
                                    {
                                        continue;
                                    }
                                    int z = allExtensions.IndexOf(extensions[y]);
                                    if (z != -1)
                                    {
                                        if (before == -1 || z < before)
                                        {
                                            before = z;
                                        }
                                    }
                                }
                                if (before == -1)
                                {
                                    throw new InvalidOperationException();
                                }
                                allExtensions.Insert(before, extensions[x]);
                            }
                            else
                            {
                                // before
                                int after = -1;
                                for (int y = 0; y < x; y++)
                                {
                                    int z = allExtensions.IndexOf(extensions[y]);
                                    if (z != -1)
                                    {
                                        if (after == -1 || z > after)
                                        {
                                            after = z;
                                        }
                                    }
                                }
                                if (after != -1)
                                {
                                    allExtensions.Insert(after + 1, extensions[x]);
                                }
                                else
                                {
                                    // after
                                    int before = -1;
                                    for (int y = x + 1; y < extensions.Count; y++)
                                    {
                                        if (y == x)
                                        {
                                            continue;
                                        }
                                        int z = allExtensions.IndexOf(extensions[y]);
                                        if (z != -1)
                                        {
                                            if (before == -1 || z < before)
                                            {
                                                before = z;
                                            }
                                        }
                                    }
                                    if (before == -1)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    allExtensions.Insert(before, extensions[x]);
                                }
                            }
                        }
                    }
                }

                input.Close();
            }
        }

        private static uint above_cam = "wire_whip".HashJenkins();

        private static bool FindHash(PropertyFile propertyFile, uint hash)
        {
            foreach (var node in propertyFile.Nodes)
            {
                if (FindHash(node, hash) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool FindHash(PropertyNode node, uint hash)
        {
            if (node.ValuesByHash != null)
            {
                if (node.ValuesByHash.ContainsKey(hash) == true)
                {
                    return true;
                }
            }

            if (node.ChildrenByHash != null)
            {
                if (node.ChildrenByHash.ContainsKey(hash) == true)
                {
                    return true;
                }

                foreach (var child in node.ChildrenByHash)
                {
                    if (FindHash(child.Value, hash) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void AddHashes(List<uint> hashes, PropertyNode obj)
        {
            if (obj.ValuesByHash != null)
            {
                foreach (var hash in obj.ValuesByHash.Keys)
                {
                    if (hashes.Contains(hash) == false)
                    {
                        hashes.Add(hash);
                    }
                }
            }

            if (obj.ChildrenByHash != null)
            {
                foreach (var child in obj.ChildrenByHash)
                {
                    if (hashes.Contains(child.Key) == false)
                    {
                        hashes.Add(child.Key);
                    }

                    AddHashes(hashes, child.Value);
                }
            }
        }

        private static void AddClasses(List<string> classes, PropertyNode obj)
        {
            if (obj.ValuesByHash != null)
            {
                if (obj.ValuesByHash.ContainsKey("_class".HashJenkins()) == true)
                {
                    object value = obj.ValuesByHash["_class".HashJenkins()];
                    if (value is string)
                    {
                        if ((string)value == "SSmartObjectActionInfo")
                        {
                        }
                        if (classes.Contains((string)value) == false)
                        {
                            classes.Add((string)value);
                        }
                    }
                }
            }

            if (obj.ChildrenByHash != null)
            {
                foreach (var child in obj.ChildrenByHash)
                {
                    AddClasses(classes, child.Value);
                }
            }
        }
    }
}
