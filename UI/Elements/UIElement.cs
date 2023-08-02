using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.UI.Components;

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Forge.UX.UI.Elements {
    public abstract class UIElement {
        public virtual string Id { get; set; } = string.Empty;
        public virtual Vector2 Size { get; set; }

        public Vector2 Position;
        public virtual bool PositionAbsolute { get; set; } = false;

        ///<summary>The lower the further behind: 0 &lt; 100 &lt; 1000</summary>
        public int ZIndex = 0;

        public bool IsMouseHover;

        ///<summary>Prevents mouse hover or click inputs from being captured from this element<br/>
        /// Will ignore all child elements when true for group</summary>
        public bool IgnoresMouse = false;

        internal virtual void OnMouseClickDown(int mb) { }
        internal virtual void OnMouseClickUp(int mb) { }

        internal virtual void OnMouseEnter() { }
        internal virtual void OnMouseLeave() { }

        public virtual void Input(SceneGraphState state) { }

        public Effects Effects { get; set; }

        public virtual List<IUIComponent> Components { get; protected set; } = new List<IUIComponent>();
    }
}
