using DasMulli.DataBuilderGenerator;

using Forge.UX.Rendering.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Prefabs.Text {

    [Prefab("s4-text")]
    [GenerateDataBuilder]
    public class S4Text : TextPrefab {
        public override string Name => "S4Text";
        public override string Description => "A generic text element";

        protected override void OverrideDefaults() {
            base.OverrideDefaults();

            TextType.Default = TextStyleType.Bold;
            TextVerticalAlignment.Default = TextStyleAlignment.Start;
        }
    }
}
