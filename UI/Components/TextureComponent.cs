using Forge.Config;
using Forge.S4.Types;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Texture;
using Forge.UX.UI.Elements;

using System;
using System.Numerics;

namespace Forge.UX.UI.Components {
    public class TextureComponent : IUIComponent {
        public virtual ITexture? Texture { get; set; }

        public Vector2 Size { get; set; } = Vector2.One;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public (PositioningMode x, PositioningMode y) PositionMode { get; set; } = (PositioningMode.Normal, PositioningMode.Normal);
        public (PositioningMode width, PositioningMode height) SizeMode { get; set; } = (PositioningMode.Relative, PositioningMode.Relative);
        public (PositioningAlignment x, PositioningAlignment y) Alignment { get; set; } = (PositioningAlignment.Start, PositioningAlignment.Start);

        public Effects Effects { get; set; }
        public IElementData? Data { get; set; }

        //Whether this texture gets scaled to the UIElement Size or not
        public bool Scaled { get; set; } = false;

        public bool IsTeamColored { get; set; } = false;
        public Team? Team { get; set; } = null;

        protected TextureComponent() { }

        public TextureComponent(ITexture texture) {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }

        public static TextureComponent FromCollection<TMap>(string id) where TMap : Enum {
            ITextureCollection<TMap> tc = DI.Resolve<ITextureCollection<TMap>>();
            return new TextureComponent(tc.GetTexture(id));
        }
    }
}
