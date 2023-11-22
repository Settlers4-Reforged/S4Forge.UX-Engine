using DryIoc;

using Forge.Config;
using Forge.S4.Types;
using Forge.UX.Rendering.Texture;

using System;
using System.Numerics;

namespace Forge.UX.UI.Components {
    public class TextureComponent : IUIComponent {
        public virtual ITexture Texture { get; set; }
        public Vector2 Offset { get; set; } = Vector2.Zero;

        //Whether this texture gets scaled to the UIElement Size or not
        public bool Scaled { get; set; } = false;

        public bool IsTeamColored { get; set; } = false;
        public Team? Team { get; set; } = null;

        protected TextureComponent() { }

        public TextureComponent(ITexture texture) {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }

        public static TextureComponent FromCollection(int collection, int id) {
            ITextureCollectionManager tcm = DI.Dependencies.Resolve<ITextureCollectionManager>();
            return new TextureComponent(tcm.GetCollection(collection).GetTexture(id));
        }
    }
}
