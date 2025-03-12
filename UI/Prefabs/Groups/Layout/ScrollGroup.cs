using DasMulli.DataBuilderGenerator;

using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Interaction;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Groups.Layout {


    [Prefab("scroll")]
    [GenerateDataBuilder]
    public class ScrollGroup : GroupPrefab {
        public override string Name => "Scroll";
        public override string Description => "A scroll area for child elements";
        public override UIElement Instantiate() {
            UIScrollGroup element = new UIScrollGroup();
            this.ApplyPropertyValues(element);

            InstantiateChildren(element);
            element.Relayout();

            OnInstantiated(element);
            return element;
        }

    }
}
