using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Interaction;
using Forge.UX.UI.Prefabs.Properties;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Groups.Interaction {
    public class RadioButtonGroup : GroupPrefab {
        public override string Name => "RadioButtonGroup";
        public override string Description => "A group of radio buttons. Radio buttons get automatically linked by their LinkId property.";
        public override UIElement Instantiate() {
            UIRadioButtonGroup<string> group = new UIRadioButtonGroup<string>(LinkId.Value!);
            ApplyPropertyValues(group);

            InstantiateChildren(group);

            return group;
        }

        public Property<string> LinkId = new Property<string>(nameof(LinkId), "The ID of the RadioButtonGroup to link to") { Required = true };
    }
}
