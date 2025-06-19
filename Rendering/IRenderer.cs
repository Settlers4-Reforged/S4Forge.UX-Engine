using Forge.UX.UI;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;

using System;
using System.Numerics;

namespace Forge.UX.Rendering {
    public interface IRenderer {
        string Name { get; }

        void RenderUIComponent(IUIComponent component, UIElement parent, SceneGraphState sceneGraphState);
        void RenderGroup(UIGroup group, SceneGraphState sceneGraphState);

        public event Action? OnUpdateRenderer;

        Vector2 GetScreenSize();
    }
}
