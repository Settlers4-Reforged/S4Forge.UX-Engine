using Forge.UX.UI;
using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Plugin {
    /// <summary>
    /// A prefab that registers itself via reflection.
    /// </summary>
    public interface IPluginPrefab {
        /// <summary>
        /// The cached prefab when the prefab is built.
        /// Set to the builder output when successfully built in <see cref="Build"/>.
        /// </summary>
        public IPrefab? Prefab { get; }

        /// <summary>
        /// Whether this prefab should be automatically registered to the prefab manager.
        /// </summary>
        public bool AutoRegister { get; }

        /// <summary>
        /// Build the prefab
        /// </summary>
        /// <returns>Whether the prefab actually built. Use this to conditionally build prefabs</returns>
        public bool Build(SceneBuilder builder);
    }
}
