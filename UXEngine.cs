using DryIoc;

using Forge.Config;
using Forge.Engine;
using Forge.Logging;
using Forge.S4.Callbacks;
using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.S4;
using Forge.UX.UI;

using System;

namespace Forge.UX {
    public class UXEngine : IEngine {
        public string Name => "UXEngine";

        private readonly S4Forge forge;
        private readonly ICallbacks callbacks;

        public UXEngine(S4Forge forge, ICallbacks callbacks) {
            this.forge = forge;
            this.callbacks = callbacks;
        }

        public bool Initialize() {
            Logger.LogInfo("Initialized UXEngine");

            RegisterDependencies();

            IsInitialized = true;

            Logger.LogInfo("Requesting implementations from {0} assemblies that were already loaded...", onRequestingImplementation?.GetInvocationList().Length ?? 0);
            onRequestingImplementation?.Invoke();

            unsafe {
                callbacks.OnFrame += (texture, width) => {
                    if (!IsReady) {
                        if (DI.Dependencies.IsRegistered<IRenderer>() == false) {
                            //TODO: Add dummy renderer or throw exception
                            Logger.LogWarn("Missing renderer implementation...");
                            return;
                        }

                        Logger.LogInfo($"UXEngine is ready to render with {DI.Dependencies.Resolve<IRenderer>().Name}");

                        IsReady = true;

                        onReady?.Invoke();
                    }
                };


                onReady += () => {
                    DI.Resolve<SceneManager>().Init();
                    DI.Resolve<IInputManager>().Init();
                };
            }

            return true;
        }

        public void RegisterDependencies() {
            DI.Dependencies.RegisterMany<UIManager>(Reuse.Singleton);
            DI.Dependencies.RegisterInstance<IInputManager>(new InputManager());

            DI.Dependencies.RegisterInstance(new UIEngine());

            DI.Dependencies.RegisterInstance<IRendererConfig>(new RendererConfig());
        }

        public static bool IsReady;
        public static bool IsInitialized;

        internal static Action? onReady;
        static Action? onRequestingImplementation;

        private static bool isImplemented = false;
        private static int? latestImplementationPriority = null;

        public static void Implement(Type rendererEngine, Type collectionManager, int implementationPriority) {
            Logger.LogInfo("Requested to add a new render engine implementation for UXEngine: {0} @ {1}", rendererEngine.Name, implementationPriority);

            if (IsInitialized) {
                InternalImplementation();
            } else {
                onRequestingImplementation += InternalImplementation;
            }

            return;

            void InternalImplementation() {
                if (IsReady) {
                    Logger.LogWarn("{0} requested to add new implementation, after first render!", rendererEngine.Name);
                    return;
                }

                if (isImplemented && implementationPriority < (latestImplementationPriority ?? -1)) {
                    Logger.LogWarn("{0} requested to add new implementation, but a higher priority implementation is already in place! New: {1} vs Current: {2}", rendererEngine.Name, implementationPriority, latestImplementationPriority ?? -1);
                    return;
                }

                Logger.LogInfo("{0} promoted to new render engine", rendererEngine.Name);

                if (rendererEngine != null)
                    DI.Dependencies.RegisterMany(new[] { rendererEngine }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);

                if (collectionManager != null)
                    DI.Dependencies.RegisterMany(new[] { collectionManager }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);

                isImplemented = true;
                latestImplementationPriority = implementationPriority;
            }
        }
    }
}
