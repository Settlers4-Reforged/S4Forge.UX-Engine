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
        MouseClickDown = 0b0000_0001,
        MouseClickUp = 0b0000_0010,
        MouseClick = MouseClickDown | MouseClickUp,
        MouseWheel = 0b0000_0100,
        MouseMove = 0b0000_1000,
        Mouse = MouseClick | MouseWheel | MouseMove,
        Keyboard = 0b0001_0000,
        All = 0b1111_1111
    }
}
