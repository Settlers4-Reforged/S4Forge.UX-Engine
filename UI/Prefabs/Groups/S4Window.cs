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

    [GenerateDataBuilder]
    public class S4Window : GroupPrefab {
        public override string Name => "S4Window";
        public override string Description => "A generic Window in the style of a S4 pop-out";

        protected override void OverrideDefaults() {
            Padding.Default = new Vector4(35);
        }

        public override UIElement Instantiate() {
            ITextureCollectionManager tcm = DI.Dependencies.Resolve<ITextureCollectionManager>();
            UIWindow element = new UIWindow(new NineSliceTextureComponent(tcm.Get((int)TextureCollectionMap.ForgeUI, (int)ForgeTextureMap.S4Window), 35));
            this.ApplyPropertyValues(element);

            InstantiateChildren(element);

            return element;
        }
    }
}
