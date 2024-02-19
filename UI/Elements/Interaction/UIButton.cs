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
        private readonly TextComponent textComponent = new TextComponent("NO TEXT") { Offset = new Vector2(0, 0) };
        private readonly TextureComponent textureComponent;

        // Proxies for various component options:
        /// <summary>
        /// The text of the button
        /// </summary>
        public string Text {
            get => textComponent?.Text ?? "";
            set => textComponent.Text = value;
        }

        /// <summary>
        /// The position offset of the button text
        /// </summary>
        public Vector2 TextOffset {
            get => textComponent.Offset;
            set => textComponent.Offset = value;
        }

        #endregion

        private readonly List<IUIComponent> components;
        public override List<IUIComponent> Components {
            get {
                switch (holdStatus) {
                    case State.Down:
                        textureComponent.Texture = ButtonHeldTexture!;
                        break;
                    case State.Up:
                        textureComponent.Texture = ButtonTexture!;
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
                    Effects &= ~Effects.GrayScale;
                } else {
                    Effects |= Effects.GrayScale;
                    holdStatus = State.Up;
                    interactionStarted = false;
                }

                enabled = value;
            }
        }

        public enum State {
            Up,
            Down,
        }

        protected bool interactionStarted = false;
        protected State holdStatus = State.Up;

        public Action<UIElement>? OnInteract { get; set; }
        public Action<UIElement>? OnHover { get; set; }
        #endregion

        public void DefaultTextures() {
            ITextureCollectionManager textureCollectionManager = DI.Dependencies.Resolve<ITextureCollectionManager>();

            DefaultButtonTexture ??= textureCollectionManager.Get(0, 0);
            DefaultButtonHeldTexture ??= textureCollectionManager.Get(0, 0);

            ButtonTexture ??= DefaultButtonTexture;
            ButtonHeldTexture ??= DefaultButtonHeldTexture;
        }


        public UIButton() {
            DefaultTextures();

            textureComponent = new TextureComponent(ButtonTexture!);
            components = new List<IUIComponent>() {
                textureComponent,
                textComponent,
            };
        }

        protected void SetState(State newState) {
            if (enabled) {
                holdStatus = newState;
            }
        }

        public override void OnMouseClickDown(int mb) {
            interactionStarted = true;
            SetState(State.Down);
        }

        public override void OnMouseClickUp(int mb) {
            interactionStarted = false;
            OnInteract?.Invoke(this);
            SetState(State.Up);
        }

        public override void OnMouseGlobalClickUp(int mb) {
            interactionStarted = false;
        }

        public override void OnMouseEnter() {
            Effects |= Effects.Highlight;

            if (interactionStarted) {
                SetState(State.Down);
            }

            OnHover?.Invoke(this);
        }

        public override void OnMouseLeave() {
            Effects &= ~Effects.Highlight;
            SetState(State.Up);
        }
    }
}
