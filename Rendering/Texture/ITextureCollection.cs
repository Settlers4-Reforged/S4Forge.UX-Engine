using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollection<in TMap> where TMap : System.Enum {
        string Path { get; }

        ITexture[] GetTextures();

        ITexture GetTexture(string id);

        ITexture GetTexture(TMap id) {
            return GetTexture(id.ToString());
        }
    }
}
