using System.Collections.Generic;
using System.Linq;

namespace Forge.UX.UI.Elements.Grouping {
    public class UIGroup : UIElement {
        public List<UIElement> Elements { get; set; }

        public bool ClipContent { get; set; } = false;

        public virtual bool Visible { get; set; } = true;

        public UIGroup() {
            Elements ??= new List<UIElement>();
        }

        public IEnumerable<UIElement> GetSortedElements() {
            return Elements.OrderBy(element => element.ZIndex);
        }
    }
}
