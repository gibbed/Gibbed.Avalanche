using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Avalanche.FileFormats.DataFormat
{
    public enum NativeValueType : uint
    {
        UInt8 = 0x0CA2821D,
        Int8 = 0x580D0A62,
        UInt16 = 0x86D152BD,
        Int16 = 0xD13FCF93,
        UInt32 = 0x075E4E4F,
        Int32 = 0x192FE633,
        Float = 0x7515A207,
        String = 0x34B4DBF8,
    }
}
