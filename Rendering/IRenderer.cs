using Forge.UX.UI;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;

using System.Numerics;

namespace Forge.UX.Rendering {
    public interface IRenderer {
        string Name { get; }

        void RenderUIComponent(IUIComponent component, UIElement parent, SceneGraphState sceneGraphState);

        Vector2 GetScreenSize();
    }
}
