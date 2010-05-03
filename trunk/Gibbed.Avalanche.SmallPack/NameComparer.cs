using System;
using System.Collections.Generic;
//using System.Linq;
using System.IO;
using System.Text;
using Gibbed.Avalanche.FileFormats;

namespace Gibbed.Avalanche.SmallPack
{
    internal class NameComparer : IComparer<string>
    {
        private static List<string> Extensions;
        static NameComparer()
        {
            Extensions = new List<string>();
            /*Extensions.Add(".dds");
            Extensions.Add(".rbm");
            Extensions.Add(".lod");
            Extensions.Add(".pal");
            Extensions.Add(".pfx");
            Extensions.Add(".dec");*/
            Extensions.Add(".bsk");
            Extensions.Add(".ban");
            Extensions.Add(".pfs");
            Extensions.Add(".dds");
            Extensions.Add(".agui");
            Extensions.Add(".rbm");
            Extensions.Add(".batchlod");
            Extensions.Add(".lod");
            Extensions.Add(".pal");
            Extensions.Add(".cdoll");
            Extensions.Add(".pfx");
            Extensions.Add(".dec");
            Extensions.Add(".vdoll");
            Extensions.Add(".mvdoll");
            Extensions.Add(".sst");
            Extensions.Add(".aiteam");
            Extensions.Add(".pfi");
            Extensions.Add(".dcs");
            Extensions.Add(".obc");
            Extensions.Add(".blo");
            Extensions.Add(".cgd");
            Extensions.Add(".afsm");
            Extensions.Add(".acsb");
            Extensions.Add(".aiprop");
            Extensions.Add(".asb");
            Extensions.Add(".event_trackb");
            Extensions.Add(".seq");
            Extensions.Add(".skl");
            Extensions.Add(".af");
            Extensions.Add(".psmb");
            Extensions.Add(".bl");
            Extensions.Add(".epe");
        }

        public int Compare(string x, string y)
        {
            x = x.ToLowerInvariant();
            y = y.ToLowerInvariant();

            if (x == y)
            {
                return 0;
            }

            string xe = Path.GetExtension(x);
            string ye = Path.GetExtension(y);

            if (xe != null && ye != null)
            {
                int xi = Extensions.IndexOf(xe);
                int yi = Extensions.IndexOf(ye);

                if (xi >= 0 && yi >= 0 && xi != yi)
                {
                    return xi < yi ? -1 : 1;
                }
                else if (xi < 0)
                {
                    return 1;
                }
                else if (yi < 0)
                {
                    return -1;
                }
                else if (xe == ye)
                {
                    return string.Compare(x, y);
                }
                else
                {
                    return string.Compare(xe, ye);
                }
            }
            else if (xe == null && ye == null)
            {
                return string.Compare(x, y);
            }
            else if (xe != null)
            {
                return 1;
            }
            else if (ye != null)
            {
                return -1;
            }

            return 0;
        }
    }
}
