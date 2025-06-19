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

    public struct TextStyle : IEquatable<TextStyle> {
        public TextStyleType Type;
        public TextStyleSize Size;
        public bool Wrapped;
        public bool Shadowed;
        public TextStyleAlignment HorizontalAlignment;
        public TextStyleAlignment VerticalAlignment;
        public Vector4 Color = new Vector4(255, 255, 255, 255);

        // TODO: custom font support

        public TextStyle() {
            Type = TextStyleType.Normal;
            Size = TextStyleSize.Regular;
            Wrapped = false;
            Shadowed = true;
            HorizontalAlignment = TextStyleAlignment.Start;
            VerticalAlignment = TextStyleAlignment.Start;
            Color = new Vector4(255, 255, 255, 255);
        }

        public TextStyle(TextStyleType type = TextStyleType.Normal, TextStyleSize size = TextStyleSize.Regular, bool wrapped = false, bool shadowed = true, TextStyleAlignment horizontalAlignment = TextStyleAlignment.Start, TextStyleAlignment verticalAlignment = TextStyleAlignment.Start, Vector4? color = null) {
            Type = type;
            Size = size;
            Wrapped = wrapped;
            Shadowed = shadowed;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            Color = color ?? new Vector4(255, 255, 255, 255);
        }

        public bool Equals(TextStyle other) {
            return Type == other.Type &&
                   Size == other.Size &&
                   Wrapped == other.Wrapped &&
                   Shadowed == other.Shadowed &&
                   HorizontalAlignment == other.HorizontalAlignment &&
                   VerticalAlignment == other.VerticalAlignment &&
                   Color.Equals(other.Color);
        }
    }
}
