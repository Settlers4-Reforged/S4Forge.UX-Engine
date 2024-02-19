using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Elements {

    [Flags]
    public enum PositioningMode {
        Normal = 0,
        /// <summary>Absolute in screen coordinates</summary>
        Absolute = 1 << 0,
        /// <summary>relative to the parent group size - all values should be between 0..1</summary>
        Relative = 1 << 1,
        /// <summary>relative to the screen - all values should be between 0..1</summary>
        AbsoluteRelative = Absolute | Relative,
    }
}
