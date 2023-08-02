using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Elements.Interaction {
    internal interface IUIInteractable {

        /// <summary>
        /// Whether this element can be interacted with
        /// </summary>
        bool Enabled { get; set; }


        /// <summary>
        /// Triggered when the element is interacted with - interaction is defined by the implementing element (e.g. button => click)
        /// </summary>
        Action<UIElement /*parent*/>? OnInteract { get; set; }
    }
}
