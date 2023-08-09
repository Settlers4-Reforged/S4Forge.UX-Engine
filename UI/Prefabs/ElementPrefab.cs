using Forge.UX.UI.Elements;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Forge.UX.UI.Prefabs {
    public abstract class ElementPrefab : IPrefab {
        public abstract string Name { get; }
        public abstract UIElement Create();

        #region Properties

        public RelativeProperty X = new(nameof(X), "X position of the element");
        public RelativeProperty Y = new(nameof(Y), "Y position of the element");

        public RelativeProperty Width = new(nameof(Width), "Width of the element");
        public RelativeProperty Height = new(nameof(Height), "Height of the element");

        public Property<bool> PositionAbsolute = new(nameof(PositionAbsolute), "Whether the position is relative to group space (false) or screen space (true)", false);

        public Property<bool> IgnoreMouse = new(nameof(IgnoreMouse), "Whether to ignore mouse inputs - affects children", false);

        public Property<int> ZIndex = new(nameof(ZIndex), "Z-Index of element", 0);

        protected void ApplyPropertyValues(UIElement element) {
            UIElement.PositioningMode posMode = PositionAbsolute ? UIElement.PositioningMode.Absolute : UIElement.PositioningMode.Normal;

            element.PositionMode = (
                posMode | (X.IsRelative ? UIElement.PositioningMode.Relative : UIElement.PositioningMode.Normal),
                posMode | (Y.IsRelative ? UIElement.PositioningMode.Relative : UIElement.PositioningMode.Normal)
            );

            element.SizeMode = (
                posMode | (Width.IsRelative ? UIElement.PositioningMode.Relative : UIElement.PositioningMode.Normal),
                posMode | (Height.IsRelative ? UIElement.PositioningMode.Relative : UIElement.PositioningMode.Normal)
            );

            element.Position = new Vector2(X, Y);
            element.Size = new Vector2(Width, Height);

            element.IgnoresMouse = IgnoreMouse;

            element.ZIndex = ZIndex;
        }

        #endregion

        public IEnumerable<IProperty> GetProperties() {
            List<IProperty> properties = new();

            return properties;
        }
    }
}
