using Forge.Config;
using Forge.S4.Types;
using Forge.S4.Types.Native.UI;
using Forge.UX.S4;

using System.Collections.Generic;
using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Display {

    /// <summary>
    /// A menu is a fullscreen group
    /// </summary>
    public sealed class UIMenu : UIDisplay<UIMenu> {

        public override (PositioningMode x, PositioningMode y) PositionMode => (PositioningMode.AbsoluteRelative, PositioningMode.AbsoluteRelative);
        public override (PositioningMode width, PositioningMode height) SizeMode => (PositioningMode.AbsoluteRelative, PositioningMode.AbsoluteRelative);

        public override Vector2 Size => new Vector2(1, 1);
        public override Vector2 Position => new Vector2(0, 0);

        /// <summary>
        /// 
        /// </summary>
        public S4UIMenuId? AttachedMenu { get; set; } = null;

        public bool IsAttached => AttachedMenu.HasValue;

        public UIMenu() {
            UICallbacks callbacks = DI.Resolve<UICallbacks>();

            callbacks.OnMenuChange += HandleMenuChange;
        }


        void HandleMenuChange(List<S4UIMenuId> previous, List<S4UIMenuId> next) {
            if (!IsAttached)
                return;

            if (next.Contains(AttachedMenu!.Value)) {
                OnOpen();
            } else {
                OnClose();
            }
        }

        public void OnOpen() {

        }

        public void OnClose() {

        }
    }
}
