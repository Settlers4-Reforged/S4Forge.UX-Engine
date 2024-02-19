using DryIoc;

using Forge.Config;
using Forge.UX.Debug;
using Forge.UX.UI.Prefabs;

namespace Forge.UX.UI {
    public class UIEngine {
        public UIEngine() {
            RegisterDependencies();
        }

        internal void RegisterDependencies() {
            DI.Dependencies.Register<SceneBuilder>(Reuse.Singleton);
            DI.Dependencies.Register<SceneManager>(Reuse.Singleton);

            PrefabManager prefabManager = new PrefabManager();
            prefabManager.RegisterDefaultPrefabs();
            DI.Dependencies.RegisterInstance<PrefabManager>(prefabManager);

#if DEBUG
            DI.Dependencies.Register<UIDebugWindow>();
#endif
        }
    }
}
