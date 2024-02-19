using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Layout {
    public class UIStack : UIGroup, IUILayout {
        private readonly float rowHeight;
        private int Rows => Elements.Count;
        private readonly Vector2 direction;

        public UIStack(float rowHeight, bool isHorizontal = false) {
            this.rowHeight = rowHeight;

            direction = isHorizontal ? new Vector2(1, 0) : new Vector2(0, 1);
        }

        public UIStack AddRows(params UIElement[] element) {
            Elements.AddRange(element);

            Relayout();

            return this;
        }

        public void SetRows(params UIElement[] elements) {
            Elements.Clear();
            Elements.AddRange(elements);

            Relayout();
        }

        public override void Relayout() {
            base.Relayout();

            Vector2 offset = Vector2.Zero;
            foreach (UIElement element in TransparentElements) {
                Vector2 elementSize = element.Size * direction;
                float height = elementSize.X + elementSize.Y;
                height = height > rowHeight ? height : rowHeight;

                element.Position = offset;
                offset += direction * height;
            }
        }
    }
}
