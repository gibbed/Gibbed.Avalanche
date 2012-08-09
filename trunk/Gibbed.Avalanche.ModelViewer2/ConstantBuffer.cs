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
using System.Runtime.InteropServices;
using Gibbed.IO;
using SlimDX.Direct3D10;
using Buffer = SlimDX.Direct3D10.Buffer;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class ConstantBuffer<TStructure> : IDisposable
        where TStructure : struct
    {
        public Buffer Buffer;

        public ConstantBuffer(Device device)
        {
            var size = Marshal.SizeOf(typeof(TStructure));
            this.Buffer = new Buffer(device,
                                     size.Align(16),
                                     ResourceUsage.Dynamic,
                                     BindFlags.ConstantBuffer,
                                     CpuAccessFlags.Write,
                                     ResourceOptionFlags.None);
        }

        public void Update(TStructure structure)
        {
            GCHandle handle;
            int structureSize;
            byte[] buffer;

            // TODO: this is terrible

            structureSize = Marshal.SizeOf(typeof(TStructure));
            buffer = new byte[structureSize];
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
            handle.Free();

            using (var stream = this.Buffer.Map(MapMode.WriteDiscard,
                                                MapFlags.None))
            {
                stream.Write(buffer, 0, buffer.Length);
                this.Buffer.Unmap();
            }
        }

        public void Dispose()
        {
            if (this.Buffer != null)
            {
                this.Buffer.Dispose();
            }
        }
    }
}
