using System;
using Gibbed.IO;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Avalanche.FileFormats;

namespace Gibbed.Avalanche.RenderBlockModel
{
    public struct Material : IFormat
    {
        public string DiffuseTexture;
        public string NormalMap;
        public string PropertiesMap;
        public string NormalMapEx0;
        public string NormalMapEx1;
        public string NormalMapEx2;
        public string NormalMapEx3;
        public string ShadowMapTexture;
        public uint Unknown8;

        public void Serialize(Stream output, Endian endian)
        {
            output.WriteStringU32(this.DiffuseTexture, endian);
            output.WriteStringU32(this.NormalMap, endian);
            output.WriteStringU32(this.PropertiesMap, endian);
            output.WriteStringU32(this.NormalMapEx0, endian);
            output.WriteStringU32(this.NormalMapEx1, endian);
            output.WriteStringU32(this.NormalMapEx2, endian);
            output.WriteStringU32(this.NormalMapEx3, endian);
            output.WriteStringU32(this.ShadowMapTexture, endian);
            output.WriteValueU32(this.Unknown8, endian);
        }

        public void Deserialize(Stream input, Endian endian)
        {
            this.DiffuseTexture = input.ReadStringU32(endian);
            this.NormalMap = input.ReadStringU32(endian);
            this.PropertiesMap = input.ReadStringU32(endian);
            this.NormalMapEx0 = input.ReadStringU32(endian);
            this.NormalMapEx1 = input.ReadStringU32(endian);
            this.NormalMapEx2 = input.ReadStringU32(endian);
            this.NormalMapEx3 = input.ReadStringU32(endian);
            this.ShadowMapTexture = input.ReadStringU32(endian);
            this.Unknown8 = input.ReadValueU32(endian);
        }
    }
}
