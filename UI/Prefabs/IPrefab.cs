using Forge.UX.UI.Elements;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs {

    /// <summary>
    /// A prefab is a pre styled set of UI Elements
    /// </summary>
    public interface IPrefab {
        /// <summary>
        /// The name of the prefab - also used as node name in file deserialization
        /// </summary>
        public string Name { get; }
        public string Description { get; }

        /// <summary>
        /// Instantiates this prefab instance with default properties
        /// </summary>
        public UIElement Instantiate();
        public T Instantiate<T>() where T : UIElement;

        public event Action<UIElement>? Instantiated;

        public IEnumerable<IProperty> GetProperties();

        public IPrefab Clone();
    }
}
