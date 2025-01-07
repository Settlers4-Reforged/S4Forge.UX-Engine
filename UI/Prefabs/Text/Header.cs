using DasMulli.DataBuilderGenerator;

using Forge.UX.Rendering.Text;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Text {
    [GenerateDataBuilder]
    public class Header : TextPrefab {
        public override string Name => "Header";
        public override string Description => "A header text element.";

        protected override void OverrideDefaults() {
            base.OverrideDefaults();

            this.TextSize.Default = TextStyleSize.Large;
            this.TextHorizontalAlignment.Default = TextStyleAlignment.Center;
            this.TextType.Default = TextStyleType.Bold;
        }
    }
}
