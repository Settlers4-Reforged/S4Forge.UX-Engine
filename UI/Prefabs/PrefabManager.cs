using Forge.Logging;
using Forge.UX.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Forge.UX.UI.Prefabs {
    public class PrefabManager : IErrorHandler {
        #region Error Handling
        private string? lastError = null;

        private const string InvalidNameError = "Tried to register a prefab with a forbidden character";
        private const string EmptyNameError = "Tried to register a prefab with an empty name";
        private const string PrefabAlreadyExistsError = "Tried to register a prefab with a name that is already taken";

        public string? GetLastError() {
            string? error = lastError;
            lastError = null;
            return error;
        }
        #endregion

        private readonly Dictionary<string, IPrefab> prefabs = new Dictionary<string, IPrefab>();

        public bool RegisterPrefab(IPrefab prefab, string tagName) {
            char[] forbiddenChars = "<>,.;:|!§$%&/()=? *+#-´`\"'^°".ToCharArray();
            if (forbiddenChars.Any(c => prefab.Name.Contains(c))) {
                lastError = InvalidNameError;
                return false;
            }

            if (prefab.Name.Trim() == "") {
                lastError = EmptyNameError;
                return false;
            }

            if (prefabs.Keys.Contains(prefab.Name)) {
                lastError = PrefabAlreadyExistsError;
                return false;
            }

            prefabs.Add(tagName, prefab);
            return true;
        }

        public void RegisterDefaultPrefabs() {

            Logger.LogInfo($"Registering default prefabs from UX-Engine...");

            Assembly prefabAssembly = Assembly.GetAssembly(typeof(IPrefab))!;


            Type[] types = prefabAssembly.GetExportedTypes();

            Type[] prefabList = (from Type t in types
                                 where t.GetInterface(nameof(IPrefab)) != null && t.IsPublic && !t.IsAbstract
                                 select t).ToArray();

            foreach (Type prefab in prefabList) {
                IPrefab? prefabInstance;
                try {
                    prefabInstance = (IPrefab?)Activator.CreateInstance(prefab);

                    if (prefabInstance == null) {
                        Logger.LogError(null, $"Failed to create instance of prefab \"{prefab.Name}\"");
                        continue;
                    }
                } catch (Exception e) {
                    Logger.LogError(e, $"Failed to create instance of prefab \"{prefab.Name}\"");
                    continue;
                }

                PrefabAttribute? prefabConfig = prefab.GetCustomAttributes<PrefabAttribute>(false).FirstOrDefault();
                if (prefabConfig == null)
                    continue;

                bool success = RegisterPrefab(prefabInstance, prefabConfig.TagName);
                if (!success) {
                    throw new ArgumentException($"Failed to register prefab \"{prefabInstance.Name}\" - Error: \"{GetLastError()}\"");
                }
            }

            Logger.LogInfo($"Registered {prefabList.Length} default prefabs from UX-Engine");
        }

        /// <summary>
        /// Returns a new instance of a prefab by its tag name
        /// </summary>
        /// <param name="name">The name of the tag, e.g. "s4-text"</param>
        public IPrefab? GetPrefabByName(string name) {
            prefabs.TryGetValue(name, out IPrefab? foundPrefab);
            return foundPrefab?.Clone();
        }

    }
}
