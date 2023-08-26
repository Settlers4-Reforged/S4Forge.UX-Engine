using Forge.Logging;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Forge.UX.UI {
    /// <summary>
    /// Helper to create scenes from XML files based on prefabs
    /// </summary>
    public class SceneBuilder {
        public PrefabManager PrefabManager;
        public SceneManager SceneManager;

        public SceneBuilder(SceneManager manager) {
            PrefabManager = new PrefabManager();
            PrefabManager.RegisterDefaultPrefabs();

            SceneManager = manager;
        }

        public bool CreateScene(string config) {
            //TODO: Validate input

            XmlDocument tree = new XmlDocument();
            try {
                tree.LoadXml(config);
            } catch (Exception e) {
                Logger.LogError(e, "Failed to parse scene into xml document");
                return false;
            }

            UIGroup scene;

            try {
                scene = ParseScene(tree);
            } catch (Exception e) {
                Logger.LogError(e, "Failed to parse scene");
                return false;
            }

            SceneManager.AddRootElement(scene);

            return true;
        }

        public UIGroup ParseScene(XmlDocument tree) {
            if (tree.FirstChild == null) throw new ArgumentException("Tried to parse scene without a parent node");

            if (ParseNode(tree.FirstChild) is UIGroup g) {
                return g;
            }

            throw new NotSupportedException("Tried to load scene without a UIGroup as parent");
        }

        public UIElement? ParseNode(XmlNode node) {
            IPrefab prefab = PrefabManager.GetPrefabByName(node.Name) ?? throw new ArgumentException("Found node without a registered prefab!", node.Name);

            foreach (IProperty property in prefab.GetProperties()) {
                property.Parse(node);
            }

            UIElement nodeElement;
            try {
                nodeElement = prefab.Instantiate();
            } catch (Exception e) {
                throw new InvalidOperationException($"Failed to instantiate prefab {prefab.Name}", e);
            }

            if (nodeElement is not UIGroup g) return nodeElement;

            foreach (XmlNode childNode in node.ChildNodes) {
                UIElement? childElement = ParseNode(childNode);
                if (childElement != null) {
                    g.Elements.Add(childElement);
                }
            }

            return nodeElement;
        }

        public bool CreateSceneFromFile(string file) {
            if (!File.Exists(file)) {
                throw new FileNotFoundException($"Tried to create a scene from a non existing file! File: {file}");
            }

            return CreateScene(File.ReadAllText(file));
        }
    }
}
