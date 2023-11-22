using Forge.UX.Input;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements.Grouping.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Display {
    public sealed class UIWindow : UIDisplay<UIWindow> {
        /// <summary>
        /// Whether the window should stay open when the S4 menu changes
        /// </summary>
        public bool PersistMenus { get; set; } = true;

        public bool Draggable { get; set; } = true;

        public UIWindow() {
            PositionMode = (PositioningMode.Absolute, PositioningMode.Absolute);

            Components = new List<IUIComponent>() { new NineSliceTextureComponent(null!, Vector4.Zero, Vector4.Zero) };
        }

        public UIWindow(NineSliceTextureComponent backgroundTexture) {
            PositionMode = (PositioningMode.Absolute, PositioningMode.Absolute);

            Components = new List<IUIComponent>() { backgroundTexture };
        }

        public override void OnMouseClickDown(int mb) {
            base.OnMouseClickDown(mb);

            if (Draggable && mb == 0) {
                Position += InputManager.MouseDelta;
            }
        }

        public override void OnMouseClickUp(int mb) {
            base.OnMouseClickUp(mb);
        }

        public void Open() {
            throw new System.NotImplementedException();
        }

        public void Close() {
            throw new System.NotImplementedException();
        }

        public UIEvent<UIWindow> OnOpen { get; set; }
        public UIEvent<UIWindow> OnClose { get; set; }
    }
}
