using Forge.UX.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI.Elements.Static {
    public sealed class UITexture : UIElement {
        public override Vector2 Size => new(texture.Texture.Width, texture.Texture.Height);

        private readonly TextureComponent texture;

        public UITexture(TextureComponent texture) {
            this.texture = texture;
            Components = new List<IUIComponent> { texture };
        }
    }
}
