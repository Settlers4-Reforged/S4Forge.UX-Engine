using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.UX.Rendering {
    public interface IRendererConfig {
        T? GetConfig<T>(string key, T? @default = default);
        void SetConfig<T>(string key, T value);
    }

    public class RendererConfig : IRendererConfig {
        private readonly Dictionary<string, object> config = new Dictionary<string, object>();

        public T? GetConfig<T>(string key, T? @default = default) {
            if (!config.TryGetValue(key, out object? value)) return @default;
            return (T)value;

        }

        public void SetConfig<T>(string key, T? value) {
            config[key] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
