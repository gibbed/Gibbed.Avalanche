using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public class Matrix
    {
        public float A;
        public float B;
        public float C;
        
        public float D;
        public float E;
        public float F;

        public float G;
        public float H;
        public float I;

        public float J;
        public float K;
        public float L;

        public void Serialize(Stream output, bool littleEndian)
        {
            output.WriteValueF32(this.A, littleEndian);
            output.WriteValueF32(this.B, littleEndian);
            output.WriteValueF32(this.C, littleEndian);
            output.WriteValueF32(this.D, littleEndian);
            output.WriteValueF32(this.E, littleEndian);
            output.WriteValueF32(this.F, littleEndian);
            output.WriteValueF32(this.G, littleEndian);
            output.WriteValueF32(this.H, littleEndian);
            output.WriteValueF32(this.I, littleEndian);
            output.WriteValueF32(this.J, littleEndian);
            output.WriteValueF32(this.K, littleEndian);
            output.WriteValueF32(this.L, littleEndian);
        }

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.A = input.ReadValueF32(littleEndian);
            this.B = input.ReadValueF32(littleEndian);
            this.C = input.ReadValueF32(littleEndian);
            this.D = input.ReadValueF32(littleEndian);
            this.E = input.ReadValueF32(littleEndian);
            this.F = input.ReadValueF32(littleEndian);
            this.G = input.ReadValueF32(littleEndian);
            this.H = input.ReadValueF32(littleEndian);
            this.I = input.ReadValueF32(littleEndian);
            this.J = input.ReadValueF32(littleEndian);
            this.K = input.ReadValueF32(littleEndian);
            this.L = input.ReadValueF32(littleEndian);
        }
    }
}
