using System;
using System.IO;
using System.Text;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public static class StreamHelpers
    {
        public static string ReadStringASCIIUInt32(this Stream input)
        {
            return input.ReadStringASCIIUInt32(true);
        }

        public static string ReadStringASCIIUInt32(this Stream input, bool littleEndian)
        {
            uint length = input.ReadValueU32(littleEndian);
            if (length >= 0x7FFF)
            {
                throw new Exception();
            }
            return input.ReadStringASCII(length);
        }

        public static void WriteStringASCIIUInt32(this Stream output, string value)
        {
            output.WriteStringASCIIUInt32(value, true);
        }

        public static void WriteStringASCIIUInt32(this Stream output, string value, bool littleEndian)
        {
            output.WriteValueS32(value.Length);
            output.WriteStringASCII(value);
        }
    }
}
