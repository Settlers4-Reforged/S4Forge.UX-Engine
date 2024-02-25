using Forge.UX.Rendering.Text;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Text {
    public abstract class TextPrefab : ElementPrefab {
        public abstract override string Name { get; }
        public abstract override string Description { get; }
        public override UIElement Instantiate() {
            UIText text = new UIText(Text!);
            this.ApplyPropertyValues(text);

            ApplyTextPropertyValues(text.TextComponent);

            return text;
        }

        protected void ApplyTextPropertyValues(TextComponent targetTextComponent) {
            targetTextComponent.Text = Text!;
            targetTextComponent.Style = new TextStyle() {
                Size = TextSize,
                Type = TextType,
                Alignment = TextAlignment
            };
        }

        public ValueProperty Text { get; set; } = new ValueProperty(nameof(Text), "The text to display");
        public Property<bool> FitText { get; set; } = new Property<bool>(nameof(FitText), "If true, the text will be scaled to fit the element") { Default = false };
        public EnumProperty<TextStyleAlignment> TextAlignment { get; set; } = new EnumProperty<TextStyleAlignment>(nameof(TextAlignment), "The alignment of the text") { Default = TextStyleAlignment.Center };
        public EnumProperty<TextStyleType> TextType { get; set; } = new EnumProperty<TextStyleType>(nameof(TextType), "The type of text to display") { Default = TextStyleType.Normal };
        public EnumProperty<TextStyleSize> TextSize { get; set; } = new EnumProperty<TextStyleSize>(nameof(TextSize), "The size of the text") { Default = TextStyleSize.Regular };
    }
}
