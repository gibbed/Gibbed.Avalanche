using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gibbed.Avalanche.FileFormats.Model
{
    public interface IRenderBlock
    {
        void Serialize(Stream output);
        void Deserialize(Stream input);
    }
}
