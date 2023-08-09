using Forge.UX.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Forge.UX.UI.Prefabs.Properties {
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

        public T? Value { get; protected set; }
        public T? Default { get; set; } = default(T);

        public virtual bool Parse(string value) {
            try {
                Value = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            } catch (Exception ex) {
                lastError = string.Format(ParseError, ex.Message);
                Value = default(T);
                return false;
            }

            return true;
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
    }
}
