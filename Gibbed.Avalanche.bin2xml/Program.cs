using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Gibbed.Avalanche.FileFormats;
using Gibbed.Helpers;
using NDesk.Options;

namespace Gibbed.Avalanche.bin2xml
{
    internal class Program
    {
        private static Dictionary<uint, string> Names = new Dictionary<uint, string>();
        private static void LoadNamesFromPath(string path)
        {
            if (File.Exists(path))
            {
                TextReader reader = new StreamReader(path);

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (line.Length <= 0)
                    {
                        continue;
                    }

                    line = line.Trim();
                    uint hash = Path.GetFileName(line).HashJenkins();

                    if (Names.ContainsKey(hash) == true && Names[hash] != line)
                    {
                        throw new Exception();
                    }

                    Names[hash] = line;
                    //Names.Add(hash, line);
                }

                reader.Close();
            }
        }

        private static void WriteProperty(XmlWriter writer, object value)
        {
            if (value is int)
            {
                writer.WriteAttributeString("type", "int");
                writer.WriteValue((int)value);
            }
            else if (value is float)
            {
                writer.WriteAttributeString("type", "float");
                writer.WriteValue((float)value);
            }
            else if (value is string)
            {
                writer.WriteAttributeString("type", "string");
                writer.WriteValue((string)value);
            }
            else if (value is Vector2)
            {
                var vector = (Vector2)value;
                writer.WriteAttributeString("type", "vec2");
                writer.WriteValue(string.Format(
                    "{0},{1}",
                    vector.X, vector.Y));
            }
            else if (value is Vector3)
            {
                var vector = (Vector3)value;
                writer.WriteAttributeString("type", "vec");
                writer.WriteValue(string.Format(
                    "{0},{1},{2}",
                    vector.X, vector.Y, vector.Z));
            }
            else if (value is Vector4)
            {
                var vector = (Vector4)value;
                writer.WriteAttributeString("type", "vec4");
                writer.WriteValue(string.Format(
                    "{0},{1},{2},{3}",
                    vector.X, vector.Y, vector.Z, vector.W));
            }
            else if (value is Matrix)
            {
                var matrix = (Matrix)value;
                writer.WriteAttributeString("type", "mat");
                writer.WriteValue(string.Format(
                    "{0},{1},{2}, {3},{4},{5}, {6},{7},{8}, {9},{10},{11}",
                    matrix.A, matrix.B, matrix.C,
                    matrix.D, matrix.E, matrix.F,
                    matrix.G, matrix.H, matrix.I,
                    matrix.J, matrix.K, matrix.L));
            }
            else if (value is List<int>)
            {
                writer.WriteAttributeString("type", "vec_int");
                writer.WriteValue(((List<int>)value).Implode(","));
            }
            else if (value is List<float>)
            {
                writer.WriteAttributeString("type", "vec_float");
                writer.WriteValue(((List<float>)value).Implode(","));
            }
            else
            {
                throw new Exception();
            }
        }

        private static void WriteObject(XmlWriter writer, PropertyNode obj)
        {
            if (obj.Unknown3 != null)
            {
                writer.WriteStartElement("unknown3");
                writer.WriteBase64(obj.Unknown3, 0, obj.Unknown3.Length);
                writer.WriteEndElement();
            }

            if (obj.ValuesByName != null)
            {
                foreach (var kvp in obj.ValuesByName.OrderBy(p => p.Key))
                {
                    writer.WriteStartElement("value");
                    writer.WriteAttributeString("name", kvp.Key);
                    //writer.WriteAttributeString("hash", "false");
                    WriteProperty(writer, kvp.Value);
                    writer.WriteEndElement();
                }
            }

            if (obj.ValuesByHash != null)
            {
                foreach (var kvp in obj.ValuesByHash.OrderBy(p => p.Key, new NameComparer(Names)))
                {
                    writer.WriteStartElement("value");

                    if (Names.ContainsKey(kvp.Key) == true)
                    {
                        writer.WriteAttributeString("name", Names[kvp.Key]);
                    }
                    else
                    {
                        writer.WriteAttributeString("id", kvp.Key.ToString("X8"));
                    }

                    WriteProperty(writer, kvp.Value);
                    writer.WriteEndElement();
                }
            }

            if (obj.ChildrenByName != null)
            {
                foreach (var kvp in obj.ChildrenByName.OrderBy(c => c.Key))
                {
                    writer.WriteStartElement("object");
                    writer.WriteAttributeString("name", kvp.Key);
                    //writer.WriteAttributeString("hash", "false");
                    WriteObject(writer, kvp.Value);
                    writer.WriteEndElement();
                }
            }

            if (obj.ChildrenByHash != null)
            {
                foreach (var kvp in obj.ChildrenByHash.OrderBy(c => c.Key, new NameComparer(Names)))
                {
                    writer.WriteStartElement("object");

                    if (Names.ContainsKey(kvp.Key) == true)
                    {
                        writer.WriteAttributeString("name", Names[kvp.Key]);
                    }
                    else
                    {
                        writer.WriteAttributeString("id", kvp.Key.ToString("X8"));
                    }

                    WriteObject(writer, kvp.Value);
                    writer.WriteEndElement();
                }
            }
        }

        private static string GetExecutablePath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void Main(string[] args)
        {
            bool littleEndian = true;
            bool showHelp = false;

            OptionSet options = new OptionSet()
            {
                {
                    "b|bigendian",
                    "read in big endian mode",
                    v => littleEndian = v == null
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
            };

            List<string> extra;

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extra.Count < 1 || extra.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_bin [output_xml]", GetExecutableName());
                Console.WriteLine("Convert a binary property file to an xml property file.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string listsPath = Path.Combine(GetExecutablePath(), "lists");
            if (Directory.Exists(listsPath))
            {
                foreach (string listPath in Directory.GetFiles(listsPath, "*.namelist", SearchOption.AllDirectories))
                {
                    LoadNamesFromPath(listPath);
                }
            }

            string binPath = extra[0];
            string xmlPath = extra.Count > 1 ?
                extra[1] : Path.ChangeExtension(binPath, ".xml");

            var input = File.Open(binPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var propertyFile = new PropertyFile();
            propertyFile.Deserialize(input, littleEndian);
            input.Close();

            var output = File.Create(xmlPath);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter writer = XmlWriter.Create(output, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("root");
            foreach (var node in propertyFile.Nodes)
            {
                writer.WriteStartElement("object");
                WriteObject(writer, node);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            output.Close();
        }
    }
}
