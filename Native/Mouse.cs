using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Forge.UX.Native {
    internal static class Mouse {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Vector2 lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Vector2 lpPoint);
    }
}
