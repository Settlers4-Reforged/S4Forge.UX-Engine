using Forge.UX.Rendering.Text;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Text {
    public abstract class BaseText : ElementPrefab {
        public abstract override string Name { get; }
        public abstract override string Description { get; }
        public override UIElement Instantiate() {
            UIText text = new UIText(Text!);
            this.ApplyPropertyValues(text);

            TextComponent textComponent = text.TextComponent;
            textComponent.FitText = FitText;

            return text;
        }

        public ValueProperty Text { get; set; } = new ValueProperty(nameof(Text), "The text to display");
        public Property<bool> FitText { get; set; } = new Property<bool>(nameof(FitText), "If true, the text will be scaled to fit the element") { Default = false };
        public EnumProperty<TextAlignment> Alignment { get; set; } = new EnumProperty<TextAlignment>(nameof(Alignment), "The alignment of the text") { Default = TextAlignment.Center };
        public EnumProperty<TextType> Type { get; set; } = new EnumProperty<TextType>(nameof(TextType), "The type of text to display") { Default = TextType.Normal };
        public EnumProperty<TextSize> Size { get; set; } = new EnumProperty<TextSize>(nameof(Size), "The size of the text") { Default = TextSize.Regular };
    }
}
