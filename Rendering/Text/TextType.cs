using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Rendering.Text {
    public enum TextType {
        Normal,
        Bold,
        Thin,
        Italic
    }


    public enum TextSize {
        Small,
        Regular,
        Large
    }

    public struct TextStyle {
        private TextType type;
        private TextSize size;
        private Alignment alignment;
    }
}
