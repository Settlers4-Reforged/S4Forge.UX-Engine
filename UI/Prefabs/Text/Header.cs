using DasMulli.DataBuilderGenerator;

using System;
using System.Collections.Generic;
using System.Text;

namespace Forge.UX.UI.Prefabs.Text {
    [GenerateDataBuilder]
    public class Header : TextPrefab {
        public override string Name => "Header";
        public override string Description => "A header text element.";
    }
}
