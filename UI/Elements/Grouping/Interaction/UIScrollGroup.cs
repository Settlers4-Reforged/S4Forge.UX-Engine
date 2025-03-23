using Forge.UX.Input;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        Vector2 calculatedSize = Vector2.Zero;

        public UIScrollGroup() {
            Elements.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) => Dirty();
        }

        public override void Process(SceneGraphState state) {
            base.Process(state);

            calculatedSize = state.TranslateElement(this).size;
        }

        public override void Input(ref InputEvent @event) {
            base.Input(ref @event);

            if (!Visible) return;

            //if (!ManualScrolling) return;
            if (@event.Type != InputType.MouseWheel) return;

            ScrollPosition -= @event.Scroll / childSize * 0.5f;
            ScrollPosition = Vector2.Clamp(ScrollPosition, Vector2.Zero, Vector2.One);

            Offset = Vector2.Min(-ScrollPosition * (childSize - calculatedSize), Vector2.Zero);
        }

        public override void InvalidateLayout() {
            base.InvalidateLayout();
            UpdateChildSize();
        }

        private void UpdateChildSize() {
            childSize = Vector2.Zero;

            this.TraverseScene(null, (element, state) => {
                Vector2 size = state.TranslateElement(element).size;
                childSize = Vector2.Max(childSize, size);
            });
        }
    }
}
