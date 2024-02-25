using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Forge.UX.UI.Prefabs.Properties {
    [DebuggerDisplay("EnumProperty {Name} | Value = {Value}")]
    public class EnumProperty<T> : Property<T> where T : struct, Enum {

        private EnumProperty() { }
        public EnumProperty(string name, string description) : base(name, description) { }

        public static implicit operator EnumProperty<T>(T value) {
            return new EnumProperty<T> { Value = value };
        }

        public override bool Parse(string value) {
            bool success = Enum.TryParse(value, true, out T output);
            if (success) {
                Value = output;
            }

            return success;
        }
    }
}
