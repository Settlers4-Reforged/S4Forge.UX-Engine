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

    public enum TextAlignment {
        Left,
        Center,
        Right
    }

    public struct TextStyle {
        public TextType Type;
        public TextSize Size;
        public TextAlignment TextAlignment;
        public TextStyle(TextType type = TextType.Normal, TextSize size = TextSize.Regular, TextAlignment textAlignment = TextAlignment.Left) {
            Type = type;
            Size = size;
            TextAlignment = textAlignment;
        }
    }
}
