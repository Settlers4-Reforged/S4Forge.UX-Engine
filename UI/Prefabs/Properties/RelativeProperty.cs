﻿using Forge.UX.UI.Elements;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Schema;

namespace Forge.UX.UI.Prefabs.Properties {

    [DebuggerDisplay("RelativeProperty {Name} | Value = {Value}, Relative = {PositionMode}")]
    public sealed class RelativeProperty : Property<float> {
        private PositioningMode? _positionMode = null;
        public PositioningMode PositionMode {
            get => _positionMode ?? DefaultMode;
            set => _positionMode = value;
        }

        public PositioningMode DefaultMode { get; set; } = PositioningMode.Normal;

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
            value = value.Replace("vp", "");

            bool success = base.Parse(value);

            if (success && PositionMode.HasFlag(PositioningMode.Relative)) {
                Value /= 100.0f;
            }

            return success;
        }
    }
}
