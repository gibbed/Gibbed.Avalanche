﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Avalanche.FileFormats.Model
{
    public static class BlockTypes
    {
        private static Dictionary<uint, Type> Types;
        
        private static void Register<TBlockType>(string name)
            where TBlockType: IRenderBlock
        {
            if (Types == null)
            {
                Types = new Dictionary<uint, Type>();
            }

            Types.Add(name.HashJenkins(), typeof(TBlockType));
        }

        public static IRenderBlock GetBlock(string type)
        {
            return GetBlock(type.HashJenkins());
        }

        public static IRenderBlock GetBlock(uint type)
        {
            if (Types.ContainsKey(type) == false)
            {
                return null;
                throw new Exception("undefined render block type " + type.ToString("X8"));
            }

            return Activator.CreateInstance(Types[type]) as IRenderBlock;
        }

        static BlockTypes()
        {
            Register<SkinnedGeneral>("SkinnedGeneral");
        }
    }
}
