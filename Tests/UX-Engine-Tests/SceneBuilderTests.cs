using Forge.UX;
using Forge.UX.UI;
using Forge.UX.UI.Elements.Grouping.Display;

using System.Reflection;

namespace UX_Engine_Tests {
    public class Tests {
        [SetUp]
        public void Setup() {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                string assemblyName = new AssemblyName(args.Name).Name;
                return assemblyName switch {
                    "S4Forge" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4Forge.nasi"),
                    "S4APIWrapper" => Assembly.LoadFrom(Environment.CurrentDirectory + "\\S4APIWrapper.asi"),
                    _ => null
                };
            };
        }

        [Test]
        public void SceneBuilder_WithCorrectScene_CorrectBuild() {
            // Prepare
            SceneManager manager = new SceneManager();
            SceneBuilder builder = new SceneBuilder(manager);

            // Run
            bool result = builder.CreateScene(CorrectScene);

            // Assert
            Assert.True(result);
            Assert.That(manager.GetRootElements().Count(), Is.EqualTo(1));
            Assert.That(manager.GetRootElements().First(), Is.AssignableTo(typeof(UIWindow)));
        }

        private const string CorrectScene = @"
<S4Window id=""forge_ux_demo"" width=""500"" height=""500"" x=""50"" y=""50"" draggable=""true"">
	<Stack>
		<Header>Forge UX Demo</Header>
		<Group height=""200"" width=""100%"">
			<RadioButtonGroup>
				<Header variant=""small"">Radio Buttons</Header>
				<Stack width=""50%"">
					<RadioButton>Test 1</RadioButton>
					<RadioButton>Test 2</RadioButton>
				</Stack>
				<Stack x=""50%"" width=""50%"">
					<RadioButton>Test 3</RadioButton>
					<RadioButton>Test 4</RadioButton>
				</Stack>
			</RadioButtonGroup>
		</Group>
		<Button id=""accept_button"">Accept</Button>
	</Stack>
</S4Window>
";
    }
}