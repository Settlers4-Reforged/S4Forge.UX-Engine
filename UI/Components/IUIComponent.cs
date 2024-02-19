using Forge.UX.Rendering;

using System.Numerics;

namespace Forge.UX.UI.Components {
    public interface IUIComponent {
        public Vector2 Offset { get; }

        public Effects Effects { get; set; }
    }
}
