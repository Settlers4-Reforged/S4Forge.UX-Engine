using Forge.UX.UI.Elements;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Schema;

namespace Forge.UX.UI.Prefabs.Properties {

    [DebuggerDisplay("RelativeProperty {Name} | Value = {Value}, Relative = {IsRelative}")]
    public sealed class RelativeProperty : Property<float> {
        public PositioningMode PositionMode;

        private RelativeProperty() { }
        public RelativeProperty(string name, string description) : base(name, description) { }

        public static implicit operator RelativeProperty((float value, PositioningMode mode) v) {
            return new RelativeProperty { Value = v.value, PositionMode = v.mode };
        }
        public static implicit operator RelativeProperty(float value) {
            return new RelativeProperty { Value = value, PositionMode = PositioningMode.Normal };
        }
        public static implicit operator RelativeProperty(string value) {
            RelativeProperty property = new RelativeProperty();
            property.Parse(value);
            return property;
        }

        public static implicit operator PositioningMode(RelativeProperty p) {
            return p.PositionMode;
        }

        public override bool Parse(string value) {
            PositionMode = value switch {
                _ when value.EndsWith("%") => PositioningMode.Relative,
                _ when value.EndsWith("vp") => PositioningMode.Absolute, // vp => view position
                _ when value.EndsWith("vp%") => PositioningMode.AbsoluteRelative,
                _ => PositioningMode.Normal
            };

            value = value.Replace("%", "");
            value = value.Replace("px", "");

            bool success = base.Parse(value);

            if (success && PositionMode.HasFlag(PositioningMode.Relative)) {
                Value /= 100.0f;
            }

            return success;
        }
    }
}
