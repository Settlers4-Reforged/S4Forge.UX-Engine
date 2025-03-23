using Forge.UX.UI.Components;
using Forge.UX.UI.Elements.Interaction;

using System;
using System.Collections.Generic;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UIText : UIElement, IUIBindable<string> {
        public TextComponent TextComponent;
        public string Text {
            get => TextComponent.Text;
            set {
                if (TextComponent.Text == value) return;
                TextComponent.Text = value;
                Dirty();
            }
        }

        public UIText(string text) {
            TextComponent = new TextComponent(text);
            Components = new List<IUIComponent> { TextComponent };
        }

        public override void Process(SceneGraphState state) {
            base.Process(state);

            if (BindingGetValue != null) {
                Text = BindingGetValue(this);
            }

            Size = TextComponent.CalculatedSize;
            SizeMode = (PositioningMode.Normal, PositioningMode.Normal);
        }

        /// <summary>
        /// Sets the text of the element. Get's called during rendering
        /// </summary>
        public event BindableGetter<string>? BindingGetValue;
    }
}
