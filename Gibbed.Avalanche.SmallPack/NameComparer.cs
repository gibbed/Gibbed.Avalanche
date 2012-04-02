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

using System.Collections.Generic;
using System.IO;

namespace Gibbed.Avalanche.SmallPack
{
    internal class NameComparer : IComparer<string>
    {
        private static readonly List<string> Extensions;
        static NameComparer()
        {
            Extensions = new List<string>()
            {
                //".dds"
                //".rbm"
                //".lod"
                //".pal"
                //".pfx"
                //".dec"
                ".bsk",
                ".ban",
                ".pfs",
                ".dds",
                ".agui",
                ".rbm",
                ".batchlod",
                ".lod",
                ".pal",
                ".cdoll",
                ".pfx",
                ".dec",
                ".vdoll",
                ".mvdoll",
                ".sst",
                ".aiteam",
                ".pfi",
                ".dcs",
                ".obc",
                ".blo",
                ".cgd",
                ".afsm",
                ".acsb",
                ".aiprop",
                ".asb",
                ".event_trackb",
                ".seq",
                ".skl",
                ".af",
                ".psmb",
                ".bl",
                ".epe"
            };
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

                if (xi < 0)
                {
                    return 1;
                }

                if (yi < 0)
                {
                    return -1;
                }

                if (xe == ye)
                {
                    return string.CompareOrdinal(x, y);
                }

                return string.CompareOrdinal(xe, ye);
            }

            if (xe == null && ye == null)
            {
                return string.CompareOrdinal(x, y);
            }

            if (xe != null)
            {
                return 1;
            }

            //if (ye != null)
            {
                return -1;
            }

            //return 0;
        }
    }
}
