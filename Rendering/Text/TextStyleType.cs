using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Rendering.Text {
    public enum TextStyleType {
        Normal,
        Bold,
        Italic
    }

    public enum TextStyleSize {
        Small,
        Regular,
        Large
    }

    public enum TextStyleAlignment {
        Start,
        Center,
        End
    }

    public struct TextStyle {
        public TextStyleType Type;
        public TextStyleSize Size;
        public TextStyleAlignment HorizontalAlignment;
        public TextStyleAlignment VerticalAlignment;
        public Vector4 Color = new Vector4(255, 255, 255, 255);

        public TextStyle(TextStyleType type = TextStyleType.Normal, TextStyleSize size = TextStyleSize.Regular, TextStyleAlignment horizontalAlignment = TextStyleAlignment.Start, TextStyleAlignment verticalAlignment = TextStyleAlignment.Start, Vector4? color = null) {
            Type = type;
            Size = size;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            Color = color ?? new Vector4(255, 255, 255, 255);
        }
    }
}
