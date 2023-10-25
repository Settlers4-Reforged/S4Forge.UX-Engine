using Forge.UX.Rendering.Texture;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UX_Engine_Tests.Mocks {
    internal class TextureCollectionManagerMock : ITextureCollectionManager {
        public ITextureCollection GetCollection(int id) {
            throw new NotImplementedException();
        }

        public ITexture Get(int col, int id) {
            return new TextureMock();
        }

        internal class TextureMock : ITexture {
            public int Width { get; }
            public int Height { get; }
        }
    }
}
