using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using D3D10 = SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class ConstantBuffer<TStructure> : IDisposable
        where TStructure: struct
    {
        public D3D10.Buffer Buffer;

        public ConstantBuffer(D3D10.Device device)
        {
            this.Buffer = new D3D10.Buffer(
                device,
                Marshal.SizeOf(typeof(TStructure)),
                D3D10.ResourceUsage.Dynamic,
                D3D10.BindFlags.ConstantBuffer,
                D3D10.CpuAccessFlags.Write,
                D3D10.ResourceOptionFlags.None);
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

            SlimDX.DataStream stream = this.Buffer.Map(
                D3D10.MapMode.WriteDiscard, D3D10.MapFlags.None);
            stream.Write(buffer, 0, buffer.Length);
            this.Buffer.Unmap();
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
