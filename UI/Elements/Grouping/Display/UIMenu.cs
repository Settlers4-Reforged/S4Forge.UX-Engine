using System.Numerics;

namespace Forge.UX.UI.Elements.Grouping.Display {
    public sealed class UIMenu : UIDisplay<UIMenu> {

        public override (PositioningMode x, PositioningMode y) PositionMode => (PositioningMode.AbsoluteRelative, PositioningMode.AbsoluteRelative);
        public override (PositioningMode width, PositioningMode height) SizeMode => (PositioningMode.AbsoluteRelative, PositioningMode.AbsoluteRelative);

        public override Vector2 Size => new Vector2(1, 1);
        public override Vector2 Position => new Vector2(0, 0);

        public UIMenu() {

        }


        public void OnOpen() {

        }

        public void OnClose() {

        }
    }
}
