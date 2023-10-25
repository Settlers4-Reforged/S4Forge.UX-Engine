using Forge.UX.UI.Components;
using Forge.UX.UI.Elements.Interaction;

using System;
using System.Collections.Generic;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UIText : UIElement, IUIBindable<string> {
        public TextComponent Text;

        public UIText(string text) {
            Text = new TextComponent(text);
            Components = new List<IUIComponent> { Text };
        }

        public override void Input(SceneGraphState state) {
            base.Input(state);

            if (BindingGetValue != null) {
                Text.Text = BindingGetValue(this);
            }
        }

        /// <summary>
        /// Sets the text of the element. Get's called during rendering
        /// </summary>
        public Func<UIElement, string>? BindingGetValue { get; set; }
    }
}
