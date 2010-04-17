using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Avalanche.FileFormats.DataFormat
{
    public enum DefinitionType : uint
    {
        Structure = 1,
        Unknown2 = 2,
        Array = 3,
        Unknown4 = 4,
        Native = 5,
    }
}
