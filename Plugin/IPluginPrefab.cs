using Forge.UX.UI;
using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Plugin {
    public interface IPluginPrefab {
        public IPrefab? Prefab { get; }
        public bool AutoRegister { get; }

        public void Build(SceneBuilder builder);
        public void AfterSceneTreeAdd() { }
    }
}
