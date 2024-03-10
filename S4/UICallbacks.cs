using Forge.S4;
using Forge.S4.Types;
using Forge.UX.S4.Types.Native;

using System;
using System.Collections.Generic;

using static Forge.UX.S4.IUICallbacks;
using static Forge.UX.S4.UICallbacks;

namespace Forge.UX.S4 {
    #region Delegates

    public delegate void ScreenCallback(S4UIScreenId previous, S4UIScreenId next);
    public delegate void MenuCallback(List<S4UIMenuId> previous, List<S4UIMenuId> next);
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
        event EventCallback? OnEventChange;
    }

    internal class UICallbacks : IUICallbacks {

        public UICallbacks() {

        }

        public event ScreenCallback? OnScreenChange;
        public event MenuCallback? OnMenuChange;
        public event EventCallback? OnEventChange;
    }
}
