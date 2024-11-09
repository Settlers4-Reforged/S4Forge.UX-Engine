using Forge.Native;
using Forge.S4;
using Forge.UX.Native;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using static Forge.UX.Input.IInputManager;

namespace Forge.UX.Input {
    public interface IInputManager {
        float MouseScroll { get; }

        bool TextInputActive {
            get;
        }

        bool ChatInputActive {
            get;
        }

        Vector2 MousePosition { get; }
        Vector2 MouseDelta { get; }

        delegate void InputEventHandlerCallback(ref InputEvent e);

        event InputEventHandlerCallback InputEventHandler;

        void Init();
        void Update();

        bool IsMouseInRectangle(Vector4 rect);
        bool IsMouseOnScreen();

        /// <summary>
        /// Returns true if the key is was just pressed down.
        /// </summary>
        bool IsKeyDown(Keys key);

        /// <summary>
        /// Returns true if the key is was just released.
        /// </summary>
        bool IsKeyUp(Keys key);

        /// <summary>
        /// Returns true if the key is currently held down.
        /// </summary>
        bool IsKeyHeld(Keys key);

        /// <summary>
        /// Registers a keybind with the input manager.
        /// </summary>
        /// <returns>
        /// Returns true if the keybind was successfully registered, false otherwise.
        /// <br/>
        /// Reasons for failure include:
        /// <list type="bullet">
        /// <item>Keybind already registered, but overwrite is false</item>
        /// </list>
        /// </returns>
        bool RegisterKeybind(Keys key, bool overrideExisting, Action action);

        /// <inheritdoc cref="InputManager.RegisterKeybind(Forge.UX.Input.Keys,bool,System.Action)"/>
        bool RegisterKeybind(IList<Keys> keys, bool overrideExisting, Action action);

        /// <summary>
        /// Removes a keybind from the input manager.
        /// </summary>
        /// <returns>
        /// Returns true if the keybind was successfully removed, false otherwise.
        /// </returns>
        bool RemoveKeybind(Keys key);

        /// <inheritdoc cref="InputManager.RemoveKeybind(Forge.UX.Input.Keys)"/>
        bool RemoveKeybind(IList<Keys> keys);

        void AddInputBlockingMiddleware(InputBlockMiddleware middleware);
        void RemoveInputBlockingMiddleware(InputBlockMiddleware middleware);
    }

    public class InputManager : IInputManager {
        private HashSet<Keys> downKeys, heldKeys, upKeys;
        private int mouseScroll;

        public InputManager() {
            downKeys = new HashSet<Keys>();
            heldKeys = new HashSet<Keys>();
            upKeys = new HashSet<Keys>();
        }

        public void Init() {
#pragma warning disable CS0618 // Type or member is obsolete - InputManager requires a WndProc to be set
            User32.AddWndProc(InputHandler);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public bool TextInputActive {
            get => false; //TODO: implement
        }

        public bool ChatInputActive {
            get => false; //TODO: implement
        }

        bool InputHandler(WndProcMsg msg, UIntPtr wParam, UIntPtr lParam) {
            // When in Text input mode have a special input handler for WM_KEYDOWN events
            // TODO: handle special keys like "Escape" better
            if (TextInputActive && msg == WndProcMsg.WM_KEYDOWN) {
                char c = (char)wParam;
                // TODO: find out if this is the correct way to check for normal characters
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || char.IsWhiteSpace(c)) {
                    if (ChatInputActive) {
                        return false;
                    } else {
                        InputEvent inputEvent = new InputEvent() {
                            Type = InputType.TextInput,
                            Character = c,
                        };

                        InputEventHandler?.Invoke(
                            ref inputEvent
                        );
                        return inputEvent.IsHandled;
                    }
                }
            }

            // Only if at least one key just was pressed down we want to handle the input
            // This is to prevent the input from being handled multiple times
            // TODO: find out if some keybinds should be called for every frame a key is held down
            // Should then be handled by the Update method - makes it FPS dependent though
            foreach (Keybind keybind in keybinds.Where(keybind => keybind.Keys.All(IsKeyHeld) && keybind.Keys.Any(IsKeyDown))) {
                keybind.Action();
                // Remove all keys that are part of the keybind
                // This is to prevent the keybind from being called multiple times before the next update call
                // TODO: find out if this collides with key states for consuming clients
                downKeys.RemoveWhere(keybind.Keys.Contains);
                return false;
            }

            if (HandleKeyStates(msg, wParam))
                return true;

            return HandleInputBlocking(msg);
        }

