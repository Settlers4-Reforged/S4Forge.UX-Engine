using Forge.Logging;
using Forge.UX.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Forge.UX.UI.Prefabs.Properties {
    [DebuggerDisplay("Property {Name} | Value = {Value}")]
    public class Property<T> : IProperty {
        #region Error Handling
        private string? lastError = null;

        private const string ParseError = "Failed to parse value - Error: {0}";

        public string? GetLastError() {
            string? error = lastError;
            lastError = null;
            return error;
        }
        #endregion

        public string Name { get; }
        public string Description { get; }

        public bool Required { get; set; }

        public T? Value { get; protected set; }
        public T? Default { get; set; } = default(T);

        public virtual bool Parse(string value) {
            try {
                Value = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            } catch (Exception ex) {
                lastError = string.Format(ParseError, ex.Message);
                Logger.LogError(null, "Failed to parse value - Error: {0}", ex.Message);
                Value = default(T);
                return false;
            }

            return true;
        }

        public virtual bool Parse(XmlNode node) {
            if (node.Attributes == null) return true;

            XmlAttribute? attribute = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(attr => attr.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase));

            return attribute == null || Parse(attribute!.Value);
        }

        public static implicit operator T?(Property<T> t) {
            return t.Value ?? t.Default;
        }

        protected Property() {

        }

        public Property(string name, string description) {
            Name = name;
            Description = description;
        }

        public Property(string name, string description, T defaultValue) {
            Name = name;
            Description = description;
            Default = defaultValue;
        }

        public Property(string name, string description, Func<T> defaultValue) {
            Name = name;
            Description = description;
            Default = defaultValue();
        }
    }
}
