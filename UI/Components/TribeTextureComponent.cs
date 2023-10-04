using Forge.S4.Managers;
using Forge.S4.Types;
using Forge.UX.Rendering.Texture;

using System.Collections.Generic;

namespace Forge.UX.UI.Components {
    /// <summary>
    /// Changes based on current tribe - defaults to DEFAULT if a tribe is not specified
    /// </summary>
    public sealed class TribeTextureComponent : TextureComponent {
        public override ITexture Texture {
            get {
                Tribe c = GameConfig.GetCurrentTribe();
                if (!textures.TryGetValue(c, out ITexture? o)) {
                    o = textures[Tribe.Default];
                }

                return o;
            }
        }

        private readonly IDictionary<Tribe, ITexture> textures;

        public TribeTextureComponent(IDictionary<Tribe, ITexture> textures) : base() {
            this.textures = textures;
        }
    }
}
