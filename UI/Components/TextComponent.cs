using Forge.UX.Rendering.Text;

using System.Net.Mime;
using System.Numerics;

namespace Forge.UX.UI.Components {
    public class TextComponent : IUIComponent {
        public Vector2 Offset { get; set; }
        public Vector2? Size { get; set; }
        public string Text { get; set; }

        public bool FitText { get; set; } = true;

        public TextStyle Style { get; set; }

        public TextComponent(string text) {
            Text = text;
        }
    }
}
