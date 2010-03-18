using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public class Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public void Serialize(Stream output, bool littleEndian)
        {
            output.WriteValueF32(this.X, littleEndian);
            output.WriteValueF32(this.Y, littleEndian);
            output.WriteValueF32(this.Z, littleEndian);
        }

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.X = input.ReadValueF32(littleEndian);
            this.Y = input.ReadValueF32(littleEndian);
            this.Z = input.ReadValueF32(littleEndian);
        }
    }
}
