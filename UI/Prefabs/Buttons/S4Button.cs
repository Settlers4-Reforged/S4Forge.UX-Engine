using DasMulli.DataBuilderGenerator;

using DryIoc;

using Forge.Config;
using Forge.S4.Game;
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

using static System.Net.Mime.MediaTypeNames;

namespace Forge.UX.UI.Prefabs.Buttons {

    [Prefab("s4-button")]
    [GenerateDataBuilder]
    public class S4Button : TextPrefab {
        public override string Name => "S4Button";
        public override string Description => "A generic button in the style of a S4 button";

        const float DefaultWidth = 322;
        const float DefaultHeight = 60;

        protected override void OverrideDefaults() {
            // Default size of a S4 button:
            Size.Default = new Vector2(DefaultWidth, DefaultHeight);
            TextHorizontalAlignment.Default = TextStyleAlignment.Center;
            TextSize.Default = TextStyleSize.Large;
            TextType.Default = TextStyleType.Bold;
        }

        public override UIElement Instantiate() {
            ITextureCollection<S4MainUITextureMap> tc = DI.Dependencies.Resolve<ITextureCollection<S4MainUITextureMap>>();

            UIButton button = new UIButton {
                ButtonTexture = tc.GetTexture(S4MainUITextureMap._194),
                ButtonHeldTexture = tc.GetTexture(S4MainUITextureMap._195),
                Text = Text!,
                Enabled = IsEnabled!,
                TextOffset = new Vector2(15 / DefaultWidth, 13 / DefaultHeight), // 15, 10
                TextSize = new Vector2(292 / DefaultWidth, 32 / DefaultHeight),
                HeldTextOffset = new Vector2(2 / DefaultWidth, 2 / DefaultHeight)
            };


            bool? prevState = null;
            button.OnHover += (element, hovering) => {
                ISoundApi sound = DI.Resolve<ISoundApi>();
                if (button.IsHolding) {
                    if (prevState == hovering) {
                        return;
                    }

                    sound.PlaySound(hovering ? 7 : 6);
                    prevState = hovering;
                }
            };

            button.OnInteract += (_) => {
                prevState = null;

                ISoundApi sound = DI.Resolve<ISoundApi>();
                sound.PlaySound(11);
                //This seems to be a b u g in the game, it plays the release and click button simultaneously when releasing a button press
                sound.PlaySound(6);
            };

            this.ApplyPropertyValues(button);
            this.ApplyTextPropertyValues(button.TextComponent);
            button.TextComponent.PositionMode = (PositioningMode.Relative, PositioningMode.Relative);

            OnInstantiated(button);
            return button;
        }

        public Property<bool> IsEnabled { get; set; } = new Property<bool>(nameof(IsEnabled), "Whether the button is enabled") { Default = true };
    }
}
