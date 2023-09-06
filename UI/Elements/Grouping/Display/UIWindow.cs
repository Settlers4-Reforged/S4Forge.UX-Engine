using Forge.UX.UI.Components;
using Forge.UX.UI.Elements.Grouping.Layout;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Display {
    public sealed class UIWindow : UIDisplay<UIWindow> {
        /// <summary>
        /// Whether the window should stay open when the S4 menu changes
        /// </summary>
        public bool PersistMenus { get; set; } = true;

        public UIWindow() {
            PositionMode = (PositioningMode.Absolute, PositioningMode.Absolute);

            Components = new List<IUIComponent>() { new NineSliceTextureComponent(null!, Vector4.Zero, Vector4.Zero) };
        }

        public UIWindow(NineSliceTextureComponent backgroundTexture) {
            PositionMode = (PositioningMode.Absolute, PositioningMode.Absolute);

            Components = new List<IUIComponent>() { backgroundTexture };
        }

        public void Open() {
            throw new System.NotImplementedException();
        }

        public void Close() {
            throw new System.NotImplementedException();
        }

        public void OnOpen() {

        }

        public void OnClose() {

        }
    }
}
