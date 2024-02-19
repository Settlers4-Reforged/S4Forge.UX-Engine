using Forge;
using Forge.Config;
using Forge.S4.Callbacks;
using Forge.UX;
using Forge.UX.Debug;
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

using System.Numerics;
using System.Reflection;

using UX_Engine_Tests.Mocks;

namespace UX_Engine_Tests {
    public partial class Tests {
        [OneTimeSetUp]
        public void Setup() {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                string assemblyName = new AssemblyName(args.Name).Name;
                return assemblyName switch {
                    "S4Forge" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4Forge.dll"),
                    "S4APIWrapper" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4APIWrapper.asi"),
                    _ => null
                };
            };

            ImplementDI();
        }

        private static void ImplementDI() {
            UXTestDependencies.AddTestDependencies();
            UXEngine.Implement(typeof(RenderingManagerMock), typeof(TextureCollectionManagerMock), 0);
        }
    }
}