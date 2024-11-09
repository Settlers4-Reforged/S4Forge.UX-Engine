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

        public override void Process(SceneGraphState state) {
            base.Process(state);

            IInputManager im = DI.Resolve<IInputManager>();

            if (dragging && im.IsMouseOnScreen() && im.MouseDelta.LengthSquared() > 0) {
                Position += im.MouseDelta;
                IsDirty = true;

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

        public override void Input(ref InputEvent @event) {
            base.Input(ref @event);

            if (!Visible)
                return;

            if (@event is { Type: InputType.MouseWheel })
                @event.IsHandled = true;

            if (Draggable && @event is { Key: Keys.LButton, Type: InputType.KeyDown }) {
                dragging = true;

                @event.IsHandled = true;
            }
        }


        public override void OnMouseGlobalClickUp(int mb) {
            base.OnMouseGlobalClickUp(mb);
            dragging = false;
        }

        public void Open() {
            Visible = true;
            IsDirty = true;

            Opened?.Invoke(this);
        }

        public void Close() {
            Visible = false;
            IsDirty = true;

            Closed?.Invoke(this);
        }

        public event UIEventAction<UIWindow>? Opened;
        public event UIEventAction<UIWindow>? Closed;
    }
}
