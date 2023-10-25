using System;

namespace Forge.UX.UI.Elements.Interaction {

    /// <summary>
    /// A UI Element that can be interacted with
    /// </summary>
    public interface IUIInteractable {

        /// <summary>
        /// Whether this element can be interacted with
        /// </summary>
        bool Enabled { get; set; }


        /// <summary>
        /// Triggered when the element is interacted with - interaction is defined by the implementing element (e.g. button => click)
        /// </summary>
        Action<UIElement /*parent*/>? OnInteract { get; set; }
    }

    /// <summary>
    /// A UI Element that can be bound to a value
    /// </summary>
    public interface IUIBindable<TValue> {
        /// <summary>
        /// Retrieves the current value of the underlying binding.
        /// <br/>
        /// Usually called during initialization, or sometimes during rendering.
        /// See implementation for when binding gets updated
        /// </summary>
        Func<UIElement, TValue>? BindingGetValue { get; set; }
    }

    public interface IUIValueObserver<TValue> {
        /// <summary>
        /// Gets executed when the value changes (e.g. when the user changes the value)
        /// </summary>
        Action<UIElement, TValue>? OnValueChange { get; set; }
    }
}
