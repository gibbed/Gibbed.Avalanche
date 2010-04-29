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

            obj.ValuesByHash = new Dictionary<uint, IPropertyType>();
            var values = node.Select("value");
            while (values.MoveNext() == true)
            {
                var current = values.Current;

                uint id = GetIdOrName(current);
                string type = current.GetAttribute("type", "");

                IPropertyType value = PropertyHelpers.GetPropertyType(type, false);
                value.Parse(current.Value);
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
                    "write in big endian mode",
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

            var input = File.Open(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var doc = new XPathDocument(input);
            var nav = doc.CreateNavigator();

            var root = nav.SelectSingleNode("/root");
            string binExtension = root.GetAttribute("extension", "");
            string rawMode = root.GetAttribute("raw", "");

            bool raw = true;
            
            if (string.IsNullOrEmpty(rawMode) == false)
            {
                bool.TryParse(rawMode, out raw);
            }

            string binPath = extra.Count > 1 ?
                extra[1] : Path.ChangeExtension(xmlPath, binExtension == null ? ".bin" : binExtension);

            PropertyFile propertyFile = new PropertyFile();
            propertyFile.Raw = raw;

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
