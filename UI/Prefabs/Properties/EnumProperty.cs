using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Properties {
    public class EnumProperty<T> : Property<T> where T : struct, Enum {

        private EnumProperty() { }
        public EnumProperty(string name, string description) : base(name, description) { }

        public override bool Parse(string value) {
            T output;
            bool success = Enum.TryParse(value, true, out output);
            if (success) {
                Value = output;
            }

            return success;
        }
    }
}
