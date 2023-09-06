using Forge.UX.Rendering.Texture;

using System.Numerics;

namespace Forge.UX.UI.Components {
    public sealed class NineSliceTextureComponent : TextureComponent {
        //Slices:
        // Corner widths (top-left, top-right, bottom-left, bottom-right)
        public Vector4 CornerWidths { get; set; }

        // Edge widths (top, left, right, bottom)
        public Vector4 EdgeWidths { get; set; }

        public bool RepeatEdges { get; set; } = false;
        public bool RepeatCorners { get; set; } = true;
        public bool RepeatCenter { get; set; } = true;

        public NineSliceTextureComponent(ITexture texture, Vector4 cornerWidths, Vector4 edgeWidths) : base(texture) {
            CornerWidths = cornerWidths;
            EdgeWidths = edgeWidths;
        }
        public NineSliceTextureComponent(ITexture texture, float cornerWidths, float edgeWidths) : base(texture) {
            CornerWidths = Vector4.One * cornerWidths;
            EdgeWidths = Vector4.One * edgeWidths;
        }

        public NineSliceTextureComponent(ITexture texture, float edgeWidths) : base(texture) {
            CornerWidths = Vector4.One * edgeWidths;
            EdgeWidths = Vector4.One * edgeWidths;
        }
    }
}
