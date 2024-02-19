using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs {
    public abstract class GroupPrefab : ElementPrefab {

        #region Properties

        public Property<bool> ClipContent { get; set; } = new(nameof(ClipContent),
            "Whether the group should hide/clip content that is outside it's bounds");

        #endregion

        protected void InstantiateChildren(UIGroup group) {
            foreach (IPrefab prefab in ChildPrefabs) {
                group.Elements.Add(prefab.Instantiate());
            }
        }

        public List<IPrefab> ChildPrefabs { get; set; } = new List<IPrefab>();


        protected override void ApplyPropertyValues(UIElement element) {
            base.ApplyPropertyValues(element);

            if (element is UIGroup g) {
                g.ClipContent = ClipContent;
            } else {
                throw new InvalidOperationException($"Tried to apply group prefab properties from '{Name}' to non UI group element '{element.Id}'!");
            }
        }
    }
}
