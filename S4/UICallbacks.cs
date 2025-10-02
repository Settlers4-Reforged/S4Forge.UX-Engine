using Forge.Game.Events;
using Forge.Game.UI;

using static Forge.UX.S4.IUICallbacks;

namespace Forge.UX.S4 {
    #region Delegates

    public delegate void ScreenCallback(S4UIScreen previous, S4UIScreen next);
    public delegate void MenuCallback(S4UIMenu previous, S4UIMenu next);
    public delegate void SubmenuCallback(S4UISubmenu previous, S4UISubmenu next);
    public delegate void EventCallback(EventCallbackParameters param);

    #endregion

    public interface IUICallbacks {
        public struct EventCallbackParameters {
            public EventType EventType;

            public int Timestamp;

            //Maybe:
            public int Caller;
        }

        event ScreenCallback? OnScreenChange;
        event MenuCallback? OnMenuChange;
        event SubmenuCallback? OnSubmenuChange;
        event EventCallback? OnEventChange;
    }

    internal class UICallbacks : IUICallbacks {

        public UICallbacks() {

        }

        public event ScreenCallback? OnScreenChange;
        public event MenuCallback? OnMenuChange;
        public event SubmenuCallback? OnSubmenuChange;
        public event EventCallback? OnEventChange;
    }
}
