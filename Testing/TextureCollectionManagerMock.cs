using Forge.UX.Rendering.Texture;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Testing {
    public class TextureCollectionManagerMock : ITextureCollectionManager {
        public ITextureCollection GetCollection(int id) {
            return new TextureCollectionMock();
        }

        public ITexture Get(int col, int id) {
            return new TextureMock();
        }

        public class TextureMock : ITexture {
            public int Width { get; }
            public int Height { get; }
        }

        public class TextureCollectionMock : ITextureCollection {
            public ITexture GetTexture(int id) {
                return new TextureMock();
            }
        }
    }
}
