using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Prefabs {

    /// <summary>
    /// A prefab that registers itself via reflection.
    /// Mark any class with this attribute to have it automatically registered to the prefab manager.
    /// </summary>
    /// <remarks>Only valid for instantiatable IPlugin classes</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrefabAttribute : Attribute {
        public string TagName { get; }


        /// <param name="TagName">
        /// The xml identifier for this prefab, e.g. <s4-window></s4-window>
        /// <br/><br/>
        /// <remarks>Should be in kebab-case (e.g. s4-window)</remarks>
        /// </param>
        public PrefabAttribute(string TagName) {
            this.TagName = TagName;
        }
    }
}
