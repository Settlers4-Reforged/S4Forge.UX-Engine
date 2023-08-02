using Forge.Engine;
using Forge.UX.Input;
using Forge.UX.Rendering;

using NetModAPI;

using System;

namespace Forge.UX {
    public class UXEngine : IEngine {
        public string Name => "UXEngine";

        public bool Initialize(S4Forge forge) {
            Logger.LogInfo("Initialized UXEngine");

            IsInitialized = true;

            Logger.LogInfo($"Requesting implementations from {OnRequestingImplementation?.GetInvocationList().Length ?? 0} assemblies that were already loaded...");

            OnRequestingImplementation?.Invoke();

            return true;
        }

        public static bool IsInitialized;

        public static Action? OnRequestingImplementation;

        private static bool isImplemented = false;
        private static int? latestImplementationPriority = null;
        private static IRenderer? renderer;

        internal static IRenderer R => renderer ?? throw new InvalidOperationException();

        public static void Implement(IRenderer rendererEngine, int implementationPriority) {
            Logger.LogInfo($"Requested to add a new render engine implementation for UXEngine: {rendererEngine.Name} @ {implementationPriority}");

            if (isImplemented && implementationPriority < (latestImplementationPriority ?? -1))
                return;

            Logger.LogInfo($"{rendererEngine.Name} promoted to new render engine");

            UXEngine.renderer = rendererEngine;

            isImplemented = true;
            latestImplementationPriority = implementationPriority;
        }
    }
}
