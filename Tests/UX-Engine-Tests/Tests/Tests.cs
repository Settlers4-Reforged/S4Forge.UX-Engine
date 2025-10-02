using DryIoc;

using Forge;
using Forge.Config;
using Forge.S4.Game;
using Forge.UX;
using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.Testing;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Elements.Grouping.Interaction;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Text;

using Moq;

using System.Numerics;
using System.Reflection;

namespace UX_Engine_Tests {

    [NonParallelizable]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public partial class Tests {
        private IoCMock mocks = null!;

        public void IoCSetup() {
            mocks = IoCMock.IoCSetup();
        }
    }
}