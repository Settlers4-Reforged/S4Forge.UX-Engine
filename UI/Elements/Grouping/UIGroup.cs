using Forge.UX.UI.Elements.Grouping.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping {
    public class UIGroup : UIElement, IUILayout {
        public SceneTree Elements { get; set; }

        /// <summary>
        /// A list of elements combined with the elements of all recursive transparent groups.
        /// </summary>
        public IEnumerable<UIElement> TransparentElements =>
            Elements
                .Where(element => element is UIGroup { IsTransparent: false } or not UIGroup)
                .Concat(Elements
                    .Where(element => element is UIGroup { IsTransparent: true })
                    .SelectMany(element => ((UIGroup)element).TransparentElements)
                );

        public bool ClipContent { get; set; } = false;


        public override void InvalidateLayout() {
            base.InvalidateLayout();
            foreach (UIElement element in Elements.GetAllElementsInTree()) {
                element.InvalidateLayout();
            }
        }

        public override SceneGraphState GraphState {
            get {
                // On demand creation of the graph state
                _graphState ??= Parent?.GraphState.ApplyGroup(this) ?? SceneGraphState.Default(this).ApplyGroup(this);
                return _graphState.Value;
            }
            set => _graphState = value;
        }

        public override bool IsDirty {
            get => base.IsDirty;
            set {
                if (value) {
                    foreach (UIElement element in Elements.GetAllElementsInTree()) {
                        if (element is UIGroup)
                            continue;

                        element.IsDirty = value;
                    }
                }

                base.IsDirty = value;
            }
        }

        private Vector4 _padding = Vector4.Zero;
        public Vector4 Padding {
            get => _padding;
            set {
                bool changed = _padding != value;
                _padding = value;
                if (!changed) return;
                InvalidateLayout();
                Dirty();
            }
        }

        private Vector2 _offset = Vector2.Zero;
        public Vector2 Offset {
            get => _offset;
            set {
                bool changed = _offset != value;
                _offset = value;
                if (!changed) return;
                InvalidateLayout();
                Dirty();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the group is transparent in layout behavior.
        /// </summary>
        /// <remarks>
        /// When a group is marked as transparent, its child elements get effected by the layout behavior
        /// of the parent group.
        /// <br/>
        /// <b>Layout transparency and relative positioning and sizing are mutually exclusive.</b>
        /// </remarks>
        public virtual bool IsTransparent {
            get => isTransparent;
            set {
                if (SizeMode != (PositioningMode.Normal, PositioningMode.Normal))
                    throw new ArgumentException("Cannot set IsTransparent on a group with a relative size mode!");
                if (PositionMode != (PositioningMode.Normal, PositioningMode.Normal))
                    throw new ArgumentException("Cannot set IsTransparent on a group with a relative position mode!");

                isTransparent = value;
            }
        }
        private bool isTransparent = false;

        public UIGroup() {
            Elements ??= new SceneTree();
        }

        public IEnumerable<UIElement> GetSortedElements() {
            //TODO: cache ordering (or make it order on insert)
            return Elements.OrderBy(element => element.ZIndex);
        }

        public UIElement? GetElement(string id) {
            foreach (var element in Elements) {
                if (element.Id == id) {
                    return element;
                }

                if (element is UIGroup g) {
                    UIElement? found = g.GetElement(id);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public override void Attach(UIGroup? owner) {
            base.Attach(owner);
            Elements.Attach(this);
        }

        public virtual void Relayout() {
            InvalidateLayout();

            foreach (IUILayout element in Elements.OfType<IUILayout>()) {
                element.Relayout();
            }
        }


        public void TraverseScene(Action<UIGroup, SceneGraphState>? OnGroup, Action<UIElement, SceneGraphState> OnElement) {
            TraverseScene(OnGroup, OnElement, (g) => false);
        }

        public void TraverseScene(Action<UIGroup, SceneGraphState>? OnGroup, Action<UIElement, SceneGraphState> OnElement, bool skipInvisible) {
            TraverseScene(OnGroup, OnElement, (g) => skipInvisible && !g.Visible);
        }

        public void TraverseScene(Action<UIGroup, SceneGraphState>? OnGroup, Action<UIElement, SceneGraphState> OnElement, Func<UIGroup, bool> ShouldSkipGroup) {
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
                if (group != this)
                    OnElement(group, state);

                SceneGraphState newState = group.GraphState;

                foreach (UIElement el in group.GetSortedElements()) {
                    TraverseElement(el, newState);
                }

                if (group != this)
                    OnGroup?.Invoke(group, state);
            }

            TraverseGroup(this, GraphState);
        }
    }
}
