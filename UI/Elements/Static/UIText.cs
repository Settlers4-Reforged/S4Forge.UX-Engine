using Forge.UX.UI.Components;
using Forge.UX.UI.Elements.Interaction;

using System;
using System.Collections.Generic;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UIText : UIElement, IUIBindable<string> {
        public TextComponent TextComponent;
        public string Text {
            get => TextComponent.Text;
            set => TextComponent.Text = value;
        }

        public UIText(string text) {
            TextComponent = new TextComponent(text);
            Components = new List<IUIComponent> { TextComponent };
        }

        public override void Input(SceneGraphState state) {
            base.Input(state);

            if (BindingGetValue != null) {
                Text = BindingGetValue(this);
            }
        }

        /// <summary>
        /// Sets the text of the element. Get's called during rendering
        /// </summary>
        public event BindableGetter<string>? BindingGetValue;
    }
}
