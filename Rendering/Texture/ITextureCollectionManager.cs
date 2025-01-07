using System;
using System.Collections.Generic;

namespace Forge.UX.Rendering.Texture {
    public interface ITextureCollectionManager {
        ITextureCollection<TMap> Register<TMap>(string path) where TMap : Enum;

        void RegisterDefaults();
    }
}
