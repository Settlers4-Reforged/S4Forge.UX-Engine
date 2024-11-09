using DryIoc;

using Forge.Config;
using Forge.S4.Game;
using Forge.UX;
using Forge.UX.Rendering;
using Forge.UX.S4;
using Forge.UX.Testing;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UX_Engine_Tests {
    public class IoCMock {
        public Mock<IEventApi> eventApi = new Mock<IEventApi>();
        public Mock<IPlayerApi> playerApi = new Mock<IPlayerApi>();
        public Mock<ISoundApi> soundApi = new Mock<ISoundApi>();

        public Mock<IUIManager> uiManager = new Mock<IUIManager>();
        public Mock<IRenderer> renderer = new Mock<IRenderer>();

        public static IoCMock IoCSetup() {
            IoCMock mocks = new IoCMock();
            DI.Reset();
            mocks.ImplementDI();

            return mocks;
        }

        private void MockNativeApi() {
            DI.Dependencies.RegisterInstance<IEventApi>(eventApi.Object);
            DI.Dependencies.RegisterInstance<IPlayerApi>(playerApi.Object);
            DI.Dependencies.RegisterInstance<ISoundApi>(soundApi.Object);

            DI.Dependencies.RegisterInstance<IUIManager>(uiManager.Object);
        }

        private void ImplementDI() {
            MockNativeApi();

            UXTestDependencies.AddTestDependencies();
            UXEngine.IsInitialized = true;

            DI.Dependencies.RegisterMany(new[] { typeof(TextureCollectionManagerMock) }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
            DI.Dependencies.RegisterInstance<IRenderer>(renderer.Object, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        }

        public void Reset() {
            eventApi.Reset();
            playerApi.Reset();
            soundApi.Reset();
            uiManager.Reset();
            renderer.Reset();
        }
    }
}
