using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Avalanche.FileFormats
{
    public static class PropertyHelpers
    {
        /* 1 - int
         * 2 - float
         * 3 - string
         * 4 - vec2
         * 5 - vec
         * 6 - vec4
         * 7 - mat3x3
         * 8 - mat
         * 9 - vec_int
         * 10 - vec_float */

        public static IPropertyType GetPropertyType(uint type)
        {
            switch (type)
            {
                case 1: return new PropertyTypes.IntegerProperty();
                case 2: return new PropertyTypes.FloatProperty();
                case 3: return new PropertyTypes.StringProperty();
                case 4: return new PropertyTypes.Vector2Property();
                case 5: return new PropertyTypes.Vector3Property();
                case 6: return new PropertyTypes.Vector4Property();
                case 8: return new PropertyTypes.Matrix4x3Property();
                case 9: return new PropertyTypes.IntegerListProperty();
                case 10: return new PropertyTypes.FloatListProperty();
                default: throw new NotSupportedException("unsupported property type");
            }
        }

        public static IPropertyType GetPropertyType(string type)
        {
            switch (type)
            {
                case "int": return new PropertyTypes.IntegerProperty();
                case "float": return new PropertyTypes.FloatProperty();
                case "string": return new PropertyTypes.StringProperty();
                case "vec2": return new PropertyTypes.Vector2Property();
                case "vec": return new PropertyTypes.Vector3Property();
                case "vec4": return new PropertyTypes.Vector4Property();
                case "mat": return new PropertyTypes.Matrix4x3Property();
                case "vec_int": return new PropertyTypes.IntegerListProperty();
                case "vec_float": return new PropertyTypes.FloatListProperty();
                default: throw new NotSupportedException("unsupported property type");
            }
        }

        /*
        public static object ReadTypedValueTagged(Stream input, uint type, bool littleEndian)
        {
            switch (type)
            {
                case 1: return input.ReadValueS32(littleEndian);
                case 2: return input.ReadValueF32(littleEndian);
                case 3:
                {
                    uint offset = input.ReadValueU32(littleEndian);
                    if (offset == 0xFFFFFFFF)
                    {
                        return "";
                    }
                    else
                    {
                        input.Seek(offset, SeekOrigin.Begin);
                        return input.ReadStringASCIIZ();
                    }
                }
                case 5:
                {
                    uint offset = input.ReadValueU32(littleEndian);
                    if (offset == 0xFFFFFFFF)
                    {
                        return new Vector3();
                    }
                    else
                    {
                        input.Seek(offset, SeekOrigin.Begin);
                        Vector3 vector = new Vector3();
                        vector.Deserialize(input, littleEndian);
                        return vector;
                    }
                }
                default: throw new Exception("unhandled type " + type.ToString());
            }
        }

        public static object ReadTypedValueRaw(Stream input, bool littleEndian)
        {
            byte type = input.ReadValueU8();

            switch (type)
            {
                case 1: return input.ReadValueS32(littleEndian);
                case 2: return input.ReadValueF32(littleEndian);
                case 3:
                {
                    ushort length = input.ReadValueU16(littleEndian);
                    return input.ReadStringASCII(length, true);
                }
                case 4:
                {
                    var vector = new Vector2();
                    vector.Deserialize(input, littleEndian);
                    return vector;
                }
                case 5:
                {
                    var vector = new Vector3();
                    vector.Deserialize(input, littleEndian);
                    return vector;
                }
                case 6:
                {
                    var vector = new Vector4();
                    vector.Deserialize(input, littleEndian);
                    return vector;
                }
                case 7: throw new FormatException("unhandled mat3x3");
                case 8:
                {
                    var matrix = new Matrix();
                    matrix.Deserialize(input, littleEndian);
                    return matrix;
                }
                case 9:
                {
                    int count = input.ReadValueS32(littleEndian);
                    var values = new List<int>();
                    for (int i = 0; i < count; i++)
                    {
                        values.Add(input.ReadValueS32(littleEndian));
                    }
                    return values;
                }
                case 10:
                {
                    int count = input.ReadValueS32(littleEndian);
                    var values = new List<float>();
                    for (int i = 0; i < count; i++)
                    {
                        values.Add(input.ReadValueF32(littleEndian));
                    }
                    return values;
                }
                default: throw new Exception("unhandled type " + type.ToString());
            }
        }

        public static void WriteTypedValueRaw(Stream output, object value, bool littleEndian)
        {
            if (value is int)
            {
                output.WriteValueU8(1);
                output.WriteValueS32((int)value, littleEndian);
            }
            else if (value is float)
            {
                output.WriteValueU8(2);
                output.WriteValueF32((float)value, littleEndian);
            }
            else if (value is string)
            {
                output.WriteValueU8(3);
                string s = (string)value;
                output.WriteValueU16((ushort)(s.Length & 0xFFFF), littleEndian);
                output.WriteStringASCII(s.Substring(0, Math.Min(s.Length, 0xFFFF)));
            }
            else if (value is Vector2)
            {
                output.WriteValueU8(4);
                ((Vector2)value).Serialize(output, littleEndian);
            }
            else if (value is Vector3)
            {
                output.WriteValueU8(5);
                ((Vector3)value).Serialize(output, littleEndian);
            }
            else if (value is Vector4)
            {
                output.WriteValueU8(6);
                ((Vector4)value).Serialize(output, littleEndian);
            }
            else if (value is Matrix)
            {
                output.WriteValueU8(8);
                ((Matrix)value).Serialize(output, littleEndian);
            }
            else if (value is List<int>)
            {
                output.WriteValueU8(9);
                var values = (List<int>)value;
                output.WriteValueS32(values.Count, littleEndian);
                foreach (int v in values)
                {
                    output.WriteValueS32(v, littleEndian);
                }
            }
            else if (value is List<float>)
            {
                output.WriteValueU8(10);
                var values = (List<float>)value;
                output.WriteValueS32(values.Count, littleEndian);
                foreach (int v in values)
                {
                    output.WriteValueF32(v, littleEndian);
                }
            }
            else
            {
                throw new Exception("unhandled type " + value.GetType().ToString());
            }
        }
        */
    }
}
