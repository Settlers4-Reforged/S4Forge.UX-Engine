using DryIoc;

using Forge.Config;
using Forge.S4;
using Forge.S4.Types;
using Forge.UX.S4;
using Forge.UX.S4.Types.Native;
using Forge.UX.UI.Prefabs;

using System;

namespace Forge.UX.UI {
    public class UIEngine {
        public UIEngine() {
            RegisterDependencies();
        }

        internal void RegisterDependencies() {
            DI.Dependencies.Register<UICallbacks>(Reuse.Singleton);
            DI.Dependencies.Register<SceneBuilder>(Reuse.Singleton);
            DI.Dependencies.Register<SceneManager>(Reuse.Singleton);

            PrefabManager prefabManager = new PrefabManager();
            prefabManager.RegisterDefaultPrefabs();
            DI.Dependencies.RegisterInstance<PrefabManager>(prefabManager);
        }

    }
}
