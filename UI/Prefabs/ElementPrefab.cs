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

        public RelativeVector2Property Position { get; set; } = new(nameof(Position), "Position of the element", "X", "Y");
        public RelativeVector2Property Size { get; set; } = new(nameof(Size), "Size of the element", "Width", "Height");

        public Property<bool> Visible { get; set; } = new(nameof(Visible), "Whether the element is visible", true);

        public Property<int> ZIndex { get; set; } = new(nameof(ZIndex), "Z-Index of element", 0);

        protected virtual void ApplyPropertyValues(UIElement element) {
            this.OverrideDefaults();

            element.PositionMode = (
                Position.X,
                Position.Y
            );

            element.SizeMode = (
                Size.X,
                Size.Y
            );

            element.Position = Position;
            element.Size = Size;

            element.Visible = Visible;

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
            return (IPrefab)Activator.CreateInstance(this.GetType())!;
        }
    }
}
