using System.Numerics;

namespace Forge.UX.UI.Components {
    public class TextComponent : IUIComponent {
        public Vector2 Offset { get; set; }
        public Vector2? Size { get; set; }
        public string Text { get; set; }

        public bool FitText { get; set; } = true;

        public TextComponent(string text) {
            Text = text;
        }
    }
}
