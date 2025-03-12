using Forge.UX.Input;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Elements.Grouping.Interaction {

    public class UIScrollGroup : UIGroup {
        /// <summary>
        /// The current scroll offset of the scroll area, when content is larger than the max size.
        ///
        /// Relative, where 0 is start and 1 is end of the axis.
        /// </summary>
        public Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Whether to allow for mouse scrolling for the scroll area, or if the scrolling should be initiated by code.
        /// </summary>
        public bool ManualScrolling { get; set; } = false;

        Vector2 childSize = Vector2.Zero;

        public UIScrollGroup() {
            Elements.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) => Dirty();
        }

        public override void Process(SceneGraphState state) {
            base.Process(state);

        }

        public override void Input(ref InputEvent @event) {
            base.Input(ref @event);

            if (!Visible) return;

            //if (!ManualScrolling) return;
            if (@event.Type != InputType.MouseWheel) return;

            ScrollPosition -= @event.Scroll / childSize * 0.5f;
            ScrollPosition = Vector2.Clamp(ScrollPosition, Vector2.Zero, Vector2.One);

            Offset = -ScrollPosition * (childSize - Size);

            Dirty();
        }

        public override void Dirty(bool force = false) {
            base.Dirty(force);
            UpdateChildSize();
        }

        private void UpdateChildSize() {
            childSize = Vector2.Zero;

            foreach (UIElement element in TransparentElements) {
                (PositioningMode x, PositioningMode y) elementPositionMode = element.PositionMode;
                //TODO: find out how to change this!
                if (elementPositionMode.x != PositioningMode.Normal || elementPositionMode.y != PositioningMode.Normal)
                    throw new NotSupportedException("Cannot use relative or absolute positioning in a scroll group");

                childSize = Vector2.Max(childSize, element.Size);
            }
        }
    }
}
