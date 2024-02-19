using DasMulli.DataBuilderGenerator;

using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Groups.Layout {

    [GenerateDataBuilder]
    public class Stack : GroupPrefab {
        public override string Name => "Stack";
        public override string Description => "A horizontal or vertical stack of elements";
        public override UIElement Instantiate() {
            UIStack element = new UIStack(MinimumDistance, IsHorizontal);
            this.ApplyPropertyValues(element);

            InstantiateChildren(element);

            return element;
        }

        public Property<bool> IsHorizontal { get; set; } = new Property<bool>(nameof(IsHorizontal), "If true, the stack will be horizontal, otherwise it will be vertical");
        public Property<float> MinimumDistance { get; set; } = new Property<float>(nameof(MinimumDistance), "The minimum amount of space between elements in the stack");
    }
}
