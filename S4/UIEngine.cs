using Forge.S4;
using Forge.S4.Types;
using Forge.UX.S4.Types;

using System;
using System.Collections.Generic;

namespace Forge.UX.S4 {
    internal static class UIEngine {
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

        /// <summary>
        /// Creates a copy to a UI Element from a container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="valueLink"></param>
        /// <returns></returns>
        public static S4UIEngineElement? GetUIElementFromIndex(int container, int valueLink) {
            unsafe {
                var element = GetUIElementFromIndexUnsafe(container, valueLink);
                return element == null ? null : *element;
            }
        }

        public static unsafe S4UIEngineElement* GetUIElementFromIndexUnsafe(int container, int valueLink) {
            Int32 UIMenus = GameValues.ReadValue<Int32>(0x1064C94);


            Int32 containerOffset = GameValues.ReadValue<Int32>(UIMenus + (container + 4) * 4, false);
            Int32 elementsOffset = GameValues.ReadValue<Int32>(containerOffset + UIMenus, false);
            Int16 elementCount = GameValues.ReadValue<Int16>(containerOffset + UIMenus + 12, false);
            S4UIEngineElement* elementArrayPointer =
                (S4UIEngineElement*)new IntPtr(containerOffset + UIMenus + 16).ToPointer();

            int i = 0;
            while (valueLink != elementArrayPointer->valueLink) {
                i++;
                elementArrayPointer++;

                if (i >= elementCount)
                    return null;
            }

            return elementArrayPointer;
        }

        public static unsafe S4UIEngineElement*[] GetAllUIElementsFromIndexUnsafe(int container) {
            Int32 UIMenus = GameValues.ReadValue<Int32>(0x1064C94);


            Int32 containerOffset = GameValues.ReadValue<Int32>(UIMenus + (container + 4) * 4, false);
            Int32 elementsOffset = GameValues.ReadValue<Int32>(containerOffset + UIMenus, false);
            Int16 elementCount = GameValues.ReadValue<Int16>(containerOffset + UIMenus + 12, false);
            S4UIEngineElement* elementArrayPointer =
                (S4UIEngineElement*)new IntPtr(containerOffset + UIMenus + 16).ToPointer();

            S4UIEngineElement*[] elements = new S4UIEngineElement*[elementCount];

            int i = 0;
            while (i < elementCount) {
                elements[i] = elementArrayPointer;

                i++;
                elementArrayPointer++;
            }

            return elements;
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
            public Event EventId;

            public int Timestamp;

            //Maybe:
            public int Caller;
        }
    }
}
