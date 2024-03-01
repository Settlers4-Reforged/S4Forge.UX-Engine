using Forge.S4.Game;
using Forge.S4.Types;
using Forge.UX.Input;
using Forge.UX.Rendering.Text;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Buttons;
using Forge.UX.UI.Prefabs.Groups;
using Forge.UX.UI.Prefabs.Groups.Layout;
using Forge.UX.UI.Prefabs.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Debug {
    public class UIDebugWindow {
        readonly SceneManager manager;
        readonly IInputManager inputManager;
        readonly IEventApi eventManager;

        public static bool Enabled = true;

        private UIWindow? window;
        private UIText? elements, cursor;

        public UIDebugWindow(SceneManager manager, IInputManager inputManager, IEventApi eventManager) {
            this.manager = manager;
            this.inputManager = inputManager;
            this.eventManager = eventManager;

            CreateMenu();
        }

        private void CreateMenu() {
            if (!Enabled)
                return;


            S4TextBuilder baseText = new S4TextBuilder()
                .WithSize(("100%", 40))
                .WithFitText(true)
                .WithTextAlignment(TextStyleAlignment.Center);
            Stack layoutPrefab = new StackBuilder().WithId("layout").WithSize(("100%", "100%")).WithMinimumDistance(80)
                .WithChildPrefabs(new List<IPrefab>() {
                    new S4ButtonBuilder()
                        .WithId("close")
                        .WithText("Close")
                        .Build(),
                    new S4ButtonBuilder()
                        .WithText("Disabled")
                        .WithIsEnabled(false)
                        .Build(),
                    new S4ButtonBuilder()
                        .WithId("test_lobby")
                        .WithText("Open Lobby\nWoah")
                        .Build(),

                    new S4TextBuilder(baseText).WithId("elements").WithText("Elements: 0").Build(),
                    new S4TextBuilder(baseText).WithId("cursor").WithText("Cursor X: 0, Y: 0").Build(),
                }).Build();

            window = new S4WindowBuilder()
                .WithPosition((500, 0))
                .WithSize((322, 500))
                .WithChildPrefabs(
                    new List<IPrefab>() {
                        layoutPrefab
                    }
                )
                .Build().Instantiate<UIWindow>();
            window.OnInput += _ => {
                UpdateMenu();
            };

            UIStack layout = window.Elements.GetById<UIStack>("layout")!;
            layout.Elements.GetById<UIButton>("close")!.OnInteract += (_) => {
                window.Close();
            };

            layout.Elements.GetById<UIButton>("test_lobby")!.OnInteract += (_) => {
                eventManager.SendEvent((EventType)64, 0, 0, 0);
            };

            elements = layout.Elements.GetById<UIText>("elements")!;
            cursor = layout.Elements.GetById<UIText>("cursor")!;

            manager.AddRootElement(window);



            UIButton windowButton = new S4ButtonBuilder()
                .WithText("Forge Debug")
                .WithSize((322f * 0.75f, 60f * 0.75f))
                .WithPosition(("90%", 0.0f))
                .Build().Instantiate<UIButton>();

            windowButton.OnInteract += (_) => {
                window.Open();
            };
            windowButton.OnInput = (button) => {
                button.Visible = !window.Visible;
            };

            manager.AddRootElement(windowButton);
        }

        private void UpdateMenu() {
            elements!.Text = "Elements: " + manager.GetAllElements().Count();
            cursor!.Text = $"Cursor X: {inputManager.MousePosition.X}, Y: {inputManager.MousePosition.Y}";
        }
    }
}
