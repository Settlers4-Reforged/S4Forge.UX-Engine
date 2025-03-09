using DasMulli.DataBuilderGenerator;

using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Prefabs.Groups {

    [Prefab("menu")]
    [GenerateDataBuilder]
    public class Menu : GroupPrefab {
        public override string Name => "Menu";
        public override string Description => "A fullscreen menu group - often attached to a S4 menu";
        public override UIElement Instantiate() {
            UIMenu group = new UIMenu();
            this.ApplyPropertyValues(group);

            InstantiateChildren(group);

            OnInstantiated(group);
            return group;
        }
    }
}
