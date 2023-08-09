using Forge.UX.UI.Elements.Grouping.Layout;

using System.Collections.Generic;
using System.Linq;

namespace Forge.UX.UI.Elements.Grouping {
    public class UIGroup : UIElement, IUILayout {
        public List<UIElement> Elements { get; set; }

        public bool ClipContent { get; set; } = false;

        public virtual bool Visible { get; set; } = true;

        public UIGroup() {
            Elements ??= new List<UIElement>();
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
