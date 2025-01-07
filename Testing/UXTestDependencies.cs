using DryIoc;

using Forge.Config;
using Forge.UX.Input;
using Forge.UX.Rendering.Texture;
using Forge.UX.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Testing {
    public static class UXTestDependencies {
        public static void AddTestDependencies() {
            DI.Dependencies.RegisterInstance<IInputManager>(new TestInputManager());
            DI.Dependencies.RegisterInstance(new UIEngine());

            DI.Dependencies.RegisterMany<TextureCollectionManagerMock>();
            DI.Dependencies.RegisterMany<TextureCollectionManagerMock.TextureCollectionMock<ForgeTextureMap>>();

            DI.Resolve<SceneManager>().Init();
        }
    }
}
