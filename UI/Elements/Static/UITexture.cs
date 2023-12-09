using Forge.UX.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UITexture : UIElement {
        public override Vector2 Size => new Vector2(texture.Texture?.Width ?? 0, texture.Texture?.Height ?? 0);

        private readonly TextureComponent texture;

        public UITexture(TextureComponent texture) {
            this.texture = texture;
            Components = new List<IUIComponent> { texture };
        }
    }
}
