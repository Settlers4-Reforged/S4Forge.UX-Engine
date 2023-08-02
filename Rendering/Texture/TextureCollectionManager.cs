using System.Collections.Generic;

namespace Forge.UX.Rendering.Texture {
    public static class TextureCollectionManager {
        private static readonly Dictionary<int, ITextureCollection> collections = new Dictionary<int, ITextureCollection>();

        public static void AddCollection(int id, ITextureCollection collection) => collections.Add(id, collection);

        public static void RemoveCollection(int id) => collections.Remove(id);

        public static ITextureCollection GetCollection(int id) => collections[id];

        public static ITexture Get(int col, int id) => GetCollection(col).GetTexture(id);
    }
}
