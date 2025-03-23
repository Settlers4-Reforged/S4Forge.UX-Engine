using Forge.UX.Rendering;
using Forge.UX.Rendering.Text;
using Forge.UX.UI.Elements;

using System.Net.Mime;
using System.Numerics;

namespace Forge.UX.UI.Components {
    public class TextComponent : IUIComponent {
        public Vector2 Size { get; set; } = Vector2.One;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public (PositioningMode x, PositioningMode y) PositionMode { get; set; } = (PositioningMode.Normal, PositioningMode.Normal);
        public (PositioningMode width, PositioningMode height) SizeMode { get; set; } = (PositioningMode.Relative, PositioningMode.Relative);
        public Effects Effects { get; set; }
        public IElementData? Data { get; set; }

        public string Text { get; set; }

        public bool FitText { get; set; } = true;
        public Vector2 CalculatedSize { get; set; } = Vector2.Zero;

        public TextStyle Style { get; set; }

        public TextComponent(string text) {
            Text = text;
        }
    }
}
