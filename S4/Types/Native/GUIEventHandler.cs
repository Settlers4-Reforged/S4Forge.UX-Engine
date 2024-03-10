using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Forge.UX.S4.Types.Native {

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GUIEventHandler {
        public int __vftable /*VFT*/;
        public short field_4;
        public byte gap_6_1;
        public byte gap_6_2;
        public int field_8;
        public int field_C;
        public int field_10;
        public int field_14;
        public int field_18;
        public int field_1C;
        public int ShowItemCount;
        public int ShowMinimap;
        public int ChatMainSurface;
        public int field_2C;
        public int field_30;
        public int ChatShowButtonSurface;
        public int field_38;
        public int TimeWindow;
        public byte unknownSurfaceData;
        public short field_41;
    }
}
