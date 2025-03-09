using DasMulli.DataBuilderGenerator;

using DryIoc;

using Forge.Config;
using Forge.UX.Rendering.Texture;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Display;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Forge.UX.UI.Prefabs.Groups {

    [Prefab("s4-window")]
    [GenerateDataBuilder]
    public class S4Window : GroupPrefab {
        public override string Name => "S4Window";
        public override string Description => "A generic Window in the style of a S4 pop-out";

        protected override void OverrideDefaults() {
            Padding.Default = new Vector4(35);
        }

        public override UIElement Instantiate() {
            ITextureCollection<ForgeTextureMap> tc = DI.Dependencies.Resolve<ITextureCollection<ForgeTextureMap>>();
            UIWindow element = new UIWindow(new NineSliceTextureComponent(tc.GetTexture(ForgeTextureMap.S4Window), 35));
            this.ApplyPropertyValues(element);

            InstantiateChildren(element);

            OnInstantiated(element);
            return element;
        }
    }
}
