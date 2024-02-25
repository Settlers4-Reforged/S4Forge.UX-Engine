using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Forge.UX.UI.Prefabs.Properties {
    [DebuggerDisplay("RelativeVector2Property {Name} | Value = {Value.X},{Value.Y}")]
    public class RelativeVector2Property : Property<Vector2> {
        public RelativeProperty X, Y;

        public override Vector2 Value {
            get {
                float x = this.X.IsSet ? this.X : this.Default.X;
                float y = this.Y.IsSet ? this.Y : this.Default.Y;

                return new Vector2(x, y);
            }
            set {
                X.Value = value.X;
                Y.Value = value.Y;
            }
        }

        protected RelativeVector2Property() {
            X ??= new RelativeProperty("X", "X part of " + Name);
            Y ??= new RelativeProperty("Y", "Y part of " + Name);
        }

        public RelativeVector2Property(string name, string description, string xName = "X", string yName = "Y") : base(name, description) {
            X = new RelativeProperty(xName, xName + " part of " + name);
            Y = new RelativeProperty(yName, yName + " part of " + name);
        }

        public RelativeVector2Property(string name, string description, RelativeProperty x, RelativeProperty y) : base(name, description) {
            X = x;
            Y = y;
        }

        public RelativeVector2Property(RelativeProperty x, RelativeProperty y) {
            X = x;
            Y = y;
        }

        public static implicit operator RelativeVector2Property((RelativeProperty x, RelativeProperty y) value) {
            return new RelativeVector2Property(value.x, value.y);
        }

        public override bool Parse(XmlNode node) {
            base.Parse(node);
            bool foundCompound = IsSet;
            if (foundCompound) return true;

            bool x = X.Parse(node);
            bool y = Y.Parse(node);
            if (x || y) {
                IsSet = true;
            }

            return x && y;
        }

        public override bool Parse(string value) {
            string[] parts = value.Split(',');

            if (parts.Length != 2) return false;
            bool xParsed = X.Parse(parts[0].Trim());
            bool yParsed = Y.Parse(parts[1].Trim());

            return xParsed && yParsed;
        }
    }
}