        private bool HandleInputBlocking(WndProcMsg msg) {
            EventBlockFlags blockedInput = inputBlocks.Where(middleware => middleware.IsBlocking).OrderBy(m => m.Priority)
                .Aggregate(EventBlockFlags.None, (current, middleware) => middleware.Middleware(current));

            if ((blockedInput & EventBlockFlags.MouseClick) == EventBlockFlags.MouseClick) { // Mouse Clicks Blocked
                if (msg is >= WndProcMsg.WM_LBUTTONDOWN and <= WndProcMsg.WM_XBUTTONDBLCLK) {
                    //Not WM_MOUSEFIRST=WM_MOUSEMOVE because we want the game to still receive those events, see EventBlockFlags.MouseMove
                    return true;
                }
            }

            if ((blockedInput & EventBlockFlags.MouseClickDown) == EventBlockFlags.MouseClickDown) { // Mouse Clicks Down Blocked
                WndProcMsg[] mouseDowns = { WndProcMsg.WM_LBUTTONDOWN, WndProcMsg.WM_RBUTTONDOWN, WndProcMsg.WM_MBUTTONDOWN, WndProcMsg.WM_XBUTTONDOWN };
                if (mouseDowns.Contains(msg)) {
                    return true;
                }
            }

            if ((blockedInput & EventBlockFlags.MouseClickUp) == EventBlockFlags.MouseClickUp) { // Mouse Clicks Up Blocked
                WndProcMsg[] mouseUps = { WndProcMsg.WM_LBUTTONUP, WndProcMsg.WM_RBUTTONUP, WndProcMsg.WM_MBUTTONUP, WndProcMsg.WM_XBUTTONUP };
                if (mouseUps.Contains(msg)) {
                    return true;
                }
            }


            if ((blockedInput & EventBlockFlags.MouseWheel) == EventBlockFlags.MouseWheel) { // Mouse Wheel Blocked
                if (msg == WndProcMsg.WM_MOUSEWHEEL) {
                    return true;
                }
            }

            if ((blockedInput & EventBlockFlags.MouseMove) == EventBlockFlags.MouseMove) { // Mouse Movement Blocked
                if (msg is >= WndProcMsg.WM_MOUSEFIRST and <= WndProcMsg.WM_MOUSELAST) {
                    return true;
                }
            }

            if ((blockedInput & EventBlockFlags.Keyboard) == EventBlockFlags.Keyboard) { // Keyboard events Blocked
                if (msg is >= WndProcMsg.WM_KEYFIRST and <= WndProcMsg.WM_KEYLAST) {
                    return true;
                }
            }

            return false;
        }

        /// <returns>True when it should block that input</returns>
        private bool HandleKeyStates(WndProcMsg msg, UIntPtr wParam) {
            Keys key = Keys.None;
            bool up = false;

            int wp = (int)wParam;

            switch (msg) {
                case WndProcMsg.WM_MOUSEWHEEL: //Scroll:
                    mouseScroll = (wp) >> 0x10;
                    break;

                case WndProcMsg.WM_MOUSEMOVE:
                    // Handled in Update
                    return false;

                //Button up:
                case WndProcMsg.WM_KEYUP:
                    key = (Keys)wp;
                    up = true;
                    break;
                case WndProcMsg.WM_SYSKEYUP: //Alt Gr up
                    key = (Keys)wp;
                    up = true;
                    break;
                //Mouse up:
                case WndProcMsg.WM_LBUTTONUP: //Left
                    key = Keys.LButton;
                    HandleMouseDrag(false);
                    up = true;
                    break;
                case WndProcMsg.WM_RBUTTONUP: //Right
                    key = Keys.RButton;
                    HandleMouseDrag(false);
                    up = true;
                    break;
                case WndProcMsg.WM_MBUTTONUP: //Middle
                    key = Keys.MButton;
                    up = true;
                    break;

                //Keyboard button down:
                case WndProcMsg.WM_KEYDOWN: //Keyboard down:
                    key = (Keys)wp;
                    break;
                case WndProcMsg.WM_SYSKEYDOWN: //Alt Gr up
                    key = (Keys)wp;
                    break;
                //Mouse down:
                case WndProcMsg.WM_LBUTTONDOWN: //Left
                    key = Keys.LButton;
                    HandleMouseDrag(true);
                    break;
                case WndProcMsg.WM_RBUTTONDOWN: //Right
                    key = Keys.RButton;
                    HandleMouseDrag(true);
                    break;
                case WndProcMsg.WM_MBUTTONDOWN: //Middle
                    key = Keys.MButton;
                    break;

                default:
                    break;
            }


            InputEvent inputEvent;
            if (key != Keys.None) {
                if (up) {
                    heldKeys.Remove(key);
                    upKeys.Add(key);
                } else {
                    heldKeys.Add(key);
                    downKeys.Add(key);
                }

                inputEvent = new InputEvent() {
                    Type = up ? InputType.KeyUp : InputType.KeyDown,
                    Key = key,
                };
            } else if (msg == WndProcMsg.WM_MOUSEWHEEL) {
                inputEvent = new InputEvent() {
                    Type = InputType.MouseWheel,
                };
            } else {

                inputEvent = new InputEvent() {
                    Type = InputType.Windows,
                    WindowsMessage = msg,
                };
            }

            InputEventHandler?.Invoke(
                ref inputEvent
            );
            return inputEvent.IsHandled;
        }

