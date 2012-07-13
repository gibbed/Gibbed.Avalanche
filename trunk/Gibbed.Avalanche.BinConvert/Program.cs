﻿/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Gibbed.Avalanche.FileFormats;
using Gibbed.IO;
using NDesk.Options;

namespace Gibbed.Avalanche.BinConvert
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private static ProjectData.HashList<uint> _Names;

        public static void Main(string[] args)
        {
            var mode = Mode.Unknown;
            Endian? endian = null;
            bool showHelp = false;
            string currentProject = null;

            var options = new OptionSet
            {
                {
                    "e|export", "convert from binary to XML", v =>
                    {
                        if (v != null)
                        {
                            mode = Mode.Export;
                        }
                    }
                    },
                {
                    "i|import", "convert from XML to binary", v =>
                    {
                        if (v != null)
                        {
                            mode = Mode.Import;
                        }
                    }
                    },
                {
                    "l|little-endian", "write in little endian mode", v =>
                    {
                        if (v != null)
                        {
                            endian = Endian.Little;
                        }
                    }
                    },
                {
                    "b|big-endian", "write in big endian mode", v =>
                    {
                        if (v != null)
                        {
                            endian = Endian.Big;
                        }
                    }
                    },
                {
                    "p|project=", "override current project", v => currentProject = v
                    },
                {
                    "h|help", "show this message and exit", v => showHelp = v != null
                    },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            // detect!
            if (mode == Mode.Unknown &&
                extras.Count >= 1)
            {
                var extension = Path.GetExtension(extras[0]);

                if (extension != null &&
                    extension.ToLowerInvariant() == ".xml")
                {
                    mode = Mode.Import;
                }
                else
                {
                    mode = Mode.Export;
                }
            }

            if (extras.Count < 1 || extras.Count > 2 ||
                showHelp == true ||
                mode == Mode.Unknown)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ [-e] input_bin [output_xml]", GetExecutableName());
                Console.WriteLine("       {0} [OPTIONS]+ [-i] input_xml [output_bin]", GetExecutableName());
                Console.WriteLine("Convert a property file between binary and XML format.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var manager = ProjectData.Manager.Load(currentProject);
            if (manager.ActiveProject == null)
            {
                Console.WriteLine("Warning: no active project loaded.");
            }

            _Names = manager.LoadListsPropertyNames();

            if (mode == Mode.Export)
            {
                if (endian.HasValue == false)
                {
                    endian = manager.GetSetting("endian", Endian.Little);
                }

                string inputPath = extras[0];
                string outputPath = extras.Count > 1
                                        ? extras[1]
                                        : Path.ChangeExtension(inputPath, ".xml");

                var extension = Path.GetExtension(inputPath);

                IPropertyFile propertyFile;
                FileFormat fileFormat;

                using (var input = File.OpenRead(inputPath))
                {
                    input.Seek(0, SeekOrigin.Begin);

                    if (BlackboardPropertyFile.CheckMagic(input) == true)
                    {
                        fileFormat = FileFormat.Blackboard;

                        input.Seek(0, SeekOrigin.Begin);
                        propertyFile = new BlackboardPropertyFile()
                        {
                            Endian = endian.Value,
                        };
                        propertyFile.Deserialize(input);
                    }
                    else
                    {
                        fileFormat = FileFormat.Raw;

                        input.Seek(0, SeekOrigin.Begin);
                        propertyFile = new RawPropertyFile()
                        {
                            Endian = endian.Value,
                        };
                        propertyFile.Deserialize(input);
                    }
                }

                using (var output = File.Create(outputPath))
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "\t",
                        CheckCharacters = false,
                    };

                    var writer = XmlWriter.Create(output, settings);

                    writer.WriteStartDocument();
                    writer.WriteStartElement("container");

                    if (extension != null)
                    {
                        writer.WriteAttributeString("extension", extension);
                    }

                    writer.WriteAttributeString("format", fileFormat.ToString());
                    writer.WriteAttributeString("endian", endian.Value.ToString());

                    foreach (var node in propertyFile.Nodes)
                    {
                        writer.WriteStartElement("object");
                        WriteObject(writer, node);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Flush();
                }
            }
            else if (mode == Mode.Import)
            {
                string inputPath = extras[0];

                IPropertyFile propertyFile;
                string extension;

                using (var input = File.OpenRead(inputPath))
                {
                    var doc = new XPathDocument(input);
                    var nav = doc.CreateNavigator();

                    var root = nav.SelectSingleNode("/container");
                    if (root == null)
                    {
                        throw new FormatException();
                    }

                    extension = root.GetAttribute("extension", "");
                    if (string.IsNullOrEmpty(extension) == true)
                    {
                        extension = ".bin";
                    }

                    FileFormat fileFormat;
                    var formatAttribute = root.GetAttribute("format", "");
                    if (string.IsNullOrEmpty(formatAttribute) == false)
                    {
                        if (Enum.TryParse(formatAttribute, out fileFormat) == false)
                        {
                            throw new FormatException();
                        }
                    }
                    else
                    {
                        fileFormat = FileFormat.Raw;
                    }

                    var endianAttribute = root.GetAttribute("endian", "");
                    if (endian.HasValue == false &&
                        string.IsNullOrEmpty(endianAttribute) == false)
                    {
                        Endian fileEndian;
                        if (Enum.TryParse(endianAttribute, out fileEndian) == false)
                        {
                            throw new FormatException();
                        }

                        endian = fileEndian;
                    }
                    else
                    {
                        endian = manager.GetSetting("endian", Endian.Little);
                    }

                    switch (fileFormat)
                    {
                        case FileFormat.Raw:
                        {
                            propertyFile = new RawPropertyFile()
                            {
                                Endian = endian.Value,
                            };
                            break;
                        }

                        case FileFormat.Blackboard:
                        {
                            propertyFile = new BlackboardPropertyFile()
                            {
                                Endian = endian.Value,
                            };
                            break;
                        }

                        default:
                        {
                            throw new NotSupportedException();
                        }
                    }

                    var nodes = root.Select("object");
                    while (nodes.MoveNext() == true)
                    {
                        propertyFile.Nodes.Add(ParseObject(nodes.Current));
                    }
                }

                string outputPath = extras.Count > 1
                                        ? extras[1]
                                        : Path.ChangeExtension(inputPath, extension);

                using (var output = File.Create(outputPath))
                {
                    propertyFile.Serialize(output);
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static void WriteProperty(XmlWriter writer, FileFormats.Property.IVariant variant)
        {
            writer.WriteAttributeString("type", variant.Tag);
            writer.WriteValue(variant.Compose());
        }

        private static void WriteObject(XmlWriter writer, FileFormats.Property.Node node)
        {
            if (node.Tag != null)
            {
                writer.WriteAttributeString("tag", node.Tag);
            }

            // is this ridiculous?
            // yeeeeep.

            var nodesByName = node.Nodes
                .Where(kv => node.KnownNames.ContainsKey(kv.Key) == true)
                .Select(kv => new KeyValuePair<string, FileFormats.Property.Node>(node.KnownNames[kv.Key], kv.Value))
                .Concat(
                    node.Nodes
                        .Where(kv => node.KnownNames.ContainsKey(kv.Key) == false &&
                                     _Names.Contains(kv.Key) == true)
                        .Select(kv => new KeyValuePair<string, FileFormats.Property.Node>(_Names[kv.Key], kv.Value)))
                .ToArray();

            var nodesByHash = node.Nodes
                .Where(kv => node.KnownNames.ContainsKey(kv.Key) == false &&
                             _Names.Contains(kv.Key) == false)
                .Select(kv => kv)
                .ToArray();

            var variantsByName = node.Variants
                .Where(kv => node.KnownNames.ContainsKey(kv.Key) == true)
                .Select(kv => new KeyValuePair<string, FileFormats.Property.IVariant>(node.KnownNames[kv.Key], kv.Value))
                .Concat(
                    node.Variants
                        .Where(kv => node.KnownNames.ContainsKey(kv.Key) == false &&
                                     _Names.Contains(kv.Key) == true)
                        .Select(kv => new KeyValuePair<string, FileFormats.Property.IVariant>(_Names[kv.Key], kv.Value)))
                .ToArray();

            var variantsByHash = node.Variants
                .Where(kv => node.KnownNames.ContainsKey(kv.Key) == false &&
                             _Names.Contains(kv.Key) == false)
                .Select(kv => kv)
                .ToArray();

            if (variantsByName.Length > 0)
            {
                foreach (var kv in variantsByName.OrderBy(kv => kv.Key))
                {
                    writer.WriteStartElement("value");
                    writer.WriteAttributeString("name", kv.Key);
                    WriteProperty(writer, kv.Value);
                    writer.WriteEndElement();
                }
            }

            if (nodesByName.Length > 0)
            {
                foreach (var kv in nodesByName.OrderBy(kv => kv.Key))
                {
                    writer.WriteStartElement("object");
                    writer.WriteAttributeString("name", kv.Key);
                    WriteObject(writer, kv.Value);
                    writer.WriteEndElement();
                }
            }

            if (variantsByHash.Length > 0)
            {
                foreach (var kv in variantsByHash.OrderBy(p => p.Key, new NameComparer(_Names)))
                {
                    writer.WriteStartElement("value");

                    if (_Names.Contains(kv.Key) == true)
                    {
                        writer.WriteAttributeString("name", _Names[kv.Key]);
                    }
                    else
                    {
                        writer.WriteAttributeString("id", kv.Key.ToString("X8"));
                    }

                    WriteProperty(writer, kv.Value);
                    writer.WriteEndElement();
                }
            }

            if (nodesByHash.Length > 0)
            {
                foreach (var kv in nodesByHash.OrderBy(n => n.Key, new NameComparer(_Names)))
                {
                    writer.WriteStartElement("object");

                    if (_Names.Contains(kv.Key) == true)
                    {
                        writer.WriteAttributeString("name", _Names[kv.Key]);
                    }
                    else
                    {
                        writer.WriteAttributeString("id", kv.Key.ToString("X8"));
                    }

                    WriteObject(writer, kv.Value);
                    writer.WriteEndElement();
                }
            }
        }

        private static uint GetIdOrName(XPathNavigator node, out string name)
        {
            string id = node.GetAttribute("id", "");
            name = node.GetAttribute("name", "");

            if (string.IsNullOrEmpty(id) == false &&
                string.IsNullOrEmpty(name) == false)
            {
                if (uint.Parse(id, NumberStyles.AllowHexSpecifier) !=
                    name.HashJenkins())
                {
                    throw new InvalidOperationException("supplied id and name, but they don't match");
                }
            }
            else if (string.IsNullOrEmpty(id) == true &&
                     string.IsNullOrEmpty(name) == true)
            {
                throw new InvalidOperationException("did not supply id or name");
            }

            if (string.IsNullOrEmpty(id) == false)
            {
                return uint.Parse(id, NumberStyles.AllowHexSpecifier);
            }

            return name.HashJenkins();
        }

        private static FileFormats.Property.Node ParseObject(XPathNavigator nav)
        {
            var node = new FileFormats.Property.Node();

            if (nav.MoveToAttribute("tag", "") == true)
            {
                node.Tag = nav.Value;
                nav.MoveToParent();
            }

            var values = nav.Select("value");
            while (values.MoveNext() == true)
            {
                var current = values.Current;
                if (current == null)
                {
                    throw new InvalidOperationException();
                }

                string name;
                var id = GetIdOrName(current, out name);
                var type = current.GetAttribute("type", "");

                var variant = FileFormats.Property.VariantFactory.GetVariant(type);
                variant.Parse(current.Value);

                if (node.Variants.ContainsKey(id) == true)
                {
                    var lineInfo = (IXmlLineInfo)current;

                    if (string.IsNullOrEmpty(name) == true)
                    {
                        throw new FormatException(string.Format("duplicate value id 0x{0:X8} at {1}", id, lineInfo));
                    }

                    throw new FormatException(string.Format("duplicate value id 0x{0:X8} ('{1}') at {2}", id, name, lineInfo));
                }

                node.Variants.Add(id, variant);
            }

            var children = nav.Select("object");
            while (children.MoveNext() == true)
            {
                var child = children.Current;

                string name;
                uint id = GetIdOrName(child, out name);
                var obj = ParseObject(child);

                if (node.Nodes.ContainsKey(id) == true)
                {
                    var lineInfo = (IXmlLineInfo)child;

                    if (string.IsNullOrEmpty(name) == true)
                    {
                        throw new FormatException(string.Format("duplicate object id 0x{0:X8} at {1}", id, lineInfo));
                    }

                    throw new FormatException(string.Format("duplicate object id 0x{0:X8} ('{1}') at {2}", id, name, lineInfo));
                }

                node.Nodes.Add(id, obj);
            }

            return node;
        }
    }
}
