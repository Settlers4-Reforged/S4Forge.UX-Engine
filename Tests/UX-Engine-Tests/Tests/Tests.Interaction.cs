using Forge.Config;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Buttons;
using Forge.UX.UI.Prefabs.Groups.Layout;

using System.Numerics;

namespace UX_Engine_Tests {
    public partial class Tests {

        [Test]
        public void Test_MouseOver() {
            IoCSetup();

        }

        [Test]
        public void Test_VisibilityPropagation() {
            IoCSetup();

            SceneBuilder builder = DI.Resolve<SceneBuilder>();

            builder.CreateScene("""
                                <group Id="group">
                                  <s4-text Id="text">Test</s4-text>
                                </group>
                                """, out GroupPrefab? scene);

            UIGroup group = (UIGroup)scene!.Instantiate();
            UIText text = (UIText)group.GetElement("text")!;

            Assert.That(group.Visible, Is.True);
            Assert.That(text.Visible, Is.True);

            group.Visible = false;
            Assert.That(group.Visible, Is.False);
            Assert.That(text.Visible, Is.False);

            group.Visible = true;
            text.Visible = false;
            Assert.That(group.Visible, Is.True);
            Assert.That(text.Visible, Is.False);
        }
    }
}
