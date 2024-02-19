using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Input {
    public struct Keybind {
        public readonly IList<Keys> Keys;
        public readonly Action Action;

        public Keybind(IList<Keys> keys, Action action) {
            Keys = keys;
            Action = action;
        }
    }
}
