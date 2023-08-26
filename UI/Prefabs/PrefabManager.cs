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

        private readonly List<IPrefab> prefabs = new List<IPrefab>();

        public bool RegisterPrefab(IPrefab prefab) {
            char[] forbiddenChars = "<>,.;:|!§$%&/()=? *+#-´`\"'^°".ToCharArray();
            if (forbiddenChars.Any(c => prefab.Name.Contains(c))) {
                lastError = InvalidNameError;
                return false;
            }

            if (prefab.Name.Trim() == "") {
                lastError = EmptyNameError;
                return false;
            }

            if (prefabs.Find(i => i.Name.Equals(prefab.Name, StringComparison.OrdinalIgnoreCase)) != null) {
                lastError = PrefabAlreadyExistsError;
                return false;
            }

            prefabs.Add(prefab);
            return true;
        }

        public void RegisterDefaultPrefabs() {

            Logger.LogInfo($"Registering default prefabs from UX-Engine...");

            Assembly prefabAssembly = Assembly.GetAssembly(typeof(IPrefab));


            Type[] types = prefabAssembly.GetExportedTypes();

            Type[] prefabList = (from Type t in types
                                 where t.GetInterface(nameof(IPrefab)) != null && t.IsPublic && !t.IsAbstract
                                 select t).ToArray();

            foreach (Type prefab in prefabList) {
                IPrefab prefabInstance = (IPrefab)Activator.CreateInstance(prefab);
                bool success = RegisterPrefab(prefabInstance);
                if (!success) {
                    throw new ArgumentException($"Failed to register prefab \"{prefabInstance.Name}\" - Error: \"{GetLastError()}\"");
                }
            }

            Logger.LogInfo($"Registered {prefabList.Length} default prefabs from UX-Engine");
        }

        public IPrefab? GetPrefabByName(string name) {
            return prefabs.Find(prefab => prefab.Name == name)?.Clone();
        }

    }
}
