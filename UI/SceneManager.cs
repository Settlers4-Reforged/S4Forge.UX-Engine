using DryIoc;

using Forge.Config;
using Forge.Logging;
using Forge.S4.Types;
using Forge.UX.Debug;
using Forge.UX.Input;
using Forge.UX.Native;
using Forge.UX.Rendering;
using Forge.UX.S4;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;
using Forge.UX.UI.Elements.Grouping.Display;

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

            this.inputManager.AddInputBlockingMiddleware(new InputBlockMiddleware(true, InputBlockingMiddleware, 1000));

            rootSceneNode = new RootNode();
            rootSceneNode.Attach(this);
        }

        private EventBlockFlags InputBlockingMiddleware(EventBlockFlags input) {
            return EventBlockFlags.None;
        }

        public void Init() {
#if DEBUG
            DI.Dependencies.Resolve<UIDebugWindow>();
#endif

            PrefabManager prefabManager = DI.Resolve<PrefabManager>();

            var pluginPrefabs = DI.Dependencies.ResolveMany<IPluginPrefab>();
            foreach (IPluginPrefab pluginPrefab in pluginPrefabs) {
                pluginPrefab.Build(DI.Dependencies.Resolve<SceneBuilder>());

                if (!pluginPrefab.AutoRegister) continue;
                prefabManager.RegisterPrefab(pluginPrefab.Prefab!);

                if (pluginPrefab is not IPluginScene pluginScene) continue;

                AddRootElement(pluginScene.Group.Instantiate());
                pluginScene.AfterSceneTreeAdd();
            }
        }

        public IEnumerable<UIElement> GetAllElements() {
            return rootSceneNode.Elements.GetAllElementsInTree();
        }

        public SceneTree GetRootElements() {
            return rootSceneNode.Elements;
        }

        public void AddRootElement(UIElement e) {
            rootSceneNode.Elements.Add(e);
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
                Logger.LogError(e, "Error in scene manager");
            }
        }

        void ProcessScene() {
            UIElement? currentHoverElement = null;
            int currentHoverDepth = int.MinValue;

            void HandleMouseHover(UIElement element, SceneGraphState state) {
                if (element.IgnoresMouse)
                    return;

                bool newHoverState = element.IsMouseHover;
                bool prevHoverState = element.IsMouseHover;

                if (currentHoverElement == null || element.ZIndex + state.Depth >= currentHoverElement.ZIndex + currentHoverDepth) {
                    (Vector2 elementPosition, Vector2 elementSize) = state.TranslateElement(element);
                    element.IsMouseHover = newHoverState = inputManager.IsMouseInRectangle(new Vector4(elementPosition, elementSize.X, elementSize.Y));

                    if (newHoverState) {
                        currentHoverElement = element;
                        currentHoverDepth = state.Depth;
                    }
                }

                if (prevHoverState != newHoverState) {
                    if (prevHoverState)
                        element.OnMouseLeave();
                    else
                        element.OnMouseEnter();
                }
            }

            TraverseScene(null, HandleMouseHover, (g) => g.IgnoresMouse);

            void HandleProcessing(UIElement element, SceneGraphState state) {
                element.Input(state);
            }

            void HandleGroupProcessing(UIGroup group, SceneGraphState state) {
                //group.Input(state);

                foreach (UIElement child in group.Elements) {
                    if (!child.IsDirty) continue;

                    group.IsDirty = true;
                    break;
                }

                if (!group.IsDirty)
                    return;

                // Propagate dirty state down the tree
                // TODO: maybe add a method to only re-render parts of the group - possibly add a "has alpha" check, to see if we can just render the group as a whole
                foreach (UIElement child in group.Elements.GetAllElementsInTree()) {
                    child.IsDirty = true;
                }
            }

            TraverseScene(HandleGroupProcessing, HandleProcessing);

            // Handle Mouse click events:
            Keys[] keys = { Keys.LButton, Keys.MButton, Keys.RButton };

            int k = 0;
            foreach (Keys key in keys) {
                if (inputManager.IsKeyDown(key)) {
                    currentHoverElement?.OnMouseClickDown(k);

                    foreach (UIElement element in GetAllElements()) {
                        if (!element.IgnoresMouse) {
                            element.OnMouseGlobalClickDown(k);
                        }
                    }
                }

                if (inputManager.IsKeyUp(key)) {
                    currentHoverElement?.OnMouseClickUp(k);

                    foreach (UIElement element in GetAllElements()) {
                        if (!element.IgnoresMouse) {
                            element.OnMouseGlobalClickUp(k);
                        }
                    }
                }

                k++;
            }

        }


        void RenderScene() {
            Renderer.ClearScreen();
            TraverseScene(RenderGroup, RenderComponents, true);
        }

        void RenderComponents(UIElement parent, SceneGraphState state) {
            if (!parent.IsDirty)
                return;

            parent.IsDirty = false;

            if (!parent.Visible)
                return;


            foreach (IUIComponent component in parent.Components) {
                Renderer.RenderUIComponent(component, parent, state);
            }
        }

        void RenderGroup(UIGroup group, SceneGraphState state) {
            if (!group.Visible)
                return;

            Renderer.RenderGroup(group, state);
        }

        void TraverseScene(Action<UIGroup, SceneGraphState>? OnGroup, Action<UIElement, SceneGraphState> OnElement) {
            TraverseScene(OnGroup, OnElement, (g) => false);
        }

        void TraverseScene(Action<UIGroup, SceneGraphState>? OnGroup, Action<UIElement, SceneGraphState> OnElement, bool skipInvisible) {
            TraverseScene(OnGroup, OnElement, (g) => skipInvisible && !g.Visible);
        }

        void TraverseScene(Action<UIGroup, SceneGraphState>? OnGroup, Action<UIElement, SceneGraphState> OnElement, Func<UIGroup, bool> ShouldSkipGroup) {
            void TraverseElement(UIElement element, SceneGraphState state) {
                if (element is UIGroup g) {
                    if (!ShouldSkipGroup(g)) {
                        TraverseGroup(g, state);
                    }
                } else {
                    OnElement(element, state);
                }
            }

            void TraverseGroup(UIGroup group, SceneGraphState state) {
                OnElement(group, state);

                SceneGraphState newState = state.ApplyGroup(group);

                foreach (UIElement el in group.GetSortedElements()) {
                    TraverseElement(el, newState);
                }

                OnGroup?.Invoke(group, state);
            }

            SceneGraphState baseState = SceneGraphState.Default(rootSceneNode);
            TraverseGroup(rootSceneNode, baseState);
        }

        class RootNode : UIGroup {
            public override Vector2 Size => DI.Dependencies.Resolve<IRenderer>().GetScreenSize();
        }
    }
}
