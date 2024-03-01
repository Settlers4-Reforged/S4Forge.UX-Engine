using Forge.UX.Rendering;
using Forge.UX.UI.Elements;

using System.Numerics;

namespace Forge.UX.UI.Components {
    public interface IUIComponent {
        public Vector2 Position { get; set; }
        /// <summary>
        /// Whether the element is positioned in screen space coordinates, or relative to its group parent or relative to the screen size
        /// </summary>
        public (PositioningMode x, PositioningMode y) PositionMode { get; set; }

        public Vector2 Size { get; set; }
        /// <summary>
        /// Whether the element is sized in screen space coordinates, or relative to its group parent or relative to the screen size
        /// </summary>
        public (PositioningMode width, PositioningMode height) SizeMode { get; set; }

        public Effects Effects { get; set; }
    }
}
