using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static Forge.UX.Input.IInputManager;

namespace Forge.UX.Input {
    public class TestInputManager : IInputManager {
        public float MouseScroll { get; set; }
        public bool TextInputActive { get; set; }
        public bool ChatInputActive { get; set; }
        public Vector2 MousePosition { get; set; }
        public Vector2 MouseDelta { get; set; }
        public event InputEventHandlerCallback InputEventHandler;

        public void Init() {
            return;
        }
        public void Update() {
            return;
        }

        public bool IsMouseInRectangle(Vector4 rect) {
            return true;
        }

        public bool IsMouseOnScreen() {
            return true;
        }

        public bool IsKeyDown(Keys key) {
            return true;
        }

        public bool IsKeyUp(Keys key) {
            return true;
        }

        public bool IsKeyHeld(Keys key) {
            return true;
        }

        public bool RegisterKeybind(Keys key, bool overrideExisting, Action action) {
            return true;
        }

        public bool RegisterKeybind(IList<Keys> keys, bool overrideExisting, Action action) {
            return true;
        }

        public bool RemoveKeybind(Keys key) {
            return true;
        }

        public bool RemoveKeybind(IList<Keys> keys) {
            return true;
        }

        public void AddInputBlockingMiddleware(InputBlockMiddleware middleware) {
            return;
        }

        public void RemoveInputBlockingMiddleware(InputBlockMiddleware middleware) {
            return;
        }
    }
}
