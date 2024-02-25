using DasMulli.DataBuilderGenerator;

using DryIoc;

using Forge.Config;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Text;
using Forge.UX.Rendering.Texture;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Prefabs.Properties;
using Forge.UX.UI.Prefabs.Text;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Forge.UX.UI.Prefabs.Buttons {

    [GenerateDataBuilder]
    public class S4Button : TextPrefab {
        public override string Name => "S4Button";
        public override string Description => "A generic button in the style of a S4 button";

        protected override void OverrideDefaults() {
            // Default size of a S4 button:
            Size.Default = new Vector2(322, 60);
            TextAlignment.Default = TextStyleAlignment.Center;
        }

        public override UIElement Instantiate() {
            ITextureCollectionManager tcm = DI.Dependencies.Resolve<ITextureCollectionManager>();

            UIButton button = new UIButton {
                ButtonTexture = tcm.Get(0, 194),
                ButtonHeldTexture = tcm.Get(0, 195),
                Text = Text!,
                Enabled = IsEnabled!,
                TextOffset = new Vector2(15, -6),
                HeldTextOffset = new Vector2(2, 2)
            };

            this.ApplyPropertyValues(button);
            this.ApplyTextPropertyValues(button.TextComponent);

            return button;
        }

        public Property<bool> IsEnabled { get; set; } = new Property<bool>(nameof(IsEnabled), "Whether the button is enabled") { Default = true };
    }
}
