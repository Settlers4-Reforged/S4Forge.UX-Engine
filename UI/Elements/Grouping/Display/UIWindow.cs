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

        private bool dragging = false;
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

        public override void Input(SceneGraphState state) {
            base.Input(state);

            IInputManager im = DI.Resolve<IInputManager>();

            if (dragging && im.IsMouseOnScreen()) {
                Position += im.MouseDelta;

                bool clipToContainer = true;
                // Clip to container:
                if (clipToContainer) {
                    Vector2 containerSize = state.CurrentContainerSize;
                    Position = new Vector2(
                        Math.Clamp(Position.X, 0, containerSize.X - Size.X),
                        Math.Clamp(Position.Y, 0, containerSize.Y - Size.Y)
                    );
                }
            }
        }

        public override void OnMouseClickDown(int mb) {
            base.OnMouseClickDown(mb);

            if (Draggable && mb == 0) {
                dragging = true;
            }
        }

        public override void OnMouseGlobalClickUp(int mb) {
            base.OnMouseGlobalClickUp(mb);
            dragging = false;
        }

        public void Open() {
            Visible = true;

            Opened?.Invoke(this);
        }

        public void Close() {
            Visible = false;

            Closed?.Invoke(this);
        }

        public event UIEvent<UIWindow>? Opened;
        public event UIEvent<UIWindow>? Closed;
    }
}
