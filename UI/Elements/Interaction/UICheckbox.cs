using Forge.UX.Input;
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

        public override void Input(ref InputEvent @event) {
            base.Input(ref @event);
            if (!Visible) return;
            if (!Enabled) return;

            if (@event is not { Key: Keys.LButton, Type: InputType.KeyUp }) return;

            IsChecked = !IsChecked;
            Interact();
            @event.IsHandled = true;
        }


        /// <summary>
        /// Binding for the value of the checkbox. Get's called during initialization
        /// </summary>
        public event BindableGetter<bool>? BindingGetValue;

        /// <summary>
        /// Gets called when the value of the checkbox changes
        /// </summary>
        public event ValueObserverSetter<bool>? OnValueChange;
    }
}
