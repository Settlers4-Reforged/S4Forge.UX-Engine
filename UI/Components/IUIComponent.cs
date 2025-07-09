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

        public (PositioningAlignment x, PositioningAlignment y) Alignment { get; set; }

        public Effects Effects { get; set; }

        /// <summary>
        /// The rendering data associated with the element
        /// </summary>
        /// <remarks>
        /// This object is only for rendering purposes and should not be used by application code
        /// </remarks>
        public IElementData? Data { get; set; }
    }
}
