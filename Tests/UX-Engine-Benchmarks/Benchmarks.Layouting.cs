using BenchmarkDotNet.Attributes;

using Forge.Config;
using Forge.UX.UI;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Prefabs;

using Moq;

using NUnit.Framework;

namespace UX_Engine_Benchmarks;

[MemoryDiagnoser]
public partial class Benchmarks {
    [GlobalCleanup()]
    public void ComplicatedLayoutCleanup() {
        SceneManager manager = DI.Resolve<SceneManager>();
        manager.GetRootElements().Clear();
    }

    [GlobalSetup(Targets = new[] { nameof(ComplicatedLayout), nameof(RenderedLayout) })]
    public void ComplicatedLayoutSetup() {
        Setup();

        string layout = @"
<S4Window id=""forge_ux_demo"" size=""500,500"" x=""50"" y=""50"" draggable=""true"">
	<Stack>
		<Header>Forge UX Demo</Header>
		<Group height=""200"" width=""100%"">
			<RadioButtonGroup Id=""radio_group"" LinkId=""TestGroup"">
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

        SceneManager manager = DI.Resolve<SceneManager>();
        SceneBuilder builder = DI.Resolve<SceneBuilder>();

        manager.GetRootElements().Clear();

        // Run
        builder.CreateScene(layout, out GroupPrefab? output);
        manager.AddRootElement(output!.Instantiate());
    }

    [IterationSetup()]
    public void ClearMocks() {
        // Moq stores the invocations, so we need to clear them before each iteration to avoid memory leaks
        mocks.Reset();
    }

    [Benchmark]
    public void ComplicatedLayout() {
        manager.DoFrame();
    }

    [IterationSetup(Target = nameof(RenderedLayout))]
    public void SetupRenderMock() {

        mocks.renderer.Setup(x => x.RenderUIComponent(It.IsAny<IUIComponent>(), It.IsAny<UIElement>(), It.IsAny<SceneGraphState>()))
            .Callback(
                (IUIComponent c, UIElement p, SceneGraphState state) => {
                    Thread.Sleep(1);
                });
    }

    [Benchmark]
    [Arguments(false)]
    [Arguments(true)]
    public void RenderedLayout(bool alwaysDirty) {
        const int iterations = 3;
        for (int i = 0; i < iterations; i++) {
            if (alwaysDirty || i == 0) {
                foreach (UIElement element in manager.GetAllElements()) {
                    element.IsDirty = true;
                }
            }
            manager.DoFrame();
        }
    }
}