        private void HandleMouseDrag(bool start) {
            if (start) {
                User32.SetCapture(GameValues.Hwnd);
            } else {
                User32.ReleaseCapture();
            }
        }

        public void Update() {
            downKeys.Clear();
            upKeys.Clear();
            mouseScroll = 0;

            foreach (Keys key in heldKeys) {
                InputEvent inputEvent = new InputEvent() {
                    Type = InputType.KeyHold,
                    Key = key,
                };

                InputEventHandler?.Invoke(
                    ref inputEvent
                );
            }

            prevMousePosition = currentMousePosition;
            User32.GetCursorPos(out User32.Pos point);
            User32.ScreenToClient(GameValues.Hwnd, ref point);
            currentMousePosition = new Vector2(point.X, point.Y);
        }

        #region Mouse

        public bool IsMouseInRectangle(Vector4 rect) {
            return MousePosition.X >= rect.X && MousePosition.X <= rect.X + rect.Z && MousePosition.Y >= rect.Y && MousePosition.Y <= rect.Y + rect.W;
        }

        public bool IsMouseOnScreen() {
            User32.Rect rect = new User32.Rect();
            User32.GetWindowRect(GameValues.Hwnd, ref rect);

            float width = rect.Z - rect.X;
            float height = rect.W - rect.Y;
            return IsMouseInRectangle(new Vector4(0, 0, width, height));
        }

        private Vector2 prevMousePosition;
        private Vector2 currentMousePosition;

        public Vector2 MousePosition => currentMousePosition;

        public Vector2 MouseDelta => MousePosition - prevMousePosition;

        public float MouseScroll => mouseScroll;


        #endregion

        //Key related input:

        /// <summary>
        /// Returns true if the key is was just pressed down.
        /// </summary>
        public bool IsKeyDown(Keys key) {
            return downKeys.Contains(key);
        }

        /// <summary>
        /// Returns true if the key is was just released.
        /// </summary>
        public bool IsKeyUp(Keys key) {
            return upKeys.Contains(key);
        }

        /// <summary>
        /// Returns true if the key is currently held down.
        /// </summary>
        public bool IsKeyHeld(Keys key) {
            return heldKeys.Contains(key);
        }

        public event InputEventHandlerCallback InputEventHandler;

        #region Kebinds

        List<Keybind> keybinds = new List<Keybind>();

        /// <summary>
        /// Registers a keybind with the input manager.
        /// </summary>
        /// <returns>
        /// Returns true if the keybind was successfully registered, false otherwise.
        /// <br/>
        /// Reasons for failure include:
        /// <list type="bullet">
        /// <item>Keybind already registered, but overwrite is false</item>
        /// </list>
        /// </returns>
        public bool RegisterKeybind(Keys key, bool overrideExisting, Action action) {
            return RegisterKeybind(new List<Keys>() { key }, overrideExisting, action);
        }

        /// <inheritdoc cref="RegisterKeybind(Forge.UX.Input.Keys,bool,System.Action)"/>
        public bool RegisterKeybind(IList<Keys> keys, bool overrideExisting, Action action) {
            if (keybinds.Any(k => k.Keys.SequenceEqual(keys))) {
                if (overrideExisting) {
                    keybinds.RemoveAll(k => k.Keys.SequenceEqual(keys));
                } else {
                    return false;
                }
            }

            keybinds.Add(new Keybind(keys, action));
            return true;
        }

        /// <summary>
        /// Removes a keybind from the input manager.
        /// </summary>
        /// <returns>
        /// Returns true if the keybind was successfully removed, false otherwise.
        /// </returns>
        public bool RemoveKeybind(Keys key) {
            return RemoveKeybind(new List<Keys>() { key });
        }

        /// <inheritdoc cref="RemoveKeybind(Forge.UX.Input.Keys)"/>
        public bool RemoveKeybind(IList<Keys> keys) {
            return keybinds.RemoveAll(k => k.Keys.SequenceEqual(keys)) > 0;
        }

        #endregion


        #region InputBlocking

        List<InputBlockMiddleware> inputBlocks = new List<InputBlockMiddleware>();


        //Original Game Input Blocking:
        public void AddInputBlockingMiddleware(InputBlockMiddleware middleware) {
            inputBlocks.Add(middleware);
        }

        public void RemoveInputBlockingMiddleware(InputBlockMiddleware middleware) {
            inputBlocks.Remove(middleware);
        }

        #endregion
    }
}
