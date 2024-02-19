using Forge.UX.Input;
using Forge.UX.UI;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Elements.Grouping.Layout;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Elements.Static;
using Forge.UX.UI.Prefabs;
using Forge.UX.UI.Prefabs.Buttons;
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

        public static bool Enabled = true;

        private UIWindow window;
        private UIText elements, cursor;

        public UIDebugWindow(SceneManager manager, IInputManager inputManager) {
            this.manager = manager;
            this.inputManager = inputManager;

            CreateMenu();
        }

        private void CreateMenu() {
            if (!Enabled)
                return;

            UIButton windowButton = new UIButton() {
                OnInteract = (_) => {
                    window.Open();
                },
                OnInput = (button) => {
                    button.Visible = !window.Visible;
                },
                Text = "Debug",
                Size = new Vector2(100, 50),
                Position = new Vector2(0.9f, 0.0f),
                PositionMode = (PositioningMode.AbsoluteRelative, PositioningMode.AbsoluteRelative),
            };
            manager.AddRootElement(windowButton);

            window = new UIWindow() {
                Position = new Vector2(500, 0),
                Size = new Vector2(200, 500),
            };
            window.OnInput += _ => {
                UpdateMenu();
            };


            S4TextBuilder baseText = new S4TextBuilder().WithFitText(true).WithWidth("100%");
            UIStack layout = new StackBuilder().WithWidth("100%").WithHeight("100%").WithMinimumDistance(75).WithChildPrefabs(new List<IPrefab>() {
                new S4ButtonBuilder()
                    .WithId("close")
                    .WithText("Close")
                    .WithWidth("100%")
                    .Build(),

                new S4TextBuilder(baseText).WithId("elements").WithText("Elements: 0").Build(),
                new S4TextBuilder(baseText).WithId("cursor").WithText("Cursor X: 0, Y: 0").Build(),
            }).Build().Instantiate<UIStack>();

            //UIStack layout = new UIStack(75) {
            //    PositionMode = (PositioningMode.Relative, PositioningMode.Relative),
            //    SizeMode = (PositioningMode.Relative, PositioningMode.Relative),
            //    Size = new System.Numerics.Vector2(1, 1),
            //};
            window.Elements.Add(layout);

            layout.Elements.GetById<UIButton>("close")!.OnInteract = (_) => {
                window.Close();
            };

            elements = layout.Elements.GetById<UIText>("elements")!;
            cursor = layout.Elements.GetById<UIText>("cursor")!;

            manager.AddRootElement(window);
        }

        private void UpdateMenu() {
            elements.Text = "Elements: " + manager.GetAllElements().Count();
            cursor.Text = $"Cursor X: {inputManager.MousePosition.X}, Y: {inputManager.MousePosition.Y}";
        }
    }
}
