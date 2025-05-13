using Forge.Config;
using Forge.Engine;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Plugin {
    public abstract class MarkupPluginScene : IPluginScene {
        public abstract string TagName { get; }
        public IPrefab? Prefab { get; private set; }
        public abstract bool AutoRegister { get; }

        public abstract string Markup { get; }

        /// <summary>
        /// Load markup from a file. Can be used to directly load markup from a file.
        /// </summary>
        /// <param name="path">Either a relative path, or an absolute path. On a relative path, the base is determined by the PluginEnvironment&lt;TPlugin&gt;</param>
        /// <returns>The file contents</returns>
        protected string FromFile<TModule>(string path) where TModule : IModule {
            string absolutePath;
            if (Path.IsPathFullyQualified(path)) {
                absolutePath = path;
            } else {
                string pluginBasePath = DI.Resolve<ModuleEnvironment<TModule>>().Path;
                absolutePath = Path.Combine(pluginBasePath, path);
            }

            return File.ReadAllText(absolutePath);
        }

        public bool Build(SceneBuilder builder) {
            if (!builder.CreateScene(Markup, out GroupPrefab? scene) || scene == null) {
                throw new InvalidOperationException("Failed to create scene from markup");
            }

            Prefab = scene;
            Prefab.Instantiated += OnInstantiated;

            return true;
        }

        public virtual void OnInstantiated(UIElement element) { }
    }
}

