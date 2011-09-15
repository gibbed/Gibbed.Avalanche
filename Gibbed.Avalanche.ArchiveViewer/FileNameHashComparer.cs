using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Avalanche.ArchiveViewer
{
    internal class FileNameHashComparer : IComparer<uint>
    {
        private ProjectData.HashList<uint> FileNames;

        public FileNameHashComparer(ProjectData.HashList<uint> names)
        {
            this.FileNames = names;
        }

        public int Compare(uint x, uint y)
        {
            if (this.FileNames == null || this.FileNames.Contains(x) == false)
            {
                if (this.FileNames == null || this.FileNames.Contains(y) == false)
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
                if (this.FileNames == null || this.FileNames.Contains(y) == false)
                {
                    return 1;
                }
                else
                {
                    return String.Compare(this.FileNames[x], this.FileNames[y]);
                }
            }
        }
    }
}
