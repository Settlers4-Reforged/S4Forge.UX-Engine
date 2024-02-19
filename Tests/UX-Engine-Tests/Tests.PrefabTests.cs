using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Buttons;
using Forge.UX.UI.Prefabs.Groups.Layout;

namespace UX_Engine_Tests {
    public partial class Tests {

        [Test]
        public void Test_BuilderPattern() {
            S4Button button = new S4ButtonBuilder().WithText("Test").WithWidth((100, PositioningMode.Absolute)).Build();

            Assert.Multiple(() => {
                // Overrides:
                Assert.That(button.Text.Value, Is.EqualTo("Test"));
                Assert.That(button.Width.Value, Is.EqualTo(100));
                Assert.That(button.Width.PositionMode, Is.EqualTo(PositioningMode.Absolute));

                // Defaults:
                Assert.That(button.IsEnabled.Default, Is.True);
                Assert.That(button.Visible.Default, Is.True);
            });

            UIButton instance = (UIButton)button.Instantiate();
            Assert.Multiple(() => {
                Assert.That(instance.Text, Is.EqualTo("Test"));
                Assert.That(instance.Size.X, Is.EqualTo(100));
                Assert.That(instance.SizeMode.width, Is.EqualTo(PositioningMode.Absolute));

                Assert.That(instance.Enabled, Is.True);
                Assert.That(instance.Visible, Is.True);
            });
        }

        [Test]
        public void Test_BuilderPattern_Relayout() {
            UIStack layout = new StackBuilder().WithWidth("100%").WithHeight("100%").WithMinimumDistance(75).WithChildPrefabs(new List<IPrefab>() {
                new S4ButtonBuilder().WithText("Test").WithWidth("100%").Build(),
            }).Build().Instantiate<UIStack>();

            Assert.Multiple(() => {
                Assert.That(layout.Elements.Count, Is.EqualTo(1));
                if (layout.Elements[0] is not UIButton button) {
                    Assert.Fail();
                    return;
                };

                Assert.That(button.Text, Is.EqualTo("Test"));
                Assert.That(button.Size.Y, Is.EqualTo(75));
            });
        }
    }
}
