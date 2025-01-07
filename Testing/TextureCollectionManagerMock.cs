using Forge.UX.Rendering.Texture;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Testing {
    public class TextureCollectionManagerMock : ITextureCollectionManager {
        public ITextureCollection<TMap> Register<TMap>(string path) where TMap : Enum {
            throw new NotImplementedException();
        }

        public void RegisterDefaults() {
            throw new NotImplementedException();
        }

        public class TextureMock : ITexture {
            public int Width { get; }
            public int Height { get; }
        }

        public class TextureCollectionMock<TMap> : ITextureCollection<TMap> where TMap : Enum {
            public string Path { get; } = "";

            public ITexture GetTexture(string id) {
                return new TextureMock();
            }
        }
    }
}
