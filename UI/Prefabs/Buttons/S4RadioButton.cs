using DryIoc;

using Forge.Config;
using Forge.UX.Rendering.Texture;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Buttons {
    public class S4RadioButton : ElementPrefab {
        public override string Name => "S4RadioButton";
        public override string Description => "A Settlers 4 style radio button - must be used in a RadioButtonGroup";
        public override UIElement Instantiate() {
            ITextureCollectionManager tcm = DI.Dependencies.Resolve<ITextureCollectionManager>();

            return new UIRadioButton<string>(LinkId!, Value!) {
                ButtonTexture = tcm.Get((int)TextureCollectionMap.ForgeUI, (int)ForgeTextureMap.RadiobuttonUnchecked),
                ButtonHeldTexture = tcm.Get((int)TextureCollectionMap.ForgeUI, (int)ForgeTextureMap.RadiobuttonUnchecked),
                Text = Text!,
                Enabled = IsEnabled!,
            };
        }


        public Property<string> LinkId = new Property<string>(nameof(LinkId), "The ID of the RadioButtonGroup to link to") { Required = true };
        public ValueProperty Text = new ValueProperty(nameof(Text), "Text to display on the button") { Default = "-" };
        public Property<bool> IsEnabled = new Property<bool>(nameof(IsEnabled), "Whether the button is enabled") { Default = true };
        public Property<string> Value = new Property<string>(nameof(Value), "The value of the radio element") { Default = "", Required = true };
    }
}
