using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements.Grouping;

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
        /// The rendering data associated with the element
        /// </summary>
        /// <remarks>
        /// This object is only for rendering purposes and should not be used by application code
        /// </remarks>
        public IElementData? Data { get; set; }

        /// <summary>
        /// Whether the element is positioned in screen space coordinates, or relative to its group parent or relative to the screen size
        /// </summary>
        public virtual (PositioningMode x, PositioningMode y) PositionMode { get; set; } = (PositioningMode.Normal, PositioningMode.Normal);

        /// <summary>
        /// Whether the element is sized in screen space coordinates, or relative to its group parent or relative to the screen size
        /// </summary>
        public virtual (PositioningMode width, PositioningMode height) SizeMode { get; set; } = (PositioningMode.Normal, PositioningMode.Normal);

        private bool _visible = true;
        /// <summary> Whether the element is visible during rendering phase </summary>
        public virtual bool Visible {
            get {
                if (Parent != null) {
                    return Parent.Visible && _visible;
                }

                return _visible;
            }
            set {
                _visible = value;
            }
        }

        /// <summary> Whether the element should process input events </summary>
        public bool ProcessInputEvents { get; set; } = true;
        /// <summary> Whether the element should process unhandled input events </summary>
        public bool ProcessUnhandledInputEvents { get; set; } = false;
        /// <summary> Whether the element should process windows message input events </summary>
        public bool ProcessWindowsInputEvents { get; set; } = false;

        /// <summary> Whether the element is dirty and needs to be re-rendered </summary>
        private bool isDirty = true;
        public virtual bool IsDirty {
            get {
                return isDirty;
            }
            set {
                if (isDirty != value && Parent != null) {
                    isDirty = value;
                    Parent.IsDirty = value;
                }
                isDirty = value;
            }
        }

        /// <summary>
        /// Marks the element as dirty and in need of re-rendering
        /// </summary>
        /// <param name="force">
        ///   Normally dirty handling is skipped when this component is disabled/not visible.
        ///   When forced will force dirty check even when disabled.
        /// </param>
        public virtual void Dirty(bool force = false) {
            if (!force && !Visible) return;
            this.IsDirty = true;
        }

        ///<summary>The lower the further behind: 0 &lt; 100 &lt; 1000</summary>
        public int ZIndex = 0;

        /// <summary>
        /// Whether the element is currently being hovered by the mouse.
        ///
        /// This does not take into account the element's visibility or input hierarchy.
        /// </summary>
        public bool IsMouseHover { get; set; }

        public event UIEventAction<UIElement, bool>? OnHover;


        /// <summary>
        /// Hides the element from rendering and disables input handling
        /// </summary>
        public void Hide() {
            if (!Visible) return;
            Visible = false;
            IsDirty = true;
        }

        /// <summary>
        /// Shows the element for rendering and enables input handling
        /// </summary>
        public void Show() {
            if (Visible) return;
            Visible = true;
            IsDirty = true;
        }

        #region Mouse Events
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

        /// <summary>
        /// The parent group of this element
        /// </summary>
        public UIGroup? Parent { get; protected set; }

        public virtual void Attach(SceneManager manager, UIGroup? parent) {
            AttachedManager = manager;
            Parent = parent;
            IsDirty = true;
        }

        /// <summary>
        /// Generic per-frame callback
        /// </summary>
        /// <param name="state"></param>
        public virtual void Process(SceneGraphState state) {
            OnProcess?.Invoke(this, state);
        }

        /// <summary>
        /// Input handling for the element
        /// </summary>
        public virtual void Input(ref InputEvent @event) {
            OnInput?.Invoke(this, @event);
        }

        /// <summary>
        /// When an input event is not handled by any hovered element, then this is called
        /// </summary>
        public virtual void UnhandledInput(InputEvent @event) {
            OnUnhandledInput?.Invoke(this, @event);
        }

        /// <summary>
        /// A per-tick callback. Used for low frequency game related logic
        /// </summary>
        public virtual void Tick() {
            OnTick?.Invoke(this);
        }

        #region EventHandlers

        public delegate void UIEventAction<in T>(T initiator);
        public delegate void UIEventAction<in T, in TA1>(T initiator, TA1 value);
        public delegate void UIEventAction<in T, in TA1, in TA2>(T initiator, TA1 value1, TA2 value2);

        public delegate TO UIEventFunc<in T, out TO, in TA1>(T initiator, TA1 value1);

        public event UIEventAction<UIElement>? OnTick;
        public event UIEventAction<UIElement, SceneGraphState>? OnProcess;
        public event UIEventAction<UIElement, InputEvent>? OnInput;
        public event UIEventAction<UIElement, InputEvent>? OnUnhandledInput;

        #endregion
    }
}
