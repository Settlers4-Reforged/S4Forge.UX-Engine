using Forge.Config;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Buttons;
using Forge.UX.UI.Prefabs.Groups;
using Forge.UX.UI.Prefabs.Groups.Layout;
using Forge.UX.UI.Prefabs.Text;

using System.Numerics;

namespace UX_Engine_Tests {
    public partial class Tests {

        [Test]
        public void Test_SceneGraphState() {
            // Prepare
            IoCSetup();
            mocks.renderer.Setup(x => x.GetScreenSize()).Returns(new Vector2(1000, 1000));

            SceneManager manager = DI.Resolve<SceneManager>();
            SceneBuilder builder = DI.Resolve<SceneBuilder>();

            manager.GetRootElements().Clear();

            // Run
            UIGroup stack = new GroupBuilder()
                .WithSize((100, 100))
                .WithPosition(("50%", "50%"))
                .WithChildPrefabs(new List<IPrefab>() {
                    new HeaderBuilder()
                        .WithId("header")
                        .WithSize(("100%", 10))
                        .WithPosition(("50%",0))
                        .Build(),
                })
                .Build().Instantiate<UIGroup>();


            manager.AddRootElement(stack);

            Assert.That(manager.GetRootElements().Count, Is.EqualTo(1));

            SceneGraphState stackState = stack.GraphState;
            Assert.That(stackState.ContainerGroup, Is.Not.Null);
            Assert.That(stackState.CurrentContainerSize, Is.EqualTo(new Vector2(100, 100)));
            Assert.That(stackState.CurrentPosition, Is.EqualTo(new Vector2(500, 500)));


            UIElement? header = stack.GetElement("header");
            Assert.That(header, Is.Not.Null);
            SceneGraphState headerState = header.GraphState;
            Assert.That(headerState.ContainerGroup, Is.EqualTo(stack));
        }

        [Test]
        public void Test_SceneGraphState_Alignment() {
            // Prepare
            IoCSetup();
            mocks.renderer.Setup(x => x.GetScreenSize()).Returns(new Vector2(1000, 1000));

            SceneManager manager = DI.Resolve<SceneManager>();
            SceneBuilder builder = DI.Resolve<SceneBuilder>();

            manager.GetRootElements().Clear();

            // Run
            UIGroup stack = new GroupBuilder()
                .WithSize((100, 100))
                .WithPosition(("50%", "50%"))
                .WithHorizontalAlignment(PositioningAlignment.Center)
                .WithVerticalAlignment(PositioningAlignment.End)
                .Build().Instantiate<UIGroup>();

            manager.AddRootElement(stack);

            Assert.That(manager.GetRootElements().Count, Is.EqualTo(1));

            SceneGraphState stackState = stack.GraphState;
            Assert.That(stackState.CurrentPosition, Is.EqualTo(new Vector2(500 - 50, 500 - 100)));
        }
    }
}
