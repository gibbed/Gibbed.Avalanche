using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gibbed.Avalanche.FileFormats
{
    public interface IPropertyType
    {
        byte Id { get; }
        string Tag { get; }
        bool Inline { get; }

        void Default();

        void Serialize(Stream output, bool raw, bool littleEndian);
        void Deserialize(Stream input, bool raw, bool littleEndian);

        void Parse(string text);
        string Compose();
    }
}
