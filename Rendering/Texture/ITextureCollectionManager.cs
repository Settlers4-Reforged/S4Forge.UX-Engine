using System.Collections.Generic;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollectionManager {
        ITextureCollection GetCollection(string id);
        ITexture Get(string col, string id);


        ITextureCollection GetCollection(TextureCollectionMap id) {
            return GetCollection(id.ToString());
        }

        ITexture Get(TextureCollectionMap col, string id) {
            return Get(col.ToString(), id);
        }

        ITexture Get(TextureCollectionMap col, ForgeTextureMap id) {
            return Get(col.ToString(), id.ToString());
        }
    }
}
