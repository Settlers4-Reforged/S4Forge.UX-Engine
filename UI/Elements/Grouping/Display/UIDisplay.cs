﻿namespace Forge.UX.UI.Elements.Grouping.Display {
    public abstract class UIDisplay<T> : UIGroup where T : UIDisplay<T>, new() {
        public static T Create() {
            T display = new T();

            return display;
        }


    }
}
