using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Forge.UX.UI.Prefabs.Properties {
    /// <summary>
    /// A Property set by the node value
    /// </summary>
    [DebuggerDisplay("ValueProperty {Name} | Value = {Value}")]
    public class ValueProperty : Property<string> {
        public override bool Parse(XmlNode node) {
            return Parse(node.Value);
        }
    }
}
