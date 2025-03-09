using DasMulli.DataBuilderGenerator;

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

    [Prefab("s4-radio-button")]
    [GenerateDataBuilder]
    public class S4RadioButton : ElementPrefab {
        public override string Name => "S4RadioButton";
        public override string Description => "A Settlers 4 style radio button - must be used in a RadioButtonGroup";
        public override UIElement Instantiate() {
            ITextureCollection<ForgeTextureMap> tc = DI.Dependencies.Resolve<ITextureCollection<ForgeTextureMap>>();

            UIElement button = new UIRadioButton<string>(LinkId!, Value!) {
                ButtonTexture = tc.GetTexture(ForgeTextureMap.RadiobuttonUnchecked),
                ButtonHeldTexture = tc.GetTexture(ForgeTextureMap.RadiobuttonUnchecked),
                Text = Text!,
                Enabled = IsEnabled!,
            };

            OnInstantiated(button);
            return button;
        }


        public Property<string> LinkId { get; set; } = new Property<string>(nameof(LinkId), "The ID of the RadioButtonGroup to link to") { Required = true };
        public ValueProperty Text { get; set; } = new ValueProperty(nameof(Text), "Text to display on the button") { Default = "-" };
        public Property<bool> IsEnabled { get; set; } = new Property<bool>(nameof(IsEnabled), "Whether the button is enabled") { Default = true };
        public Property<string> Value { get; set; } = new Property<string>(nameof(Value), "The value of the radio element") { Default = "", Required = true };
    }
}
