using System.Collections.Generic;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollectionManager {
        ITextureCollection GetCollection(int id);
        ITexture Get(int col, int id);


        ITextureCollection GetCollection(TextureCollectionMap id) {
            return GetCollection((int)id);
        }

        ITexture Get(TextureCollectionMap col, int id) {
            return Get((int)col, id);
        }
    }
}
