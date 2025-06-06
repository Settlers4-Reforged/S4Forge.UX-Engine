﻿using DryIoc;

using Forge.Config;
using Forge.Logging;
using Forge.S4.Types;
using Forge.UX.Input;
using Forge.UX.Native;
using Forge.UX.Plugin;
using Forge.UX.Rendering;
using Forge.UX.S4;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Forge.UX.UI {
    public class SceneManager {
        private readonly RootNode rootSceneNode;

        private readonly Lazy<IRenderer> _renderer;
        IRenderer Renderer => _renderer.Value;

        private readonly IInputManager inputManager;

        public SceneManager(Lazy<IRenderer> renderer, IInputManager inputManager) {
            this._renderer = renderer;
            this.inputManager = inputManager;

            this.inputManager.InputEventHandler += HandleInput;

            rootSceneNode = new RootNode();
            rootSceneNode.Attach(null!);
        }

        private EventBlockFlags InputBlockingMiddleware(EventBlockFlags input) {
            if (hoverStack.Count(element => element.Visible) == 0)
                return input;

            //TODO: add "reset" of mouse position to maybe -1,-1 to allow game to remove hover state of native elements

            //input |= EventBlockFlags.Mouse;
            input |= EventBlockFlags.MouseClickDown;
            input |= EventBlockFlags.MouseWheel;

            bool stackHasFocus = hoverStack.Any(e => e is IUIFocusable { Focused: true });
            if (stackHasFocus)
                input |= EventBlockFlags.Keyboard;

            return input;
        }

        public void Init() {
            PrefabManager prefabManager = DI.Resolve<PrefabManager>();

            // In case PluginScenes are not registered as PluginPrefabs, pull both and union for those that actually behave
            var pluginPrefabs = DI.Dependencies.ResolveMany<IPluginPrefab>();
            var pluginScenes = DI.Dependencies.ResolveMany<IPluginScene>();
            foreach (IPluginPrefab pluginPrefab in pluginPrefabs.Union(pluginScenes)) {
                try {

                    pluginPrefab.Build(DI.Dependencies.Resolve<SceneBuilder>());
                    if (pluginPrefab.Prefab == null) {
                        Logger.LogError(null, "PluginPrefab {0} was built but produced no prefab!",
                            pluginPrefab.GetType().Name);
                        continue;
                    }

                    if (!pluginPrefab.AutoRegister) continue;
                    prefabManager.RegisterPrefab(pluginPrefab.Prefab!, pluginPrefab.TagName);

                    if (pluginPrefab is not IPluginScene pluginScene) continue;

                    AddRootElement(pluginScene.Group.Instantiate());
                } catch (Exception e) {
                    Logger.LogError(e, "Error while building plugin prefab {0}", pluginPrefab.GetType().Name);
                }
            }
        }

        public IEnumerable<UIElement> GetAllElements() {
            return rootSceneNode.Elements.GetAllElementsInTree();
        }

        public SceneTree GetRootElements() {
            return rootSceneNode.Elements;
        }

        public void AddRootElement(UIElement e) {
            if (e is not UIGroup) {
                throw new ArgumentException("Root elements must be of type UIGroup");
            }

            e.Attach(rootSceneNode);
            rootSceneNode.Elements.Add(e);
        }

        /// <summary>
        /// Stack of elements that are currently hovered over
        /// </summary>
        private readonly Stack<UIElement> hoverStack = new Stack<UIElement>();

        private void HandleInput(ref InputEvent e) {
            IEnumerable<UIElement> elementStack = hoverStack;

            if (e.Type == InputType.Windows) {
                // Only process windows input events for elements that are interested in them
                elementStack = hoverStack.Where(element => element.ProcessWindowsInputEvents);
            }

            foreach (UIElement element in elementStack) {
                element.Input(ref e);

                if (e.IsHandled)
                    break;
            }

            if (e.IsHandled) return;
            //TODO: find out if this is processing intensive if (e.Type == InputType.Windows) return;

            // Unhandled input events:
            foreach (UIElement element in GetAllElements().Where(element => element.ProcessUnhandledInputEvents)) {
                element.UnhandledInput(e);

                if (e.IsHandled)
                    break;
            }
        }

        public void DoFrame() {
            try {
                IUIManager uiManager = DI.Resolve<IUIManager>();
                uiManager.GetActiveMenu();
                uiManager.GetActiveScreen();

                ProcessScene();

                RenderScene();

                inputManager.Update();
            } catch (Exception e) {
                Logger.LogError(e, "Error during frame calculation in scene manager");
            }
        }

        public void DoTick() {
            try {
                ProcessTick();
            } catch (Exception e) {
                Logger.LogError(e, "Error during tick calculation in scene manager");
            }
        }

        void ProcessTick() {
            rootSceneNode.TraverseScene(null, (e, _) => { e.Tick(); });
        }

        void ProcessScene() {
            #region Mouse

            UIElement? currentHoverElement = null;
            int currentHoverDepth = int.MinValue;

            void HandleMouseHover(UIElement element, SceneGraphState state) {
                bool isHigherThanCurrent = currentHoverElement == null || element.ZIndex + state.Depth >= currentHoverElement.ZIndex + currentHoverDepth;

                if (isHigherThanCurrent && element.ProcessInputEvents) {
                    (Vector2 elementPosition, Vector2 elementSize) = state.TranslateElement(element);

                    bool mouseIsInElement = inputManager.IsMouseInRectangle(new Vector4(elementPosition, elementSize.X, elementSize.Y));
                    if (!mouseIsInElement) {
                        element.IsMouseHover = false;
                        return;
                    }

                    element.IsMouseHover = true;
                    hoverStack.Push(element);

                    currentHoverElement = element;
                    currentHoverDepth = state.Depth;
                } else {
                    element.IsMouseHover = false;
                }
            }

            var previousHoverStack = new Stack<UIElement>(hoverStack);
            hoverStack.Clear();
            rootSceneNode.TraverseScene(null, HandleMouseHover);

            // MouseLeave on all elements that are no longer hovered
            foreach (UIElement element in previousHoverStack.Except(hoverStack)) {
                if (!element.IsMouseHover) {
                    InputEvent inputEvent = new InputEvent() {
                        Type = InputType.MouseLeave
                    };
                    element.Input(ref inputEvent);
                }
            }

            // MouseEnter on all elements that are newly hovered
            foreach (UIElement element in hoverStack.Except(previousHoverStack)) {
                if (element.IsMouseHover) {
                    InputEvent inputEvent = new InputEvent() {
                        Type = InputType.MouseEnter
                    };

                    element.Input(ref inputEvent);
                    if (inputEvent.IsHandled)
                        break;
                }
            }

            #endregion

            void HandleProcessing(UIElement element, SceneGraphState state) {
                element.Process(state);
            }

            rootSceneNode.TraverseScene(null, HandleProcessing);

            // Handle Mouse click events:
            Keys[] keys = { Keys.LButton, Keys.MButton, Keys.RButton };

            int k = 0;
            foreach (Keys key in keys) {
                if (inputManager.IsKeyDown(key)) {
                    foreach (UIElement element in GetAllElements()) {
                        element.OnMouseGlobalClickDown(k);
                    }
                }

                if (inputManager.IsKeyUp(key)) {
                    foreach (UIElement element in GetAllElements()) {
                        element.OnMouseGlobalClickUp(k);
                    }
                }

                k++;
            }

        }


        void RenderScene() {
            Renderer.ClearScreen();
            rootSceneNode.TraverseScene(RenderGroup, RenderComponents, true);

            foreach (UIElement element in GetAllElements()) {
                element.IsDirty = false;
            }
        }

        void RenderComponents(UIElement parent, SceneGraphState state) {
            if (!parent.IsDirty || !parent.Visible)
                return;

            foreach (IUIComponent component in parent.Components) {
                Renderer.RenderUIComponent(component, parent, state);
            }
        }

        void RenderGroup(UIGroup group, SceneGraphState state) {
            if (group is { Visible: false, IsDirty: false })
                return;

            Renderer.RenderGroup(group, state);
        }

        class RootNode : UIGroup {
            public override Vector2 Size {
                get => DI.Dependencies.Resolve<IRenderer>().GetScreenSize();
            }

            public override void Process(SceneGraphState state) {
                base.Process(state);
                IsDirty = true;
            }

            public RootNode() : base() {
                ProcessInputEvents = false;
                ProcessUnhandledInputEvents = false;
                ProcessWindowsInputEvents = false;

                Elements.CollectionChanged += (sender, args) => InvalidateLayout();
            }

            // Override the default UIGroup behavior to prevent all child groups from being marked as dirty involuntarily
            public override bool IsDirty { get; set; }
        }
    }
}
