using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.UI {
    public static class VectorHelper {
        public static Vector4 Intersection(this Vector4 newRect, Vector4 intersection) {
            float x1 = Math.Max(intersection.X, newRect.X);
            float x2 = Math.Min(intersection.X + intersection.Z, newRect.X + newRect.Z);
            float y1 = Math.Max(intersection.Y, newRect.Y);
            float y2 = Math.Min(intersection.Y + intersection.W, newRect.Y + newRect.W);
            return new Vector4(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
