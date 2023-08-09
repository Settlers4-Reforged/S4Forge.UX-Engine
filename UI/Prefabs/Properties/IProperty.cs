using Forge.UX.Interfaces;

namespace Forge.UX.UI.Prefabs.Properties {
    public interface IProperty : IErrorHandler {
        string Name { get; }
        string Description { get; }

        bool Parse(string value);
    }
}
