/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
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
using Gibbed.Avalanche.FileFormats;

namespace Gibbed.Avalanche.RenderBlockModel
{
    public static class BlockTypeFactory
    {
        private static Dictionary<uint, Type> _Types;

        private static void Register<TBlockType>(string name)
            where TBlockType : IRenderBlock
        {
            if (_Types == null)
            {
                _Types = new Dictionary<uint, Type>();
            }

            _Types.Add(name.HashJenkins(), typeof(TBlockType));
        }

        public static IRenderBlock GetBlock(string type)
        {
            return GetBlock(type.HashJenkins());
        }

        public static IRenderBlock GetBlock(uint type)
        {
            if (_Types.ContainsKey(type) == false)
            {
                return null;
                throw new Exception("undefined render block type " + type.ToString("X8"));
            }

            return Activator.CreateInstance(_Types[type]) as IRenderBlock;
        }

        static BlockTypeFactory()
        {
            Register<Blocks.CarPaint>("CarPaint");
            Register<Blocks.CarPaintSimple>("CarPaintSimple");
            Register<Blocks.DeformableWindow>("DeformableWindow");
            Register<Blocks.General>("General");
            Register<Blocks.SkinnedGeneral>("SkinnedGeneral");
        }
    }
}
