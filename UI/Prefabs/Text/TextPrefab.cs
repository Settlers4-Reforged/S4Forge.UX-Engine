using Forge.UX.Rendering.Text;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Forge.UX.UI.Prefabs.Text {
    public abstract class TextPrefab : ElementPrefab {
        public abstract override string Name { get; }
        public abstract override string Description { get; }
        public override UIElement Instantiate() {
            UIText text = new UIText(Text!);
            this.ApplyPropertyValues(text);

            ApplyTextPropertyValues(text.TextComponent);

            OnInstantiated(text);
            return text;
        }

        protected void ApplyTextPropertyValues(TextComponent targetTextComponent) {
            targetTextComponent.Text = Text!;

            // Convert the hex color input to a Vector4
            if (Color.Value?.StartsWith('#') != true)
                throw new ArgumentException("Color must be a hex value");

            ReadOnlySpan<char> hex = Color.Value.AsSpan()[1..];
            if (hex.Length is not (6 or 8))
                throw new ArgumentException("Color must be 6 or 8 characters long");

            Vector4 color = new Vector4(
                int.Parse(hex[0..2], NumberStyles.AllowHexSpecifier),
                int.Parse(hex[2..4], NumberStyles.AllowHexSpecifier),
                int.Parse(hex[4..6], NumberStyles.AllowHexSpecifier),
                hex.Length == 8 ? int.Parse(hex[6..8], NumberStyles.AllowHexSpecifier) : 255
            );


            targetTextComponent.Style = new TextStyle() {
                Size = TextSize,
                Type = TextType,
                HorizontalAlignment = TextHorizontalAlignment,
                Color = color
            };
        }

        public ValueProperty Text { get; set; } = new ValueProperty(nameof(Text), "The text to display");
        public Property<bool> FitText { get; set; } = new Property<bool>(nameof(FitText), "If true, the text will be scaled to fit the element") { Default = false };
        public EnumProperty<TextStyleAlignment> TextHorizontalAlignment { get; set; } = new EnumProperty<TextStyleAlignment>(nameof(TextHorizontalAlignment), "The horizontal alignment of the text") { Default = TextStyleAlignment.Center };
        public EnumProperty<TextStyleAlignment> TextVerticalAlignment { get; set; } = new EnumProperty<TextStyleAlignment>(nameof(TextVerticalAlignment), "The vertical alignment of the text") { Default = TextStyleAlignment.Center };
        public EnumProperty<TextStyleType> TextType { get; set; } = new EnumProperty<TextStyleType>(nameof(TextType), "The type of text to display") { Default = TextStyleType.Normal };
        public EnumProperty<TextStyleSize> TextSize { get; set; } = new EnumProperty<TextStyleSize>(nameof(TextSize), "The size of the text") { Default = TextStyleSize.Regular };
        public Property<string> Color { get; set; } = new Property<string>(nameof(Color), "The color of the text") { Default = "#FFFFFFFF" };
    }
}
