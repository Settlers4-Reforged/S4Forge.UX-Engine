using DryIoc;

using Forge.Config;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Texture;
using Forge.UX.UI.Components;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;

namespace Forge.UX.UI.Elements.Interaction {
    public class UIButton : UIElement, IUIInteractable {
        public static ITexture? DefaultButtonTexture, DefaultButtonHeldTexture;
        public ITexture? ButtonTexture, ButtonHeldTexture;


        #region Components
        // Button components, text defines the button text and texture represents both texture states
        public readonly TextComponent TextComponent = new TextComponent("NO TEXT");
        public readonly TextureComponent TextureComponent;

        // Proxies for various component options:
        /// <summary>
        /// The text of the button
        /// </summary>
        public string Text {
            get => TextComponent?.Text ?? "";
            set {
                IsDirty = true;
                TextComponent.Text = value;
            }
        }

        /// <summary>
        /// The offset of the button text
        /// </summary>
        public Vector2 TextOffset {
            get => TextComponent.Position;
            set {
                IsDirty = true;
                TextComponent.Position = value;
            }
        }

        /// <summary>
        /// The inner bounding box of the button text
        /// </summary>
        public Vector2 TextSize {
            get => TextComponent.Size;
            set {
                IsDirty = true;
                TextComponent.Size = value;
            }
        }

        public Vector2 HeldTextOffset { get; set; }

        #endregion

        private readonly List<IUIComponent> components;
        public override List<IUIComponent> Components {
            get {
                switch (holdStatus) {
                    case State.Down:
                        TextureComponent.Texture = ButtonHeldTexture!;
                        break;
                    case State.Up:
                        TextureComponent.Texture = ButtonTexture!;
                        break;
                }

                return components;
            }
        }

        #region State

        private bool enabled;

        /// <summary>
        /// Whether this button can be interacted with
        /// </summary>
        public bool Enabled {
            get => enabled;
            set {
                if (value) {
                    TextureComponent.Effects &= ~Effects.GrayScale;
                } else {
                    TextureComponent.Effects |= Effects.GrayScale;
                    holdStatus = State.Up;
                    interactionStarted = false;
                }

                IsDirty = true;
                enabled = value;
            }
        }

        public enum State {
            Up,
            Down,
        }

        protected bool interactionStarted = false;
        protected State holdStatus = State.Up;

        public bool IsHolding => holdStatus == State.Down;

        public event UIEvent<UIElement>? OnInteract;
        #endregion

        public void DefaultTextures() {
            ITextureCollectionManager textureCollectionManager = DI.Dependencies.Resolve<ITextureCollectionManager>();

            DefaultButtonTexture ??= textureCollectionManager.Get(TextureCollectionMap.ForgeUI, (int)ForgeTextureMap.Button);
            DefaultButtonHeldTexture ??= textureCollectionManager.Get(TextureCollectionMap.ForgeUI, (int)ForgeTextureMap.ButtonPressed);

            ButtonTexture ??= DefaultButtonTexture;
            ButtonHeldTexture ??= DefaultButtonHeldTexture;
        }


        public UIButton() {
            DefaultTextures();

            TextureComponent = new TextureComponent(ButtonTexture!);
            components = new List<IUIComponent>() {
                TextureComponent,
                TextComponent,
            };
        }

        protected void SetState(State newState) {
            if (!enabled) return;

            if (newState != holdStatus) {
                int direction = newState == State.Down ? 1 : -1;
                TextOffset += HeldTextOffset * direction;
            }

            holdStatus = newState;

            Hover(holdStatus == State.Down);
        }

        protected virtual void Interact() {
            OnInteract?.Invoke(this);
        }

        public override void OnMouseClickDown(int mb) {
            interactionStarted = true;
            SetState(State.Down);
        }

        public override void OnMouseClickUp(int mb) {
            if (!interactionStarted)
                return;

            interactionStarted = false;

            if (enabled)
                Interact();

            SetState(State.Up);
        }

        public override void OnMouseGlobalClickUp(int mb) {
            interactionStarted = false;
        }

        public override void OnMouseEnter() {
            TextureComponent.Effects |= Effects.Highlight;
            IsDirty = true;

            if (interactionStarted) {
                SetState(State.Down);
            }

        }

        public override void OnMouseLeave() {
            TextureComponent.Effects &= ~Effects.Highlight;
            IsDirty = true;
            SetState(State.Up);
        }
    }
}
