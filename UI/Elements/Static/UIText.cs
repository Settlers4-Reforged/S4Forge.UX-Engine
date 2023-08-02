using Forge.UX.UI.Components;

using System.Collections.Generic;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UIText : UIElement {
        public UIText(string text) {
            Components = new List<IUIComponent> { new TextComponent(text) };
        }
    }
}
