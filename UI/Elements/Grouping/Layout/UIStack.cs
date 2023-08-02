using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Layout {
    internal class UIRows : UIGroup, IUILayout {
        private readonly float rowHeight;
        private int Rows => Elements.Count;
        private readonly Vector2 direction;

        public UIRows(float rowHeight, bool isHorizontal = false) {
            this.rowHeight = rowHeight;

            direction = isHorizontal ? new Vector2(1, 0) : new Vector2(0, 1);
        }

        public UIRows AddRows(params UIElement[] element) {
            Elements.AddRange(element);

            Relayout();

            return this;
        }

        public void SetRows(params UIElement[] elements) {
            Elements.Clear();
            Elements.AddRange(elements);

            Relayout();
        }

        public void Relayout() {
            int i = 0;
            foreach (UIElement element in Elements) {
                element.Position = direction * rowHeight * i;
                i++;
            }
        }
    }
}
