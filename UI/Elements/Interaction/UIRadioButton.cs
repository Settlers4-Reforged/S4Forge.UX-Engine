using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Elements.Interaction {
    public class UIRadioButton : UICheckbox {
        public string LinkId { get; set; }

        public UIRadioButton(string linkId) {
            LinkId = linkId;
        }
    }
}
