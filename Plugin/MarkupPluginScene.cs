using Forge.UX.UI;
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
        public IPrefab? Prefab { get; private set; }
        public abstract bool AutoRegister { get; }

        public abstract string Markup { get; }

        /// <summary>
        /// Load markup from a file. Can be used to directly load markup from a file.
        /// </summary>
        /// <param name="path">Either a relative path, or an absolute path. The path is by default relative to the owned plugin assembly</param>
        /// <returns>The file contents</returns>
        protected string FromFile(string path) {
            string absolutePath;
            if (Path.IsPathFullyQualified(path)) {
                absolutePath = path;
            } else {
                Assembly assembly = this.GetType().Assembly;
                string basePath = Path.GetDirectoryName(assembly.Location) ?? Environment.CurrentDirectory;
                absolutePath = Path.Combine(basePath, path);
            }

            return File.ReadAllText(absolutePath);
        }

        public void Build(SceneBuilder builder) {
            if (!builder.CreateScene(Markup, out GroupPrefab? scene) || scene == null) {
                throw new InvalidOperationException("Failed to create scene from markup");
            }

            Prefab = scene;
        }
    }
}

