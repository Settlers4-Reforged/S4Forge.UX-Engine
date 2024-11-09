using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Plugin {
    public interface IPluginScene : IPluginPrefab {
        public GroupPrefab Group => this.Prefab as GroupPrefab ?? throw new InvalidOperationException();
    }
}
