using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Schema;

namespace Forge.UX.UI.Prefabs.Properties {

    [DebuggerDisplay("RelativeProperty {Name} | Value = {Value}, Relative = {IsRelative}")]
    public sealed class RelativeProperty : Property<float> {
        public bool IsRelative;

        private RelativeProperty() { }
        public RelativeProperty(string name, string description) : base(name, description) { }

        public override bool Parse(string value) {
            IsRelative = value.Contains("%");

            if (IsRelative) {
                value = value.Replace("%", "");
            }

            bool success = base.Parse(value);

            if (success && IsRelative) {
                Value /= 100;
            }

            return success;
        }
    }
}
