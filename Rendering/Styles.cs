using System;

namespace Forge.UX.Rendering {
    [Flags]
    public enum Effects {
        None = 2 ^ 0,
        GrayScale = 2 ^ 1,
        Highlight = 2 ^ 2,
    }


    public enum Alignment {
        Left,
        Center,
        Right
    }

}
