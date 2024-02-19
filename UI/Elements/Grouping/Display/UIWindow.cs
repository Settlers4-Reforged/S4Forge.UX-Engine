using Forge.Config;
using Forge.UX.Input;
using Forge.UX.Rendering.Texture;
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

            ITextureCollectionManager tcm = DI.Resolve<ITextureCollectionManager>();
            ITexture windowTexture = tcm.GetCollection(TextureCollectionMap.ForgeUI).GetTexture(ForgeTextureMap.S4Window);
            Components = new List<IUIComponent>() { new NineSliceTextureComponent(windowTexture, Vector4.Zero, Vector4.Zero) };
        }

        public UIWindow(NineSliceTextureComponent backgroundTexture) {
            PositionMode = (PositioningMode.Absolute, PositioningMode.Absolute);

            Components = new List<IUIComponent>() { backgroundTexture };
        }

        public override void OnMouseClickDown(int mb) {
            base.OnMouseClickDown(mb);

            IInputManager im = DI.Resolve<IInputManager>();

            if (Draggable && mb == 0) {
                Position += im.MouseDelta;
            }
        }

        public override void OnMouseClickUp(int mb) {
            base.OnMouseClickUp(mb);
        }

        public void Open() {
            Visible = true;
        }

        public void Close() {
            Visible = false;
        }

        public UIEvent<UIWindow>? OnOpen { get; set; }
        public UIEvent<UIWindow>? OnClose { get; set; }
    }
}
