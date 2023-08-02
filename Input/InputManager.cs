using Forge.UX.Native;

using System;
using System.Numerics;

namespace Forge.UX.Input {
    public static class InputManager {
        //Mouse:
        public static bool IsMouseInRectangle(Vector4 rect) {
            throw new NotImplementedException();
        }

        public static float GetMouseScroll() {
            throw new NotImplementedException();
        }

        //Key related input:
        public static bool IsKeyDown(Keys key) {
            throw new NotImplementedException();
        }

        public static bool IsKeyUp(Keys key) {
            throw new NotImplementedException();
        }

        public static bool IsKeyHeld(Keys key) {
            throw new NotImplementedException();
        }

        //Original Game Input Blocking:
        public static void ClearBlockFlags() {
            throw new NotImplementedException();
        }

        public static void BlockInput(EventBlockFlags flags) {
            throw new NotImplementedException();
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
