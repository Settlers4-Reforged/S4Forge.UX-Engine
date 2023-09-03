using System.Collections.Generic;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollectionManager {
        ITextureCollection GetCollection(int id);
        ITexture Get(int col, int id);
    }
}
