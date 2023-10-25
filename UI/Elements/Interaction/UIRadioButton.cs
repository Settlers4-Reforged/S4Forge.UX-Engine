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

        public override void OnMouseClickUp(int mb) {
            // Don't allow unchecking
            if (IsChecked) return;

            IsChecked = true;
            OnInteract?.Invoke(this);
        }
    }
}
