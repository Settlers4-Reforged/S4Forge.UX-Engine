using Forge.UX.Interfaces;

using System.Xml;

namespace Forge.UX.UI.Prefabs.Properties {
    public interface IProperty : IErrorHandler {
        string Name { get; }
        string Description { get; }

        bool Required { get; }

        bool Parse(string value);

        bool Parse(XmlNode node);
    }
}
