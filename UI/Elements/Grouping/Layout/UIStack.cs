using Forge.Logging;

using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Layout {
    public class UIStack : UIGroup, IUILayout {
        private readonly float rowHeight;
        private int Rows => Elements.Count;
        private readonly bool isHorizontal;

        private Vector2 direction => isHorizontal ? new Vector2(1, 0) : new Vector2(0, 1);


        public UIStack(float rowHeight, bool isHorizontal = false) {
            this.rowHeight = rowHeight;
            this.isHorizontal = isHorizontal;
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
                if ((!isHorizontal && element.SizeMode.height != PositioningMode.Normal)
                    ||
                    (isHorizontal && element.SizeMode.width != PositioningMode.Normal)) {
                    Logger.LogWarn("Non normal sizing is not allowed on the stacking axis in a stack. Element: {0}; Sizing: {1}, {2}", element.Id, element.SizeMode.width.ToString(), element.SizeMode.height.ToString());
                }

                Vector2 elementSize = element.Size * direction;
                float height = elementSize.X + elementSize.Y;
                height = height > rowHeight ? height : rowHeight;

                element.Position = offset;
                offset += direction * height;
            }
        }
    }
}
