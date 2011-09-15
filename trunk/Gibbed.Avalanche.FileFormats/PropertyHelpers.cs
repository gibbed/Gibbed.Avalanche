using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

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

        public static IPropertyType GetPropertyType(uint type, bool raw)
        {
            if (raw == false && type == 8)
            {
                return new PropertyTypes.Matrix4x4Property();
            }

            switch (type)
            {
                case 1: return new PropertyTypes.IntegerProperty();
                case 2: return new PropertyTypes.FloatProperty();
                case 3: return new PropertyTypes.StringProperty();
                case 4: return new PropertyTypes.Vector2Property();
                case 5: return new PropertyTypes.Vector3Property();
                case 6: return new PropertyTypes.Vector4Property();
                case 7: return new PropertyTypes.Matrix3x3Property();
                case 8: return new PropertyTypes.Matrix4x3Property();
                case 9: return new PropertyTypes.IntegerListProperty();
                case 10: return new PropertyTypes.FloatListProperty();
                default: throw new NotSupportedException("unsupported property type");
            }
        }

        public static IPropertyType GetPropertyType(string type, bool raw)
        {
            switch (type)
            {
                case "int": return new PropertyTypes.IntegerProperty();
                case "float": return new PropertyTypes.FloatProperty();
                case "string": return new PropertyTypes.StringProperty();
                case "vec2": return new PropertyTypes.Vector2Property();
                case "vec": return new PropertyTypes.Vector3Property();
                case "vec4": return new PropertyTypes.Vector4Property();
                case "mat3x3": return new PropertyTypes.Matrix3x3Property();
                case "mat": return new PropertyTypes.Matrix4x3Property();
                case "vec_int": return new PropertyTypes.IntegerListProperty();
                case "vec_float": return new PropertyTypes.FloatListProperty();
                default: throw new NotSupportedException("unsupported property type");
            }
        }
    }
}
