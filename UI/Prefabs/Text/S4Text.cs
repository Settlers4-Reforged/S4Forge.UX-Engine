using DasMulli.DataBuilderGenerator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Prefabs.Text {

    [GenerateDataBuilder]
    public class S4Text : TextPrefab {
        public override string Name => "S4Text";
        public override string Description => "A generic text element";
    }
}
