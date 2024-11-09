using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Elements.Grouping.Interaction {
    public class UIScrollGroup : UIGroup {
        /// <summary>
        /// The maximum size of the scroll area. Anything outside of this area will be clipped and be scrollable to.
        /// </summary>
        public Vector2 MaxSize { get; set; }

        /// <summary>
        /// The current scroll offset of the scroll area, when content is larger than the max size.
        /// </summary>
        public Vector2 ScrollPosition { get; protected set; }

        /// <summary>
        /// Whether to provide a scrollbar for the scroll area, or if the scrolling should be initiated by code.
        /// </summary>
        /// <remarks>
        /// This also toggles the ability to scroll with the mouse scroll wheel.
        /// </remarks>
        public bool ManualScrolling { get; set; } = false;

        public override void Process(SceneGraphState state) {
            base.Process(state);

            //TODO: Implement scrolling
        }
    }
}
