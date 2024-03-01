using Forge.S4;
using Forge.S4.Types;
using Forge.UX.S4.Types;

using System;
using System.Collections.Generic;

namespace Forge.UX.S4 {
    internal class S4UI {
        public delegate void ScreenCallback(UIScreen previous, UIScreen next);

        public delegate void MenuCallback(List<UIMenu> previous, List<UIMenu> next);

        public delegate void EventCallback(EventCallbackParameters param);

        //public Action<UIScreen/*previous*/, UIScreen/*new*/> OnS4ScreenChange { get; set; }
        //public Action<List<UIMenu>/*previous*/, List<UIMenu>/*new*/> OnS4MenuChange { get; set; }

        public static UIScreen[] GetScreens() {
            throw new System.NotImplementedException();
        }

        public static UIMenu[] GetMenus() {
            throw new System.NotImplementedException();
        }

        public static bool RegisterS4ScreenChangeCallback(ScreenCallback callback) {
            throw new System.NotImplementedException();
        }

        public static bool RegisterS4MenuChangeCallback(MenuCallback callback) {
            throw new System.NotImplementedException();
        }

        public static bool RegisterEventCallback(EventType eventType, EventCallback callback) {
            throw new System.NotImplementedException();
        }



        public static GUIEventHandler? GUIEventHandler {
            get {
                unsafe {
                    var handler = (GUIEventHandler*)new IntPtr(GameValues.ReadValue<Int32>(0x10540CC)).ToPointer();

                    if (handler == null) {
                        return null;
                    }

                    return *handler;
                }
            }
        }

        public struct EventCallbackParameters {
            public EventType EventType;

            public int Timestamp;

            //Maybe:
            public int Caller;
        }
    }
}
