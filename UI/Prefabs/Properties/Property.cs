﻿using Forge.Config;
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
        private static readonly CLogger Logger = DI.Resolve<CLogger>().WithCurrentContext().WithEnumCategory(ForgeLogCategory.UI);

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

        bool valueSet = false;
        T? value;

        public virtual T? Value {
            get => valueSet ? value : Default;
            set {
                valueSet = true;
                this.value = value;
            }
        }

        public bool IsSet {
            get => valueSet;
            protected set => valueSet = value;
        }

        public T? Default { get; set; } = default(T);

        public virtual bool Parse(string value) {
            try {
                Value = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            } catch (Exception ex) {
                lastError = string.Format(ParseError, ex.Message);
                Logger.TraceF(LogLevel.Error, "Failed to parse value - Error: {0}", ex.Message);
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
            return t.Value;
        }

        public static implicit operator Property<T>(T value) {
            return new Property<T> { Value = value };
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
