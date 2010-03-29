using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Helpers;
using Gibbed.Avalanche.FileFormats;
using System.IO;

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
                AddHashes(hashes, propertyFile.Root);
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

            Stream input = File.OpenRead(@"T:\JC2\retail\pc0\exported\cdoll\mc09_razman\mc09_lod1-razman.rbm");
            var model = new RbModel();
            model.Deserialize(input);
            input.Close();

            Stream output;
            
            output = File.OpenWrite(@"T:\JC2\retail\pc0\exported\cdoll\mc09_razman\shiftedvertices_6.bin");
            var mesh6 = model.RenderBlocks[6] as Gibbed.Avalanche.FileFormats.Model.SkinnedGeneral;
            foreach (var vertex in mesh6.Vertices_0)
            {
                vertex.PositionZ -= 0.005f;
                vertex.Serialize(output);
            }
            output.Close();

            output = File.OpenWrite(@"T:\JC2\retail\pc0\exported\cdoll\mc09_razman\shiftedvertices_15.bin");
            var mesh15 = model.RenderBlocks[15] as Gibbed.Avalanche.FileFormats.Model.SkinnedGeneral;
            foreach (var vertex in mesh15.Vertices_0)
            {
                vertex.PositionZ -= 0.005f;
                vertex.Serialize(output);
            }
            output.Close();
        }

        private static uint above_cam = "wire_whip".HashJenkins();

        private static void AddHashes(List<uint> hashes, PropertyNode obj)
        {
            if (obj.ValuesByHash != null)
            {
                foreach (var hash in obj.ValuesByHash.Keys)
                {
                    if (hash == above_cam)
                    {
                    }
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
                    if (child.Key == above_cam)
                    {
                    }

                    if (hashes.Contains(child.Key) == false)
                    {
                        hashes.Add(child.Key);
                    }

                    AddHashes(hashes, child.Value);
                }
            }
        }
    }
}
