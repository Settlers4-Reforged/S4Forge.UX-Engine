using Forge.Native;
using Forge.UX.Native;

using System;
using System.Numerics;

namespace Forge.UX.Input {
    public static class InputManager {
        #region Mouse

        public static bool IsMouseInRectangle(Vector4 rect) {
            Vector2 mousePosition = GetMousePosition();
            return mousePosition.X >= rect.X && mousePosition.X <= rect.X + rect.Z && mousePosition.Y >= rect.Y && mousePosition.Y <= rect.Y + rect.W;
        }

        public static float GetMouseScroll() {
            throw new NotImplementedException();
        }

        private static Vector2 prevMousePosition;
        public static Vector2 GetMousePosition() {
            User32.GetCursorPos(out User32.Pos point);
            Vector2 mousePosition = new Vector2(point.X, point.Y);
            prevMousePosition = mousePosition;
            return mousePosition;
        }

        public static Vector2 MouseDelta => GetMousePosition() - prevMousePosition;


        #endregion

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
