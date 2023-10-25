using Forge.UX;
using Forge.UX.UI;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Elements.Grouping.Interaction;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;

using System.Numerics;
using System.Reflection;

using UX_Engine_Tests.Mocks;

namespace UX_Engine_Tests {
    public class Tests {
        [SetUp]
        public void Setup() {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                string assemblyName = new AssemblyName(args.Name).Name;
                return assemblyName switch {
                    "S4Forge" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4Forge.dll"),
                    "S4APIWrapper" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4APIWrapper.asi"),
                    _ => null
                };
            };

            UXEngine.IsInitialized = true;
            UXEngine.Implement(new RenderingManagerMock(), new TextureCollectionManagerMock(), 0);
        }

        [Test]
        public void SceneBuilder_WithCorrectScene_CorrectBuild() {
            // Prepare
            SceneManager manager = new SceneManager();
            SceneBuilder builder = new SceneBuilder(manager);

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
                //Assert.That(window.Draggable, Is.True);

                Assert.That(window.Elements.Count, Is.EqualTo(1));
                Assert.That(window.Elements[0], Is.AssignableTo(typeof(UIStack)));

                UIStack stack = (UIStack)window.Elements[0];
                Assert.That(stack.Elements.Count, Is.EqualTo(3));
                Assert.That(stack.Elements[0], Is.AssignableTo(typeof(UIText)));
                UIText header = (UIText)stack.Elements[0];
                Assert.That(header.Text.Text, Is.EqualTo("Forge UX Demo"));

                Assert.That(stack.Elements[1], Is.AssignableTo(typeof(UIGroup)));
                UIGroup group = (UIGroup)stack.Elements[1];
                Assert.That(group.Elements.Count, Is.EqualTo(1));
                Assert.That(group.Elements[0], Is.AssignableTo(typeof(UIRadioButtonGroup<string>)));

                Assert.That(stack.Elements[2], Is.AssignableTo(typeof(UIButton)));
                UIButton button = (UIButton)stack.Elements[2];
                Assert.That(button.Text, Is.EqualTo("Accept"));

            });
        }

        private const string CorrectScene = @"
<S4Window id=""forge_ux_demo"" width=""500"" height=""500"" x=""50"" y=""50"" draggable=""true"">
	<Stack>
		<Header>Forge UX Demo</Header>
		<Group height=""200"" width=""100%"">
			<RadioButtonGroup LinkId=""TestGroup"">
				<Header variant=""small"">Radio Buttons</Header>
				<Stack width=""50%"">
					<S4RadioButton LinkId=""TestGroup"">Test 1</S4RadioButton>
					<S4RadioButton LinkId=""TestGroup"">Test 2</S4RadioButton>
				</Stack>
				<Stack x=""50%"" width=""50%"">
					<S4RadioButton LinkId=""TestGroup"">Test 3</S4RadioButton>
					<S4RadioButton LinkId=""TestGroup"">Test 4</S4RadioButton>
				</Stack>
			</RadioButtonGroup>
		</Group>
		<S4Button id=""accept_button"">Accept</S4Button>
	</Stack>
</S4Window>
";
    }
}