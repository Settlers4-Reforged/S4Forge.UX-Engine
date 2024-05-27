using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.UI.Components;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Forge.UX.UI.Elements {
    public abstract class UIElement {
        public virtual string Id { get; set; } = string.Empty;

        public virtual List<IUIComponent> Components { get; protected set; } = new List<IUIComponent>();

        private Vector2 size;

        public virtual Vector2 Size {
            get => size;
            set {
                IsDirty = true;
                size = value;
            }
        }


        private Vector2 position;
        public virtual Vector2 Position {
            get => position;
            set {
                IsDirty = true;
                position = value;
            }
        }

        /// <summary>
        /// Whether the element is positioned in screen space coordinates, or relative to its group parent or relative to the screen size
        /// </summary>
        public virtual (PositioningMode x, PositioningMode y) PositionMode { get; set; } = (PositioningMode.Normal, PositioningMode.Normal);

        /// <summary>
        /// Whether the element is sized in screen space coordinates, or relative to its group parent or relative to the screen size
        /// </summary>
        public virtual (PositioningMode width, PositioningMode height) SizeMode { get; set; } = (PositioningMode.Normal, PositioningMode.Normal);

        /// <summary> Whether the element is visible during rendering phase </summary>
        public virtual bool Visible { get; set; } = true;

        /// <summary> Whether the element is dirty and needs to be re-rendered </summary>
        private bool isDirty = true;
        public bool IsDirty {
            get {
                return isDirty;
            }
            set {
                isDirty = value;
            }
        }

        ///<summary>The lower the further behind: 0 &lt; 100 &lt; 1000</summary>
        public int ZIndex = 0;

        bool _isMouseHover;

        public bool IsMouseHover {
            get {
                return _isMouseHover;
            }
            set {
                _isMouseHover = value;
                OnHover?.Invoke(this, value);
            }
        }

        public event UIEvent<UIElement, bool>? OnHover;

        protected void Hover(bool hover) {
            IsMouseHover = hover;
        }

        /// <summary>
        /// Prevents mouse hover or click inputs from being captured from this element<br/>
        /// Will ignore all child elements when true for group
        /// </summary>
        public bool IgnoresMouse = false;

        /// <summary>
        /// Hides the element from rendering and disables input handling
        /// </summary>
        public void Hide() {
            Visible = false;
            IgnoresMouse = true;
        }

        /// <summary>
        /// Shows the element for rendering and enables input handling
        /// </summary>
        public void Show() {
            Visible = true;
            IgnoresMouse = false;
        }

        #region Mouse Events
        /// <summary>
        /// Called when the mouse is clicked down while the mouse is over the element
        /// </summary>
        /// <param name="mb">Which mouse button was pressed. 0 = left, 1 = middle, 2 = right</param>
        public virtual void OnMouseClickDown(int mb) { }

        /// <summary>
        /// Called when the mouse is clicked up while the mouse is over the element
        /// </summary>
        /// <param name="mb">Which mouse button was pressed. 0 = left, 1 = middle, 2 = right</param>
        public virtual void OnMouseClickUp(int mb) { }

        /// <summary>
        /// Called when the mouse wheel is scrolled while the mouse is over the element
        /// </summary>
        /// <param name="scroll">The amount scrolled, signed to the direction</param>
        public virtual void OnMouseScroll(float scroll) { }

        /// <summary>
        /// Called when the mouse enters the element
        /// </summary>
        public virtual void OnMouseEnter() { }
        /// <summary>
        /// Called when the mouse leaves the element
        /// </summary>
        public virtual void OnMouseLeave() { }

        /// <summary>
        /// Called when the mouse is clicked down anywhere on the screen
        /// </summary>
        /// <param name="mb">Which mouse button was pressed. 0 = left, 1 = middle, 2 = right</param>
        public virtual void OnMouseGlobalClickDown(int mb) { }

        /// <summary>
        /// Called when the mouse is clicked up anywhere on the screen
        /// </summary>
        /// <param name="mb">Which mouse button was pressed. 0 = left, 1 = middle, 2 = right</param>
        public virtual void OnMouseGlobalClickUp(int mb) { }
        #endregion

        /// <summary>
        /// The manager of the scene tree that this element is attached to
        /// </summary>
        public SceneManager? AttachedManager { get; protected set; }

        public virtual void Attach(SceneManager manager) {
            AttachedManager = manager;
            IsDirty = true;
        }

        public virtual void Input(SceneGraphState state) {
            OnInput?.Invoke(this);
        }

        #region EventHandlers

        public delegate void UIEvent<in T>(T initiator);
        public delegate void UIEvent<in T, in TA1>(T initiator, TA1 value);

        public event UIEvent<UIElement>? OnInput;

        #endregion
    }
}
