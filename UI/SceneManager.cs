using DryIoc;

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
using Forge.UX.UI.Elements.Interaction;
using Forge.UX.UI.Prefabs;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;

namespace Forge.UX.UI {
    public class SceneManager {
        private readonly CLogger Logger;
        private readonly RootNode rootSceneNode;

        private readonly Lazy<IRenderer> _renderer;
        IRenderer Renderer => _renderer.Value;

        private readonly IInputManager inputManager;

        public SceneManager(Lazy<IRenderer> renderer, IInputManager inputManager, CLogger logger) {
            this._renderer = renderer;
            this.inputManager = inputManager;
            Logger = logger.WithEnumCategory(ForgeLogCategory.UI);

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
                        Logger.LogF(LogLevel.Error, "PluginPrefab {0} was built but produced no prefab!", pluginPrefab.GetType().Name);
                        continue;
                    }

                    if (!pluginPrefab.AutoRegister) continue;
                    prefabManager.RegisterPrefab(pluginPrefab.Prefab!, pluginPrefab.TagName);

                    if (pluginPrefab is not IPluginScene pluginScene) continue;

                    AddRootElement(pluginScene.Group.Instantiate());
                } catch (Exception e) {
                    Logger.TraceExceptionF(LogLevel.Error, e, "Error while building plugin prefab {0}", pluginPrefab.GetType().Name);
                }
            }

            DI.Dependencies.Resolve<IRenderer>().OnUpdateRenderer += () => {
                rootSceneNode.InvalidateLayout();
                foreach (var element in GetRootElements()) {
                    element.Dirty();
                }
            };
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


        UIElement HighestParent(UIElement element) {
            UIElement? parent = element;
            while (parent?.Parent != null && parent.Parent is not RootNode) {
                parent = parent.Parent;
            }
            return parent ?? element;
        }

        /// <summary>
        /// Stack of elements that are currently hovered over
        /// </summary>
        public Stack<UIElement> hoverStack = new Stack<UIElement>();
        Stack<UIElement> previousHoverStack = new Stack<UIElement>();
        Stack<UIElement> sortedHoverStack = new Stack<UIElement>();
        Stack<UIElement> intermediateHoverStack = new Stack<UIElement>();

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
                Logger.TraceExceptionF(LogLevel.Error, e, "Error during frame calculation in scene manager");
            }
        }

        public void DoTick() {
            try {
                ProcessTick();
            } catch (Exception e) {
                Logger.TraceExceptionF(LogLevel.Error, e, "Error during tick calculation in scene manager");
            }
        }

        void ProcessTick() {
            static void TickElement(UIElement element, in SceneGraphState state) {
                element.Tick();
            }
            rootSceneNode.TraverseScene(null, TickElement);
        }

        UIElement? activeHoverElement = null;
        void ProcessScene() {
            #region Mouse

            void HandleMouseHover(UIElement element, in SceneGraphState state) {
                if (element.ProcessInputEvents) {
                    (Vector2 elementPosition, Vector2 elementSize) = state.TranslateElement(element);

                    Vector2 positionPadding = new Vector2(state.ContainerGroup?.Padding.X ?? 0, state.ContainerGroup?.Padding.Y ?? 0);
                    Vector2 sizePadding = new Vector2(state.ContainerGroup?.Padding.Z ?? 0, state.ContainerGroup?.Padding.W ?? 0);
                    elementPosition -= positionPadding;
                    elementSize += sizePadding + positionPadding;

                    Vector4 elementRect = new Vector4(elementPosition, elementSize.X, elementSize.Y);
                    bool mouseIsInElement = inputManager.IsMouseInRectangle(elementRect.Intersection(state.ClippingRect));
                    if (!mouseIsInElement) {
                        return;
                    }

                    hoverStack.Push(element);
                } else {
                    element.IsMouseHover = false;
                }
            }

            (hoverStack, previousHoverStack) = (previousHoverStack, hoverStack);
            hoverStack.Clear();
            rootSceneNode.TraverseScene(null, HandleMouseHover);

            // Reverse the hoverStack to conform to the rendering order
            // This has to keep groups together
            sortedHoverStack.Clear();
            UIElement? highestParent = null;
            foreach (var element in hoverStack) {
                UIElement parent = HighestParent(element);

                if (highestParent != parent) {
                    foreach (var queued in intermediateHoverStack) {
                        sortedHoverStack.Push(queued);
                    }

                    intermediateHoverStack.Clear();
                    highestParent = parent;
                }
                intermediateHoverStack.Push(element);
            }

            // Push any remaining elements in the intermediate stack
            foreach (var queued in intermediateHoverStack) {
                sortedHoverStack.Push(queued);
            }
            (hoverStack, sortedHoverStack) = (sortedHoverStack, hoverStack);

            UIElement prevActiveHoverElement = activeHoverElement;

            InputEvent leaveEvent = new InputEvent() {
                Type = InputType.MouseLeave
            };
            // MouseLeave on all elements that are no longer hovered
            foreach (UIElement element in previousHoverStack.Except(hoverStack)) {
                if (element.IsMouseHover) continue;

                element.Input(ref leaveEvent);
                if (activeHoverElement == element) {
                    activeHoverElement = null;
                }
            }
            InputEvent enterEvent = new InputEvent() {
                Type = InputType.MouseEnter
            };
            // MouseEnter on all elements that are newly hovered
            // Stop when an element handled the event
            foreach (UIElement element in hoverStack) {
                element.Input(ref enterEvent);

                if (!enterEvent.IsHandled) continue;
                activeHoverElement = element;
                break;
            }

            if (prevActiveHoverElement != activeHoverElement) {
                prevActiveHoverElement?.Input(ref leaveEvent);
            }

            #endregion

            void HandleProcessing(UIElement element, in SceneGraphState state) {
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
            rootSceneNode.TraverseScene(RenderGroup, RenderComponents, true);

            foreach (UIElement element in GetAllElements()) {
                element.IsDirty = false;
            }
        }

        void RenderComponents(UIElement parent, in SceneGraphState state) {
            if (!parent.IsDirty || !parent.Visible)
                return;

            foreach (IUIComponent component in parent.Components) {
                Renderer.RenderUIComponent(component, parent, state);
            }
        }

        void RenderGroup(UIGroup group, in SceneGraphState state) {
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
