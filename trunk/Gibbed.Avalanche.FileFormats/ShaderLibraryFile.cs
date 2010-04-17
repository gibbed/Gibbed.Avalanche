using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gibbed.Avalanche.FileFormats
{
    public class ShaderLibraryFile
    {
        public class Shader
        {
            public uint NameHash;
            public string Name;
            public uint DataHash;
            public byte[] Data;

            public override string ToString()
            {
                return string.IsNullOrEmpty(this.Name) == true ? base.ToString() : this.Name;
            }
        }

        public string Name;
        public string BuildTime;
        public List<Shader> VertexShaders = new List<Shader>();
        public List<Shader> FragmentShaders = new List<Shader>();

        public void Deserialize(Stream input)
        {
            var libraryFormat = new DataFormatFile();
            using (var libraryFormatData = new MemoryStream(ShaderLibraryFormat.Data))
            {
                libraryFormat.Deserialize(libraryFormatData);
                libraryFormatData.Close();
            }

            var data = new DataFormatFile();
            data.Deserialize(input);

            var root = data.FindEntry(0x1002D37C);
            if (root == null)
            {
                throw new InvalidOperationException("ShaderLibrary entry not present in shader library file");
            }

            // hack :)
            libraryFormat.LittleEndian = data.LittleEndian;

            object library = libraryFormat.ParseEntry(root, input);
            try
            {
                var structure = (DataFormat.Structure)library;
                this.Name = (string)structure.Values["Name"];
                this.BuildTime = (string)structure.Values["BuildTime"];
                
                this.VertexShaders.Clear();
                foreach (var element in (List<object>)structure.Values["VertexShaders"])
                {
                    var shaderData = (DataFormat.Structure)element;
                    Shader shader = new Shader();
                    shader.NameHash = (uint)shaderData.Values["NameHash"];
                    shader.Name = (string)shaderData.Values["Name"];
                    shader.DataHash = (uint)shaderData.Values["DataHash"];
                    shader.Data = (byte[])shaderData.Values["BinaryData"];
                    this.VertexShaders.Add(shader);
                }

                this.FragmentShaders.Clear();
                foreach (var element in (List<object>)structure.Values["FragmentShaders"])
                {
                    var shaderData = (DataFormat.Structure)element;
                    Shader shader = new Shader();
                    shader.NameHash = (uint)shaderData.Values["NameHash"];
                    shader.Name = (string)shaderData.Values["Name"];
                    shader.DataHash = (uint)shaderData.Values["DataHash"];
                    shader.Data = (byte[])shaderData.Values["BinaryData"];
                    this.FragmentShaders.Add(shader);
                }

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("something was missing from the parsed data", e);
            }
        }
    }
}
