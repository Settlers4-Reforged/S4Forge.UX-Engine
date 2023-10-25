using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Forge.UX.UI.Prefabs.Groups {
    public class Group : GroupPrefab {
        public override string Name => "Group";
        public override string Description => "A generic group of elements";
        public override UIElement Instantiate() {
            UIGroup group = new UIGroup();
            this.ApplyPropertyValues(group);

            InstantiateChildren(group);

            return group;
        }
    }
}
