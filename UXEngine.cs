using Forge.Engine;
using Forge.Logging;
using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Texture;

using System;

namespace Forge.UX {
    public class UXEngine : IEngine {
        public string Name => "UXEngine";

        public bool Initialize(S4Forge forge) {
            Logger.LogInfo("Initialized UXEngine");

            IsReady = true;

            Logger.LogInfo("Requesting implementations from {0} assemblies that were already loaded...", OnRequestingImplementation?.GetInvocationList().Length ?? 0);

            OnRequestingImplementation?.Invoke();

            return true;
        }

        public static bool IsReady;

        public static Action? OnRequestingImplementation;

        private static bool isImplemented = false;
        private static int? latestImplementationPriority = null;
        private static IRenderer? renderer;
        private static ITextureCollectionManager? textureCollectionManager;

        internal static IRenderer R => renderer ?? throw new InvalidOperationException();

        /// <summary>
        /// Global TextureCollectionManager implementation, provided by the implementing renderer - should only be used after 
        /// </summary>
        public static ITextureCollectionManager TCM => textureCollectionManager ?? throw new InvalidOperationException();

        public static void Implement(IRenderer rendererEngine, ITextureCollectionManager collectionManager, int implementationPriority) {
            Logger.LogInfo("Requested to add a new render engine implementation for UXEngine: {0} @ {0}", rendererEngine.Name, implementationPriority);

            if (IsReady) {
                Logger.LogWarn("{0} requested to add new implementation, after first render!", rendererEngine.Name);
                return;
            }

            if (isImplemented && implementationPriority < (latestImplementationPriority ?? -1))
                return;

            Logger.LogInfo("{0} promoted to new render engine", rendererEngine.Name);

            UXEngine.renderer = rendererEngine;
            UXEngine.textureCollectionManager = collectionManager;

            isImplemented = true;
            latestImplementationPriority = implementationPriority;
        }
    }
}
