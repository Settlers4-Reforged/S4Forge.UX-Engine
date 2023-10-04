using Forge.UX.Rendering;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Elements.Interaction {
    public class UICheckbox : UIButton {
        public bool IsChecked {
            get => holdStatus == State.Down;
            set => SetState(value ? State.Down : State.Up);
        }

        public override void OnMouseClickDown(int mb) { }

        public override void OnMouseClickUp(int mb) {
            IsChecked = !IsChecked;
            OnInteract?.Invoke(this);
        }

        public override void OnMouseLeave() {
            Effects &= ~Effects.Highlight;
        }
    }
}
