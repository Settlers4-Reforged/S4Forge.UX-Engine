using Forge.S4.Types;

using System.Collections.Generic;

namespace Forge.UX.S4 {
    internal static class GameEventManager {
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

        public static bool RegisterEventCallback(Event eventId, EventCallback callback) {
            throw new System.NotImplementedException();
        }
    }

    public struct EventCallbackParameters {
        public Event EventId;
        public int Timestamp;
        //Maybe:
        public int Caller;
    }
}
