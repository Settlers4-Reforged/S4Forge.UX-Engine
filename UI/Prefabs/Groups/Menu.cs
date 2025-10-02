using DasMulli.DataBuilderGenerator;

using Forge.Game.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Prefabs.Properties;

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

            group.AttachedMenu = AttachedMenu == S4UIMenu.Unknown ? null : AttachedMenu.Value;
            group.AttachedSubmenu = AttachedSubmenu == S4UISubmenu.Unknown ? null : AttachedSubmenu.Value;
            group.AttachedScreen = AttachedScreen == S4UIScreen.Unknown ? null : AttachedScreen.Value;

            InstantiateChildren(group);

            OnInstantiated(group);
            return group;
        }

        public EnumProperty<S4UIMenu> AttachedMenu = new EnumProperty<S4UIMenu>(nameof(AttachedMenu), "What S4 menu this is attached to") { Default = S4UIMenu.Unknown };
        public EnumProperty<S4UISubmenu> AttachedSubmenu = new EnumProperty<S4UISubmenu>(nameof(AttachedSubmenu), "What S4 submenu this is attached to") { Default = S4UISubmenu.Unknown };
        public EnumProperty<S4UIScreen> AttachedScreen = new EnumProperty<S4UIScreen>(nameof(AttachedScreen), "What S4 screen this is attached to") { Default = S4UIScreen.Unknown };
    }
}
