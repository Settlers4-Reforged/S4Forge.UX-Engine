using Forge.Native;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Input {
    public struct InputEvent {
        /// <summary>
        /// The type of input event
        /// </summary>
        public InputType Type { get; init; }

        /// <summary>
        /// The key that was pressed, when it's a Key* event
        /// </summary>
        public Keys? Key { get; init; }
        /// <summary>
        /// The character representation of the key that was pressed, when applicable
        /// </summary>
        public char? Character { get; init; }
        /// <summary>
        /// The mouse wheel scroll amount, when it's a MouseWheel event
        /// </summary>
        public Vector2 Scroll { get; init; }

        public WndProcMsg? WindowsMessage { get; init; }

        /// <summary>
        /// Whether the event has been handled. Set to true to notify upper elements to prevent further processing.
        /// <br/>
        /// Handling MouseEnter or MouseLeave events will toggle the hover state of the element.
        /// </summary>
        public bool IsHandled { get; set; }
    }

}
