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
            /*
            Stream input = File.OpenRead(@"C:\Users\Rick\Desktop\JC2data\current\gui\frontend\FrontEnd.agui");
            AnarkFile anark = new AnarkFile();
            anark.Deserialize(input);
            input.Close();
            */

            /*
            List<uint> hashes = new List<uint>();

            foreach (string path in Directory.GetFiles(@"C:\Users\Rick\Desktop\JC2data\current\settings", "*.bin", SearchOption.AllDirectories))
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
            string hash1 = "dialog_fre.fev".HashJenkins().ToString("X8");
            /*
            string hash2 = "time".HashJenkins().ToString("X8");
            string hash3 = "skip".HashJenkins().ToString("X8");
            string hash4 = "after".HashJenkins().ToString("X8");
            string hash5 = "arve.v010_personal_propellerplane.ee".HashJenkins().ToString("X8");
            string bbbb = BitConverter.ToString(BitConverter.GetBytes(99999.0f)).Replace("-", "");
            */
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
