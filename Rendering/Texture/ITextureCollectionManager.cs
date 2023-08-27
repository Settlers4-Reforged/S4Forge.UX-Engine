using System.Collections.Generic;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollectionManager {
        void AddCollection(int id, ITextureCollection collection);
        void RemoveCollection(int id);
        ITextureCollection GetCollection(int id);
        ITexture Get(int col, int id);
    }
}
