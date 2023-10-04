using Forge.UX.UI.Components;

using System.Collections.Generic;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UIText : UIElement {
        public TextComponent Text;

        public UIText(string text) {
            Text = new TextComponent(text);
            Components = new List<IUIComponent> { Text };
        }
    }
}
