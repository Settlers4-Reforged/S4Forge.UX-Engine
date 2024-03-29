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
        event UIElement.UIEvent<UIElement>? OnInteract;
    }

    public delegate TValue BindableGetter<out TValue>(UIElement sender);

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
        public event BindableGetter<TValue>? BindingGetValue;
    }

    public delegate void ValueObserverSetter<in TValue>(UIElement sender, TValue value);

    /// <summary>
    /// A UI Element that broadcasts its value changes
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IUIValueObserver<TValue> {
        /// <summary>
        /// Gets executed when the value changes (e.g. when the user changes the value)
        /// </summary>
        public event ValueObserverSetter<TValue>? OnValueChange;
    }
}
