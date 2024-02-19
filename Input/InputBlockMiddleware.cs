using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Input {
    public class InputBlockMiddleware {
        public readonly int Priority;
        public bool IsBlocking;
        public Func<EventBlockFlags, EventBlockFlags> Middleware;
        public InputBlockMiddleware(bool isBlocking, Func<EventBlockFlags, EventBlockFlags> middleware, int priority = 0) {
            IsBlocking = isBlocking;
            Middleware = middleware;
            Priority = priority;
        }
    }
    [Flags]
    public enum EventBlockFlags : int {
        None = 0b0000_0000,
        MouseClick = 0b0000_0001,
        MouseWheel = 0b0000_0010,
        Mouse = 0b0000_0100,
        Keyboard = 0b0000_1000,
        All = 0b1111_1111
    }
}
