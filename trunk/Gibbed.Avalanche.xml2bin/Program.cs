using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.XPath;
using Gibbed.Avalanche.FileFormats;
using NDesk.Options;

namespace Gibbed.Avalanche.xml2bin
{
    internal class Program
    {
        private static uint GetIdOrName(XPathNavigator node)
        {
            string id = node.GetAttribute("id", "");
            string name = node.GetAttribute("name", "");

            if (string.IsNullOrEmpty(id) == false &&
                string.IsNullOrEmpty(name) == false)
            {
                if (uint.Parse(id, NumberStyles.AllowHexSpecifier) !=
                    name.HashJenkins())
                {
                    throw new InvalidOperationException("supplied id and name, but they don't match");
                }
            }
            else if (
                string.IsNullOrEmpty(id) == true &&
                string.IsNullOrEmpty(name) == true)
            {
                throw new InvalidOperationException("did not supply id or name");
            }

            if (string.IsNullOrEmpty(id) == false)
            {
                return uint.Parse(id, NumberStyles.AllowHexSpecifier);
            }
            else
            {
                return name.HashJenkins();
            }
        }

        private static PropertyNode ParseObject(XPathNavigator node)
        {
            var obj = new PropertyNode();

            var unknown3 = node.SelectSingleNode("unknown3");
            if (unknown3 != null)
            {
                obj.Unknown3 = Convert.FromBase64String(unknown3.Value);
            }

            obj.ValuesByHash = new Dictionary<uint, object>();
            var values = node.Select("value");
            while (values.MoveNext() == true)
            {
                var current = values.Current;

                uint id = GetIdOrName(current);
                object value;

                string type = current.GetAttribute("type", "");
                switch (type)
                {
                    case "int": value = current.ValueAsInt; break;
                    case "float": value = current.ValueAs(typeof(float)); break;
                    case "string": value = current.Value; break;
                    case "vec2":
                    {
                        string[] parts = current.Value.Split(',');
                        if (parts.Length != 2)
                        {
                            throw new InvalidOperationException("vec2 requires two float values delimited by a comma");
                        }
                        Vector2 vector = new Vector2();
                        vector.X = float.Parse(parts[0]);
                        vector.Y = float.Parse(parts[1]);
                        value = vector;
                        break;
                    }
                    case "vec":
                    {
                        string[] parts = current.Value.Split(',');
                        if (parts.Length != 3)
                        {
                            throw new InvalidOperationException("vec requires two float values delimited by a comma");
                        }
                        Vector3 vector = new Vector3();
                        vector.X = float.Parse(parts[0]);
                        vector.Y = float.Parse(parts[1]);
                        vector.Z = float.Parse(parts[2]);
                        value = vector;
                        break;
                    }
                    case "vec4":
                    {
                        string[] parts = current.Value.Split(',');
                        if (parts.Length != 4)
                        {
                            throw new InvalidOperationException("vec4 requires three float values delimited by a comma");
                        }
                        Vector4 vector = new Vector4();
                        vector.X = float.Parse(parts[0]);
                        vector.Y = float.Parse(parts[1]);
                        vector.Z = float.Parse(parts[2]);
                        vector.W = float.Parse(parts[3]);
                        value = vector;
                        break;
                    }
                    case "mat":
                    {
                        string[] parts = current.Value.Split(',');
                        if (parts.Length != 3 * 4)
                        {
                            throw new InvalidOperationException("mat requires twelve float values delimited by a comma");
                        }

                        Matrix matrix = new Matrix();
                        matrix.A = float.Parse(parts[0]);
                        matrix.B = float.Parse(parts[1]);
                        matrix.C = float.Parse(parts[2]);
                        matrix.D = float.Parse(parts[3]);
                        matrix.E = float.Parse(parts[4]);
                        matrix.F = float.Parse(parts[5]);
                        matrix.G = float.Parse(parts[6]);
                        matrix.H = float.Parse(parts[7]);
                        matrix.I = float.Parse(parts[8]);
                        matrix.J = float.Parse(parts[9]);
                        matrix.K = float.Parse(parts[10]);
                        matrix.L = float.Parse(parts[11]);
                        value = matrix;
                        break;

                    }
                    default: throw new InvalidOperationException("unsupported type " + type.ToString());
                }

                obj.ValuesByHash.Add(id, value);
            }

            obj.ChildrenByHash = new Dictionary<uint, PropertyNode>();
            var children = node.Select("object");
            while (children.MoveNext() == true)
            {
                var child = children.Current;
                uint id = GetIdOrName(child);
                obj.ChildrenByHash.Add(id, ParseObject(child));
            }

            return obj;
        }

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
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

            string xmlPath = extra[0];
            string binPath = extra.Count > 1 ?
                extra[1] : Path.ChangeExtension(xmlPath, ".bin");

            var input = File.Open(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var doc = new XPathDocument(input);
            var nav = doc.CreateNavigator();

            PropertyFile propertyFile = new PropertyFile();

            var nodes = nav.Select("/root/object");
            while (nodes.MoveNext() == true)
            {
                propertyFile.Nodes.Add(ParseObject(nodes.Current));
            }

            input.Close();

            var output = File.Create(binPath);
            propertyFile.Serialize(output, littleEndian);
            output.Close();
        }
    }
}
