using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Plugin {
    /// <summary>
    /// A scene prefab that also automatically gets inserted into the scene tree after building.
    /// Use this in conjunction with <see cref="Forge.UX.UI.Prefabs.Groups.Menu"/> to build a scene for only specific game menus.
    /// </summary>
    public interface IPluginScene : IPluginPrefab {
        public GroupPrefab Group => this.Prefab as GroupPrefab ?? throw new InvalidOperationException();
    }
}
