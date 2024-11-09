using Forge.UX.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Elements.Interaction {
    public class UIRadioButton<T> : UICheckbox {
        public string LinkId { get; set; }
        public T Value { get; set; }

        public UIRadioButton(string linkId, T value) {
            LinkId = linkId;
            Value = value;
        }

        public override void Input(ref InputEvent @event) {
            base.Input(ref @event);
            if (!Visible) return;
            if (!Enabled) return;

            if (@event is not { Key: Keys.LButton, Type: InputType.KeyUp }) return;
            // Don't allow unchecking
            if (IsChecked) return;

            IsChecked = true;
            Interact();

            @event.IsHandled = true;
        }
    }
}
