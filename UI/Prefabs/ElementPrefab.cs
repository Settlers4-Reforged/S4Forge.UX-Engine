using Forge.UX.UI.Elements;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Forge.UX.UI.Prefabs {
    public abstract class ElementPrefab : IPrefab {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract UIElement Instantiate();
        public T Instantiate<T>() where T : UIElement {
            return (T)Instantiate();
        }

        protected virtual void OverrideDefaults() { }

        #region Properties

        public Property<string> Id { get; set; } = new(nameof(Id), "Unique identifier for the element", () => Guid.NewGuid().ToString());

        public RelativeProperty X { get; set; } = new(nameof(X), "X position of the element");
        public RelativeProperty Y { get; set; } = new(nameof(Y), "Y position of the element");

        public RelativeProperty Width { get; set; } = new(nameof(Width), "Width of the element");
        public RelativeProperty Height { get; set; } = new(nameof(Height), "Height of the element");

        public Property<bool> Visible { get; set; } = new(nameof(Visible), "Whether the element is visible", true);

        public Property<bool> IgnoreMouse { get; set; } = new(nameof(IgnoreMouse), "Whether to ignore mouse inputs - affects children", false);

        public Property<int> ZIndex { get; set; } = new(nameof(ZIndex), "Z-Index of element", 0);

        protected virtual void ApplyPropertyValues(UIElement element) {
            this.OverrideDefaults();

            element.PositionMode = (
                X,
                Y
            );

            element.SizeMode = (
                Width,
                Height
            );

            element.Position = new Vector2(X, Y);
            element.Size = new Vector2(Width, Height);

            element.Visible = Visible;
            element.IgnoresMouse = IgnoreMouse;

            element.ZIndex = ZIndex;

            element.Id = Id.Value ?? Id.Default!;
        }

        #endregion

        public IEnumerable<IProperty> GetProperties() {
            List<IProperty> properties = this.GetType().GetFields()
                .Select(prop => {
                    if (prop.FieldType.GetInterfaces().Contains(typeof(IProperty))) {
                        return (IProperty)prop.GetValue(this)!;
                    }

                    return null;
                })
                .Concat(
                    this.GetType().GetProperties().Select(prop => {
                        if (prop.PropertyType.GetInterfaces().Contains(typeof(IProperty))) {
                            return (IProperty)prop.GetValue(this)!;
                        }
                        return null;
                    }))
                .Where(prop => prop != null).ToList()!;

            return properties;
        }

        public IPrefab Clone() {
            return (IPrefab)Activator.CreateInstance(this.GetType());
        }
    }
}
