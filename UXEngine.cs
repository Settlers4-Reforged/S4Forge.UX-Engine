using DryIoc;

using Forge.Config;
using Forge.Engine;
using Forge.Game.Core;
using Forge.Logging;
using Forge.Native.DirectX;
using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Texture;
using Forge.UX.S4;
using Forge.UX.UI;

using System;

namespace Forge.UX {
    public class UXEngine : IEngine, IAfterModulesLoaded {
        public string Name => "UXEngine";

        private readonly CLogger Logger;
        private readonly S4Forge forge;
        private readonly ICallbacks callbacks;

        public UXEngine(S4Forge forge, ICallbacks callbacks, CLogger logger) {
            this.forge = forge;
            this.callbacks = callbacks;
            Logger = logger.WithEnumCategory(ForgeLogCategory.Engine);
        }

        public bool Initialize() {
            Logger.Log(LogLevel.Info, "Initialized UXEngine");
            RegisterDependencies();

            IsInitialized = true;

            return true;
        }

        public void RegisterDependencies() {
            DI.Dependencies.RegisterMany<UIManager>(Reuse.Singleton);
            DI.Dependencies.RegisterMany<InputManager>(Reuse.Singleton);

            DI.Dependencies.RegisterInstance(new UIEngine());

            DI.Dependencies.RegisterInstance<IRendererConfig>(new RendererConfig());
        }

        public static bool IsReady;
        public static bool IsInitialized;

        private static bool isImplemented = false;
        private static int? latestImplementationPriority = null;

        public void Implement<TRendererEngine, TTextureCollectionManager>(int implementationPriority) where TRendererEngine : IRenderer where TTextureCollectionManager : ITextureCollectionManager {
            Type rendererEngine = typeof(TRendererEngine);
            Type collectionManager = typeof(TTextureCollectionManager);

            Logger.LogF(LogLevel.Debug, "Requested to add a new render engine implementation for UXEngine: {0} @ {1}", rendererEngine.Name, implementationPriority);

            if (!IsInitialized) {
                Logger.LogF(LogLevel.Warning, "{0} requested to add new implementation, before initialization!", rendererEngine.Name);
                return;
            }

            if (IsReady) {
                Logger.LogF(LogLevel.Warning, "{0} requested to add new implementation, after first render!", rendererEngine.Name);
                return;
            }

            if (isImplemented && implementationPriority < (latestImplementationPriority ?? -1)) {
                Logger.LogF(LogLevel.Warning, "{0} requested to add new implementation, but a higher priority implementation is already in place! New: {1} vs Current: {2}", rendererEngine.Name, implementationPriority, latestImplementationPriority ?? -1);
                return;
            }

            Logger.LogF(LogLevel.Info, "{0} promoted to new render engine", rendererEngine.Name);

            DI.Dependencies.RegisterMany(new[] { rendererEngine }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
            DI.Dependencies.RegisterMany(new[] { collectionManager }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);

            isImplemented = true;
            latestImplementationPriority = implementationPriority;
        }

        public unsafe void AfterModulesLoaded() {
            void PrepareOnFirstFrame(IDirectDrawSurface7* surface, int frameCount) {
                DI.Resolve<IInputManager>().Init();

                // Custom UI related systems init begins here:
                if (DI.Dependencies.IsRegistered<IRenderer>() == false) {
                    //TODO: Add dummy renderer or throw exception
                    Logger.Log(LogLevel.Warning, "UX Engine is missing a renderer implementation! Disabling all custom UI related systems");
                    callbacks.OnFrame -= PrepareOnFirstFrame;
                    return;
                }

                IRendererConfig config = DI.Resolve<IRendererConfig>();
                unsafe {
                    //config.SetConfig<IntPtr>("forge.d3d9.direct3d", (IntPtr)surface.Value.Direct3D);
                    //config.SetConfig<IntPtr>("forge.d3d9.device", (IntPtr)surface.Device);
                    config.SetConfig<IntPtr>("forge.d3d9.surface", (IntPtr)surface);
                }

                Logger.LogF(LogLevel.Info, "UXEngine is ready to render with {0}", DI.Dependencies.Resolve<IRenderer>().Name);

                DI.Resolve<ITextureCollectionManager>().RegisterDefaults();

                IsReady = true;

                SceneManager sceneManager = DI.Resolve<SceneManager>();
                sceneManager.Init();


                callbacks.OnTick += (uint tick, bool hasEvent, bool isDelayed) => {
                    sceneManager.DoTick();
                };
                callbacks.OnFrame -= PrepareOnFirstFrame;
            }

            callbacks.OnFrame += PrepareOnFirstFrame;
        }
    }
}
