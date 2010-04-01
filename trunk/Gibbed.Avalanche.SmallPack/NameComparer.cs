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
            Extensions.Add(".dds");
            Extensions.Add(".rbm");
            Extensions.Add(".lod");
            Extensions.Add(".pal");
            Extensions.Add(".pfx");
            Extensions.Add(".dec");
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
