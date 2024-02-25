using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Rendering.Text {
    public enum TextStyleType {
        Normal,
        Bold,
        Thin,
        Italic
    }

    public enum TextStyleSize {
        Small,
        Regular,
        Large
    }

    public enum TextStyleAlignment {
        Left,
        Center,
        Right
    }

    public struct TextStyle {
        public TextStyleType Type;
        public TextStyleSize Size;
        public TextStyleAlignment Alignment;
        public TextStyle(TextStyleType type = TextStyleType.Normal, TextStyleSize size = TextStyleSize.Regular, TextStyleAlignment alignment = TextStyleAlignment.Left) {
            Type = type;
            Size = size;
            Alignment = alignment;
        }
    }
}
