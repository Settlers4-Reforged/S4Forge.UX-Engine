using CrashHandling;

using Forge.Config;
using Forge.S4;
using Forge.S4.Game;
using Forge.S4.Types;
using Forge.S4.Types.Native.Entities;
using Forge.UX.Input;
using Forge.UX.Rendering.Text;
using Forge.UX.S4;
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
        readonly IEntityApi entityManager;

        public static bool Enabled = true;

        private UIWindow? window;
        private UIText? entities, elements, cursor, uiScene, uiMenus;

        public UIDebugWindow(SceneManager manager, IInputManager inputManager, IEventApi eventManager, IEntityApi entityManager) {
            this.manager = manager;
            this.inputManager = inputManager;
            this.eventManager = eventManager;
            this.entityManager = entityManager;

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
                        .WithId("hurt")
                        .WithText("Hurt")
                        .Build(),

                    new S4TextBuilder(baseText).WithId("elements").WithText("Elements: 0").Build(),
                    new S4TextBuilder(baseText).WithId("cursor").WithText("Cursor X: 0, Y: 0").Build(),
                    new S4TextBuilder(baseText).WithId("ui-scene").WithText("UI Scene: Unknown").Build(),
                    new S4TextBuilder(baseText).WithId("ui-menu").WithFitText(true).WithText("UI Menu: Unknown\nUI Submenu: Unknown").Build(),

                    new S4TextBuilder(baseText).WithId("entities").WithSize(("100%", "500")).WithText("Entities: 0").WithFitText(true).Build(),
                }).Build();

            window = new S4WindowBuilder()
                .WithPosition((500, 0))
                .WithSize((395, 590))
                .WithChildPrefabs(
                    new List<IPrefab>() {
                        layoutPrefab
                    }
                )
                .Build().Instantiate<UIWindow>();
            window.OnTick += _ => {
                TickMenu();
            };
            window.OnProcess += (_, _) => {
                ProcessMenu();
            };

            UIStack layout = window.Elements.GetById<UIStack>("layout")!;
            layout.Elements.GetById<UIButton>("close")!.OnInteract += (_) => {
                window.Close();
            };
            layout.Elements.GetById<UIButton>("hurt")!.OnInteract += (_) => {
                unsafe {
                    var selection = entityManager.Selection;
                    if (selection != null && selection->Count > 0) {

                        for (int i = 0; i < selection->Count; i++) {
                            IEntity* entity = entityManager.GetEntity(selection->entityIds[i]);
                            if (entity != null && entity->health > 0) {
                                entity->health = 1;
                            }
                        }
                    }
                }
            };

            entities = layout.Elements.GetById<UIText>("entities")!;
            elements = layout.Elements.GetById<UIText>("elements")!;
            cursor = layout.Elements.GetById<UIText>("cursor")!;
            uiScene = layout.Elements.GetById<UIText>("ui-scene")!;
            uiMenus = layout.Elements.GetById<UIText>("ui-menu")!;

            manager.AddRootElement(window);

            UIStack buttonStack = new StackBuilder()
                .WithSize((322f * 0.75f, 60f * 0.75f))
                .WithPosition(("90%", 0.0f))
                .WithChildPrefabs(new List<IPrefab>() {
                    new S4ButtonBuilder()
                        .WithId("window-button")
                        .WithSize(("100%", "100%"))
                        .WithText("Forge Debug")
                        .Build()
                })
                .Build().Instantiate<UIStack>();

            UIButton windowButton = buttonStack.Elements.GetById<UIButton>("window-button")!;
            windowButton.OnInteract += (_) => {
                window.Open();
            };
            windowButton.OnProcess += (button, _) => {
                if (window.Visible)
                    button.Hide();
                else
                    button.Show();
            };

            manager.AddRootElement(buttonStack);
        }

        private void ProcessMenu() {
            cursor!.Text = $"Cursor X: {inputManager.MousePosition.X}, Y: {inputManager.MousePosition.Y}\n Delta X: {inputManager.MouseDelta.X}, Y: {inputManager.MouseDelta.Y}";
        }

        private void TickMenu() {
            entities!.Text = "No world active";
            unsafe {
                IEntity** allEntities = entityManager.BackingEntityPool;
                var selection = entityManager.Selection;
                if (selection != null && selection->Count > 0) {
                    entities!.Text = "Selection: " + selection->Count;

                    for (int i = 0; i < selection->Count; i++) {
                        IEntity* entity = entityManager.GetEntity(selection->entityIds[i]);

                        if (entity != null) {
                            EntityClass entityClass = entityManager.ClassOf(entity);
                            uint id = entity->id;

                            entities!.Text += $"\nEntity {i}: {id} {entityClass}, {entity->health}";
                        }
                    }

                } else if (allEntities != null) {
                    entities!.Text = "Entities: " + entityManager.EntityPoolSize;

                    for (uint i = 0; i < entityManager.EntityPoolSize; i++) {
                        IEntity* entity = entityManager.GetEntity(i);
                        if (entity != null) {
                            EntityClass entityClass = entityManager.ClassOf(entity);
                            uint id = entity->id;

                            entities!.Text += $"\nEntity {i}: {id} {entityClass}";
                        }
                    }
                }
            }

            elements!.Text = "Elements: " + manager.GetAllElements().Count();

            IUIManager uiManager = DI.Resolve<IUIManager>();
            S4UIScreenId currentScreen = uiManager.GetActiveScreen();
            uiScene!.Text = "UI Scene: " + currentScreen.ToString();

            uiMenus!.Text = "UI Menu: " + uiManager.GetActiveMenu() + "\nUI Submenu: " + uiManager.GetActiveSubmenu() + "\n TestXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

        }
    }
}
