using Forge.UX.Rendering.Texture;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Display;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Groups {
    public class S4Window : GroupPrefab {
        public override string Name => "S4Window";
        public override string Description => "A generic Window in the style of a S4 pop-out";

        public override UIElement Instantiate() {
            UIWindow element = new UIWindow(new NineSliceTextureComponent(UXEngine.TCM.Get((int)TextureCollectionMap.ForgeUI, (int)ForgeTextureMap.Window), 0, 0, 0, 0));
            this.ApplyPropertyValues(element);
            return element;
        }
    }
}
