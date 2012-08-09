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

namespace Gibbed.Avalanche.ModelViewer2
{
    internal static class Native
    {
        public static bool IsWindowActive(IntPtr handle)
        {
            var active = GetActiveWindow();
            
            while (active != IntPtr.Zero)
            {
                if (active == handle)
                {
                    return true;
                }

                active = GetParent(active);
            }

            return false;
        }

        public static class VirtualKeys
        {
            public const int LeftShift = 160;
            public const int RightShift = 161;
        }

        public const int WM_KEYDOWN = 256;
        public const int WM_SYSKEYDOWN = 260;

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern ushort GetKeyState(int nVirtKey);

        public static bool IsKeyDown(int virtualKey)
        {
            return (GetKeyState(virtualKey) & 0x8000u) != 0;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int idThread);
        
        [DllImport("user32.dll")]
        public static extern int MapVirtualKeyEx(int uCode, int uMapType, IntPtr dwhkl);
    }
}
