using System;

namespace Forge.UX.Rendering {
    [Flags]
    public enum Effects {
        None = 0,
        GrayScale = 1,
        Highlight = 1 << 1,
    }


    public enum Alignment {
        Left,
        Center,
        Right
    }

}
