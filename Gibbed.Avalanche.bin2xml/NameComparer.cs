using System;
using System.Collections.Generic;

namespace Gibbed.Avalanche.bin2xml
{
    internal class NameComparer : IComparer<uint>
    {
        private ProjectData.HashList<uint> Names;

        public NameComparer(ProjectData.HashList<uint> names)
        {
            this.Names = names;
        }

        public int Compare(uint x, uint y)
        {
            if (this.Names.Contains(x) == false)
            {
                if (this.Names.Contains(y) == false)
                {
                    if (x == y)
                    {
                        return 0;
                    }

                    return x < y ? -1 : 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (this.Names.Contains(y) == false)
                {
                    return 1;
                }
                else
                {
                    return String.Compare(this.Names[x], this.Names[y]);
                }
            }
        }
    }
}
