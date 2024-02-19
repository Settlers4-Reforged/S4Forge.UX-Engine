using Forge.UX.Rendering;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Elements.Interaction {
    public class UICheckbox : UIButton, IUIBindable<bool>, IUIValueObserver<bool> {
        public bool IsChecked {
            get => holdStatus == State.Down;
            set {
                SetState(value ? State.Down : State.Up);
                OnValueChange?.Invoke(this, value);
            }
        }

        public UICheckbox() : base() {
            IsChecked = BindingGetValue?.Invoke(this) ?? false;
        }

        public override void OnMouseClickDown(int mb) { }

        public override void OnMouseClickUp(int mb) {
            IsChecked = !IsChecked;
            OnInteract?.Invoke(this);
        }

        /// <summary>
        /// Binding for the value of the checkbox. Get's called during initialization
        /// </summary>
        public Func<UIElement, bool>? BindingGetValue { get; set; }

        /// <summary>
        /// Gets called when the value of the checkbox changes
        /// </summary>
        public Action<UIElement, bool>? OnValueChange { get; set; }
    }
}
