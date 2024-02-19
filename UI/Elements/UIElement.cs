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

        public virtual Vector2 Size { get; set; }

        public virtual Vector2 Position { get; set; }
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

        public Action<UIElement, bool>? OnHover { get; set; }

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


        public virtual void OnMouseClickDown(int mb) { }
        public virtual void OnMouseClickUp(int mb) { }
        public virtual void OnMouseGlobalClickDown(int mb) { }
        public virtual void OnMouseGlobalClickUp(int mb) { }
        public virtual void OnMouseScroll(float scroll) { }

        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }

        public virtual void Input(SceneGraphState state) {
            OnInput?.Invoke(this);
        }

        #region EventHandlers

        public delegate void UIEvent<in T>(T initiator);

        public UIEvent<UIElement>? OnInput;

        #endregion
    }
}
