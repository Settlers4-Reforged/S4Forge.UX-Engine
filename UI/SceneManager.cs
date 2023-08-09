using Forge.S4.Types;
using Forge.UX.Input;
using Forge.UX.Native;
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
    public static class SceneManager {
        private static readonly RootNode rootSceneNode = new RootNode();


        static void Init() {
            GameEventManager.RegisterS4ScreenChangeCallback(OnScreenChange);
        }

        private static void OnScreenChange(UIScreen prev, UIScreen next) {
            foreach (UIWindow window in rootSceneNode.GetAllSceneElements().OfType<UIWindow>()) {
                if (!window.PersistMenus)
                    window.Close();
            }
        }

        static void DoFrame() {
            InputScene();

            RenderScene();
        }

        static void InputScene() {
            UIElement? currentHoverElement = null;

            void HandleMouse(UIElement element, SceneGraphState state) {
                if (element.IgnoresMouse)
                    return;

                bool newHoverState = element.IsMouseHover;
                bool prevHoverState = element.IsMouseHover;

                if (currentHoverElement == null || element.ZIndex > currentHoverElement.ZIndex) {
                    (Vector2 elementPosition, Vector2 elementSize) = state.TranslateElement(element);
                    newHoverState = InputManager.IsMouseInRectangle(new Vector4(elementPosition, elementSize.X, elementSize.Y));

                    if (element.IsMouseHover) {
                        currentHoverElement = element;
                    }
                }

                if (prevHoverState != newHoverState) {
                    if (prevHoverState)
                        element.OnMouseLeave();
                    else
                        element.OnMouseEnter();
                }
            }

            TraverseScene(HandleMouse, HandleMouse, (g) => g.IgnoresMouse);

            void HandleInput(UIElement element, SceneGraphState state) {
                element.Input(state);
            }

            TraverseScene(HandleInput, HandleInput);
        }
        static void RenderScene() {
            TraverseScene(RenderComponents, RenderComponents, true);
        }

        static void RenderComponents(UIElement parent, SceneGraphState state) {
            foreach (IUIComponent component in parent.Components) {
                UXEngine.R.RenderUIComponent(component, parent, state);
            }
        }

        static void TraverseScene(Action<UIGroup, SceneGraphState> OnGroup, Action<UIElement, SceneGraphState> OnElement) {
            TraverseScene(OnGroup, OnElement, (g) => false);
        }

        static void TraverseScene(Action<UIGroup, SceneGraphState> OnGroup, Action<UIElement, SceneGraphState> OnElement, bool skipInvisible) {
            TraverseScene(OnGroup, OnElement, (g) => skipInvisible && g.Visible);
        }

        static void TraverseScene(Action<UIGroup, SceneGraphState> OnGroup, Action<UIElement, SceneGraphState> OnElement, Func<UIGroup, bool> ShouldSkipGroup) {
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
            public List<UIElement> GetAllSceneElements() {
                List<UIElement> elements = new List<UIElement>();

                static void GetChildElements(UIGroup parent, ref List<UIElement> elements) {
                    foreach (UIElement child in parent.Elements) {
                        if (child is UIGroup g) {
                            GetChildElements(g, ref elements);
                        }

                        elements.Add(child);
                    }
                }


                GetChildElements(this, ref elements);

                return elements;
            }
        }
    }

    public struct SceneGraphState {
        public Vector2 CurrentPosition { get; private set; }
        public Vector2 CurrentScale { get; private set; } //Unused ATM
        public Vector4 ClippingRect { get; private set; }

        public int Depth { get; private set; }

        public bool DebugActive { get; private set; }

        public SceneGraphState(Vector2 currentPosition, Vector2 currentScale, Vector4 clippingRect, int depth) {
            CurrentPosition = currentPosition;
            CurrentScale = currentScale;
            ClippingRect = clippingRect;
            Depth = depth;
            DebugActive = false;
        }
        public SceneGraphState Clone() {
            return new SceneGraphState(CurrentPosition, CurrentScale, ClippingRect, Depth) { DebugActive = this.DebugActive };
        }

        public SceneGraphState ApplyGroup(UIGroup group) {
            SceneGraphState next = this.Clone();

            next.Depth++;

            if (group.PositionAbsolute) {
                next.CurrentPosition = group.Position;
            } else {
                next.CurrentPosition += group.Position;
            }

            if (group.ClipContent) {
                next.ClippingRect = new Vector4(group.Position, group.Size.X, group.Size.Y);
            }

            return next;
        }

        public static SceneGraphState Default() {
            Vector2 screenSize = UXEngine.R.GetScreenSize();

            return new SceneGraphState(Vector2.Zero, Vector2.One, new Vector4(0, 0, screenSize.X, screenSize.Y), 0);
        }

        public (Vector2 position, Vector2 size) TranslateElement(UIElement element) {
            if (element.PositionAbsolute) {
                return (element.Position, element.Size);
            } else {
                return (CurrentPosition + element.Position, CurrentScale * element.Size);
            }
        }
    }
}
