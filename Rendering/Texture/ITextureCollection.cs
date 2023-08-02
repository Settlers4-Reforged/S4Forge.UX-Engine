using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollection
    {
        ITexture GetTexture(int id);
    }
}
