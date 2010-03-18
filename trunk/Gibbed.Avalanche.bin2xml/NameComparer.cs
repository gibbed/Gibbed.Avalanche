using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Avalanche.bin2xml
{
    internal class NameComparer : IComparer<uint>
    {
        private Dictionary<uint, string> Names;

        public NameComparer(Dictionary<uint, string> names)
        {
            this.Names = names;
        }

        public int Compare(uint x, uint y)
        {
            if (this.Names.ContainsKey(x) == false)
            {
                if (this.Names.ContainsKey(y) == false)
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
                if (this.Names.ContainsKey(y) == false)
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
