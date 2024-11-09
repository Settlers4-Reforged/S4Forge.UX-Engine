using DryIoc;

using Forge.Config;
using Forge.Logging;
using Forge.S4.Types;
using Forge.UX.Debug;
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
            rootSceneNode.Attach(this);
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
            if (e is not UIGroup) {
                throw new ArgumentException("Root elements must be of type UIGroup");
            }

            e.Attach(this);
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
            TraverseScene(null, (e, _) => { e.Tick(); });
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
            TraverseScene(null, HandleMouseHover);

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

            void HandleGroupProcessing(UIGroup group, SceneGraphState state) {
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
            if (group is { Visible: false, IsDirty: false })
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
                if (group != rootSceneNode)
                    OnElement(group, state);

                SceneGraphState newState = state.ApplyGroup(group);

                foreach (UIElement el in group.GetSortedElements()) {
                    TraverseElement(el, newState);
                }

                if (group != rootSceneNode)
                    OnGroup?.Invoke(group, state);
            }

            SceneGraphState baseState = SceneGraphState.Default(rootSceneNode);
            TraverseGroup(rootSceneNode, baseState);
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
            }
        }
    }
}
