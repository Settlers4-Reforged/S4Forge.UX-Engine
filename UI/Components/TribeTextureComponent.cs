
using Forge.Config;
using Forge.Game.Core;
using Forge.Game.World;
using Forge.Native;
using Forge.UX.Rendering.Texture;

using System.Collections.Generic;

namespace Forge.UX.UI.Components {
    /// <summary>
    /// Changes based on current tribe - defaults to DEFAULT if a tribe is not specified
    /// </summary>
    public sealed class TribeTextureComponent : TextureComponent {
        public override ITexture Texture {
            get {
                IPlayerApi api = DI.Resolve<IPlayerApi>();
                Tribe c = api.GetLocalPlayer().Tribe;
                if (!textures.TryGetValue(c, out ITexture? o)) {
                    o = textures[Tribe.Default];
                }

                return o;
            }
        }

        private readonly IDictionary<Tribe, ITexture> textures;

        public TribeTextureComponent(IDictionary<Tribe, ITexture> textures) : base() {
            //TODO: validate that the dictionary contains all tribes or at least Default

            this.textures = textures;
        }
    }
}
