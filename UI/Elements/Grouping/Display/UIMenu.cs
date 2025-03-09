using Forge.Config;
using Forge.S4.Types;
using Forge.S4.Types.Native.UI;
using Forge.UX.S4;

using System;
using System.Collections.Generic;
using System.Numerics;

using S4UIMenu = Forge.S4.Types.S4UIMenu;

namespace Forge.UX.UI.Elements.Grouping.Display {

    /// <summary>
    /// A menu is a fullscreen group.
    ///
    /// When a menu is attached to a S4 screen, menu, or submenu, it will be only shown when the S4 menu is on screen
    /// </summary>
    public sealed class UIMenu : UIDisplay<UIMenu> {

        public override (PositioningMode x, PositioningMode y) PositionMode => (PositioningMode.Absolute, PositioningMode.Absolute);
        public override (PositioningMode width, PositioningMode height) SizeMode => (PositioningMode.AbsoluteRelative, PositioningMode.AbsoluteRelative);

        public override Vector2 Size => new Vector2(1, 1);
        public override Vector2 Position => new Vector2(0, 0);

        /// <summary>
        /// What S4 menu this is attached to. When null, the menu is always visible
        /// </summary>
        public S4UIMenu? AttachedMenu { get; set; } = null;
        public S4UISubmenu? AttachedSubmenu { get; set; } = null;
        public S4UIScreen? AttachedScreen { get; set; } = null;

        IUIManager uiManager = DI.Resolve<IUIManager>();

        public bool IsAttached => AttachedMenu.HasValue;

        public UIMenu() {
        }

        public override void Process(SceneGraphState state) {
            S4UIMenu activeMenu = AttachedMenu == null ? S4UIMenu.Unknown : uiManager.GetActiveMenu();
            S4UISubmenu activeSubmenu = AttachedSubmenu == null ? S4UISubmenu.Unknown : uiManager.GetActiveSubmenu();
            S4UIScreen activeScreen = AttachedScreen == null ? S4UIScreen.Unknown : uiManager.GetActiveScreen();

            if (
                AttachedMenu?.HasFlag(activeMenu) == true ||
                AttachedSubmenu?.HasFlag(activeSubmenu) == true ||
                AttachedScreen?.HasFlag(activeScreen) == true
            ) {
                OnOpen();
            } else {
                OnClose();
            }
        }


        public void OnOpen() {
            if (Visible) return;

            Visible = true;
            Dirty();
        }

        public void OnClose() {
            if (!Visible) return;

            Visible = false;
            Dirty();
        }
    }
}
