using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Forge.UX.S4.Types {

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct GUIEventHandler {
        public Int32 __vftable /*VFT*/;
        public Int16 field_4;
        public Byte gap_6_1;
        public Byte gap_6_2;
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
        public Byte unknownSurfaceData;
        public Int16 field_41;
    }
}
