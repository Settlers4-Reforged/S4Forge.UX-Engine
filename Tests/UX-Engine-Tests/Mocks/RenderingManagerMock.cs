using Forge.UX.Rendering;
using Forge.UX.UI;
using Forge.UX.UI.Components;
using Forge.UX.UI.Elements;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UX_Engine_Tests.Mocks {
    internal class RenderingManagerMock : IRenderer {
        public string Name => "Mock Rendering Manager";
        public void RenderUIComponent(IUIComponent component, UIElement parent, SceneGraphState sceneGraphState) {
            throw new NotImplementedException();
        }

        public Vector2 GetScreenSize() {
            throw new NotImplementedException();
        }
    }
}
