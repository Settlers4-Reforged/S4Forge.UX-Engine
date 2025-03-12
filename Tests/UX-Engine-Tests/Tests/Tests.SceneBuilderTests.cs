using Forge.Config;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Elements.Grouping.Interaction;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UX_Engine_Tests {
    public partial class Tests {
        [Test]
        public void SceneBuilder_WithCorrectScene_CorrectBuild() {
            // Prepare
            IoCSetup();

            SceneManager manager = DI.Resolve<SceneManager>();
            SceneBuilder builder = DI.Resolve<SceneBuilder>();

            manager.GetRootElements().Clear();

            // Run
            bool result = builder.CreateScene(CorrectScene, out GroupPrefab? output);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(output, Is.Not.Null);

            manager.AddRootElement(output!.Instantiate());

            Assert.That(manager.GetRootElements().Count, Is.EqualTo(1));
            Assert.That(manager.GetRootElements().First(), Is.AssignableTo(typeof(UIWindow)));

            Assert.Multiple(() => {
                // Test that the output element tree is equal to the CorrectScene description
                UIWindow window = (UIWindow)manager.GetRootElements().First();
                Assert.That(window.Size, Is.EqualTo(new Vector2(500, 500)));
                Assert.That(window.Position, Is.EqualTo(new Vector2(50, 50)));
                Assert.That(window.Id, Is.EqualTo("forge_ux_demo"));
                Assert.That(window.Draggable, Is.True);

                Assert.That(window.Elements.Count, Is.EqualTo(1));
                Assert.That(window.Elements[0], Is.AssignableTo(typeof(UIStack)));

                UIStack stack = (UIStack)window.Elements[0];
                Assert.That(stack.Elements.Count, Is.EqualTo(3));
                Assert.That(stack.Elements[0], Is.AssignableTo(typeof(UIText)));
                UIText header = (UIText)stack.Elements[0];
                Assert.That(header.TextComponent.Text, Is.EqualTo("Forge UX Demo"));

                Assert.That(stack.Elements[1], Is.AssignableTo(typeof(UIGroup)));
                UIGroup group = (UIGroup)stack.Elements[1];
                Assert.That(group.Elements.Count, Is.EqualTo(1));
                Assert.That(group.Elements[0], Is.AssignableTo(typeof(UIRadioButtonGroup<string>)));
                {
                    UIRadioButtonGroup<string> radioGroup = (UIRadioButtonGroup<string>)group.Elements[0];
                    Assert.That(radioGroup.Elements.Count, Is.EqualTo(3));
                    Assert.That(radioGroup.Id, Is.EqualTo("radio_group"));

                    Assert.That(window.Elements.GetById("radio_group"), Is.EqualTo(radioGroup));

                    Assert.That(radioGroup.Elements[0], Is.AssignableTo(typeof(UIText)));

                    Assert.That(radioGroup.Elements[1], Is.AssignableTo(typeof(UIStack)));
                    UIStack radioStack1 = (UIStack)radioGroup.Elements[1];
                    Assert.That(radioStack1.SizeMode, Is.EqualTo((PositioningMode.Relative, PositioningMode.Normal)));
                    Assert.That(radioStack1.Elements.Count, Is.EqualTo(2));
                    Assert.That(radioStack1.Elements[0], Is.AssignableTo(typeof(UIRadioButton<string>)));
                    Assert.That(radioStack1.Elements[1], Is.AssignableTo(typeof(UIRadioButton<string>)));

                    UIRadioButton<string> radio1 = (UIRadioButton<string>)radioStack1.Elements[0];
                    Assert.That(radio1.Text, Is.EqualTo("Test 1"));
                    Assert.That(radio1.LinkId, Is.EqualTo("TestGroup"));
                }

                Assert.That(stack.Elements[2], Is.AssignableTo(typeof(UIButton)));
                UIButton button = (UIButton)stack.Elements[2];
                Assert.That(button.Text, Is.EqualTo("Accept"));

            });
        }

        private const string CorrectScene = @"
<s4-window id=""forge_ux_demo"" size=""500,500"" x=""50"" y=""50"" draggable=""true"">
	<stack>
		<header>Forge UX Demo</header>
		<group height=""200"" width=""100%"">
			<radio-button-group Id=""radio_group"" LinkId=""TestGroup"">
				<header variant=""small"">Radio Buttons</header>
				<stack width=""50%"">
					<s4-radio-button LinkId=""TestGroup"">Test 1</s4-radio-button>
					<s4-radio-button LinkId=""TestGroup"">Test 2</s4-radio-button>
				</stack>
				<stack x=""50%"" width=""50%"">
					<s4-radio-button LinkId=""TestGroup"">Test 3</s4-radio-button>
					<s4-radio-button LinkId=""TestGroup"">Test 4</s4-radio-button>
				</stack>
			</radio-button-group>
		</group>
		<s4-button id=""accept_button"">Accept</s4-button>
	</stack>
</s4-window>
";
    }
}
