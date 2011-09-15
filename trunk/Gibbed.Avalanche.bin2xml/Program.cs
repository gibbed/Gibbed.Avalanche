using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Gibbed.Avalanche.FileFormats;
using Gibbed.IO;
using NDesk.Options;

namespace Gibbed.Avalanche.bin2xml
{
    internal class Program
    {
        private static Dictionary<uint, string> Names = new Dictionary<uint, string>();

        private static void WriteProperty(XmlWriter writer, IPropertyType value)
        {
            writer.WriteAttributeString("type", value.Tag);
            writer.WriteValue(value.Compose());
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

            var manager = Setup.Manager.Load();
            if (manager.ActiveProject != null)
            {
                manager.ActiveProject.Load();
                Names = manager.ActiveProject.NameHashLookup;
            }
            else
            {
                Console.WriteLine("Warning: no active project loaded.");
            }

            string binPath = extra[0];
            string xmlPath = extra.Count > 1 ?
                extra[1] : Path.ChangeExtension(binPath, ".xml");

            string binExtension = Path.GetExtension(binPath);

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

            if (binExtension != null)
            {
                writer.WriteAttributeString("extension", binExtension);
            }

            if (propertyFile.Raw != true)
            {
                writer.WriteAttributeString("raw", propertyFile.Raw.ToString());
            }

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
