using Forge.UX.UI.Elements.Grouping.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

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

        public virtual bool Visible { get; set; } = true;

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
            return Elements.OrderBy(element => element.ZIndex);
        }

        public UIElement? GetElement(string id) {
            foreach (var element in Elements) {
                if (element.Id == id) {
                    return element;
                }

                if (element is UIGroup g) {
                    return g.GetElement(id);
                }
            }

            return null;
        }


        public virtual void Relayout() {
            foreach (IUILayout element in Elements.OfType<IUILayout>()) {
                element.Relayout();
            }
        }
    }
}
