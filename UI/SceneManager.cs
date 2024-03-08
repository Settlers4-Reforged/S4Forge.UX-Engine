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
        private readonly RootNode rootSceneNode = new RootNode();

        private readonly Lazy<IRenderer> _renderer;
        IRenderer Renderer => _renderer.Value;

        private readonly IInputManager inputManager;

        public SceneManager(Lazy<IRenderer> renderer, IInputManager inputManager) {
            this._renderer = renderer;
            this.inputManager = inputManager;

            this.inputManager.AddInputBlockingMiddleware(new InputBlockMiddleware(true, InputBlockingMiddleware, 1000));
        }

        private EventBlockFlags InputBlockingMiddleware(EventBlockFlags input) {
            return EventBlockFlags.None;
        }

        public void Init() {
#if DEBUG
            DI.Dependencies.Resolve<UIDebugWindow>();
#endif
            //UIEngine.RegisterS4ScreenChangeCallback(OnScreenChange);
        }

        private void OnScreenChange(UIScreen prev, UIScreen next) {
            foreach (UIWindow window in GetAllElements().OfType<UIWindow>()) {
                if (!window.PersistMenus)
                    window.Close();
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
                InputScene();

                RenderScene();

                inputManager.Update();
            } catch (Exception e) {
                Logger.LogError(e, "Error in scene manager");
            }
        }

        void InputScene() {
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

            TraverseScene(HandleMouseHover, HandleMouseHover, (g) => g.IgnoresMouse);

            void HandleInput(UIElement element, SceneGraphState state) {
                element.Input(state);
            }

            TraverseScene(HandleInput, HandleInput);

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

            TraverseScene(RenderComponents, RenderComponents, true);
        }

        void RenderComponents(UIElement parent, SceneGraphState state) {
            if (!parent.Visible)
                return;

            foreach (IUIComponent component in parent.Components) {
                Renderer.RenderUIComponent(component, parent, state);
            }
        }

        void TraverseScene(Action<UIGroup, SceneGraphState> OnGroup, Action<UIElement, SceneGraphState> OnElement) {
            TraverseScene(OnGroup, OnElement, (g) => false);
        }

        void TraverseScene(Action<UIGroup, SceneGraphState> OnGroup, Action<UIElement, SceneGraphState> OnElement, bool skipInvisible) {
            TraverseScene(OnGroup, OnElement, (g) => skipInvisible && !g.Visible);
        }

        void TraverseScene(Action<UIGroup, SceneGraphState> OnGroup, Action<UIElement, SceneGraphState> OnElement, Func<UIGroup, bool> ShouldSkipGroup) {
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
                OnGroup(group, state);

                SceneGraphState newState = state.ApplyGroup(group);

                foreach (UIElement el in group.GetSortedElements()) {
                    TraverseElement(el, newState);
                }
            }

            SceneGraphState baseState = SceneGraphState.Default();
            TraverseGroup(rootSceneNode, baseState);
        }

        class RootNode : UIGroup {
            public override Vector2 Size => DI.Dependencies.Resolve<IRenderer>().GetScreenSize();
        }
    }

    public struct SceneGraphState {
        public Vector2 CurrentPosition { get; private set; }
        public Vector2 CurrentContainerSize { get; private set; }
        public Vector2 CurrentScale { get; private set; } //Unused ATM
        public Vector4 ClippingRect { get; private set; }

        public int Depth { get; private set; }

        public bool DebugActive { get; private set; }

        public SceneGraphState(Vector2 currentPosition, Vector2 currentContainerSize, Vector2 currentScale, Vector4 clippingRect, int depth) {
            CurrentPosition = currentPosition;
            CurrentContainerSize = currentContainerSize;
            CurrentScale = currentScale;
            ClippingRect = clippingRect;
            Depth = depth;
            DebugActive = false;
        }
        public SceneGraphState Clone() {
            return new SceneGraphState(CurrentPosition, CurrentContainerSize, CurrentScale, ClippingRect, Depth) { DebugActive = this.DebugActive };
        }

        public SceneGraphState ApplyGroup(UIGroup group) {
            SceneGraphState next = this.Clone();

            // Transparent groups don't affect the layout state
            if (group.IsTransparent)
                return next;

            next.Depth++;

            next.CurrentPosition = ApplyRelativeModeToPosition(group.Position, group.PositionMode);
            next.CurrentContainerSize = ApplyRelativeModeToSize(group.Size, group.SizeMode);

            // Apply clipping:
            // For consideration... Clipping inside or outside of padding?
            if (group.ClipContent) {
                next.ClippingRect = new Vector4(group.Position, group.Size.X, group.Size.Y);
            }

            // Apply padding:
            Vector2 positionPadding = new Vector2(group.Padding.X, group.Padding.Y);
            Vector2 sizePadding = new Vector2(group.Padding.Z, group.Padding.W);
            next.CurrentPosition += positionPadding;
            next.CurrentContainerSize -= sizePadding + positionPadding;


            return next;
        }

        public static SceneGraphState Default() {
            Vector2 screenSize = DI.Dependencies.Resolve<IRenderer>().GetScreenSize();

            return new SceneGraphState(Vector2.Zero, Vector2.Zero, Vector2.One, new Vector4(0, 0, screenSize.X, screenSize.Y), 0);
        }

        /// <summary>
        /// Translates the element according to the current scene state with the relative modes applied
        /// </summary>
        public (Vector2 position, Vector2 size) TranslateElement(UIElement element) {
            Vector2 elementSize = element.Size;
            Vector2 relativePosition = element.Position;

            elementSize = ApplyRelativeModeToSize(elementSize, element.SizeMode);
            relativePosition = ApplyRelativeModeToPosition(relativePosition, element.PositionMode);

            return (relativePosition, elementSize);
        }

        /// <summary>
        /// Translates the component according to the current scene state with the relative modes applied
        /// </summary>
        public (Vector2 position, Vector2 size) TranslateComponent(UIElement element, IUIComponent component) {
            var transElement = TranslateElement(element);

            Vector2 componentSize = component.Size;
            Vector2 componentPos = component.Position;

            componentSize = ApplyRelativeModeToSize(componentSize, component.SizeMode, transElement.size);
            componentPos = ApplyRelativeModeToPosition(componentPos, component.PositionMode, transElement.size, transElement.position);

            return (componentPos, componentSize);
        }

        private Vector2 ApplyRelativeModeToSize(Vector2 target, (PositioningMode x, PositioningMode y) mode, Vector2? currentContainerSize = null) {
            currentContainerSize ??= CurrentContainerSize;

            Vector2 ApplyModeToAxis(PositioningMode axisMode, Vector2 axisValue) {
                Vector2 output = Vector2.Zero;
                if (axisMode.HasFlag(PositioningMode.Absolute) || axisMode == PositioningMode.Normal) {
                    output = axisValue;
                    if (axisMode.HasFlag(PositioningMode.Relative)) {
                        output *= DI.Dependencies.Resolve<IRenderer>().GetScreenSize();
                    }
                } else if (axisMode.HasFlag(PositioningMode.Relative)) {
                    output = axisValue * currentContainerSize.Value;
                }

                return output;
            }

            Vector2 output = Vector2.Zero;

            output += ApplyModeToAxis(mode.x, target * Vector2.UnitX);
            output += ApplyModeToAxis(mode.y, target * Vector2.UnitY);

            return output;
        }

        private Vector2 ApplyRelativeModeToPosition(Vector2 target, (PositioningMode x, PositioningMode y) mode, Vector2? currentContainerSize = null, Vector2? currentPosition = null) {
            currentContainerSize ??= CurrentContainerSize;
            currentPosition ??= CurrentPosition;

            Vector2 GetAdjustedOffset(PositioningMode axisMode, Vector2 dir) {
                Vector2 output = Vector2.Zero;

                // When the mode is absolute, the position is already in screen space
                // In relative or normal mode, the position needs to be adjusted by the current position of the group/container
                // as that is the output of ApplyRelativeModeToSize
                if (axisMode.HasFlag(PositioningMode.Relative) || axisMode == PositioningMode.Normal) {
                    return currentPosition!.Value * dir;
                }

                return output;
            }

            // First apply the relative mode to the position according to the current container size
            Vector2 output = ApplyRelativeModeToSize(target, mode, currentContainerSize);
            // Then adjust the position by the current position of the group/container
            output += GetAdjustedOffset(mode.x, Vector2.UnitX);
            output += GetAdjustedOffset(mode.y, Vector2.UnitY);

            return output;
        }
    }
}
