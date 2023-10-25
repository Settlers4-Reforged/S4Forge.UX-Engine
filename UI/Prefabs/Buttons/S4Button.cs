using Forge.UX.Rendering;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Forge.UX.UI.Prefabs.Buttons {
    public class S4Button : ElementPrefab {
        public override string Name => "S4Button";
        public override string Description => "A generic button in the style of a S4 button";
        public override UIElement Instantiate() {
            UIButton button = new UIButton {
                ButtonTexture = UXEngine.TCM.Get(0, 194),
                ButtonHeldTexture = UXEngine.TCM.Get(0, 195),
                Text = Text!,
                Enabled = IsEnabled!,
                TextOffset = new Vector2(0, -4),
            };

            this.ApplyPropertyValues(button);

            return button;
        }

        public ValueProperty Text = new ValueProperty(nameof(Text), "Text to display on the button") { Default = "-" };
        public Property<bool> IsEnabled = new Property<bool>(nameof(IsEnabled), "Whether the button is enabled") { Default = true };


        public override IEnumerable<IProperty> GetProperties() {
            List<IProperty> props = (List<IProperty>)base.GetProperties();
            props.AddRange(new IProperty[] {
                Text,
                IsEnabled,
            });

            return props;
        }
    }
}
