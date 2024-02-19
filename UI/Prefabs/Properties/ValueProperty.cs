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
        protected ValueProperty() : base() { }
        public ValueProperty(string name, string description) : base(name, description) { }


        public static implicit operator ValueProperty(string value) {
            return new ValueProperty { Value = value };
        }

        public override bool Parse(XmlNode node) {
            string innerText = node.InnerText;
            return Parse(innerText);
        }
    }
}
