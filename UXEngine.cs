using DryIoc;

using Forge.Config;
using Forge.Engine;
using Forge.Logging;
using Forge.S4.Callbacks;
using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Texture;
using Forge.UX.S4;
using Forge.UX.UI;

using Microsoft.DirectX.DirectDraw;

using System;

namespace Forge.UX {
    public class UXEngine : IEngine, IAfterPluginsLoaded {
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

        private static bool isImplemented = false;
        private static int? latestImplementationPriority = null;

        public static void Implement<TRendererEngine, TTextureCollectionManager>(int implementationPriority) where TRendererEngine : IRenderer where TTextureCollectionManager : ITextureCollectionManager {
            Type rendererEngine = typeof(TRendererEngine);
            Type collectionManager = typeof(TTextureCollectionManager);

            Logger.LogInfo("Requested to add a new render engine implementation for UXEngine: {0} @ {1}", rendererEngine.Name, implementationPriority);

            if (!IsInitialized) {
                Logger.LogWarn("{0} requested to add new implementation, before initialization!", rendererEngine.Name);
                return;
            }

            if (IsReady) {
                Logger.LogWarn("{0} requested to add new implementation, after first render!", rendererEngine.Name);
                return;
            }

            if (isImplemented && implementationPriority < (latestImplementationPriority ?? -1)) {
                Logger.LogWarn("{0} requested to add new implementation, but a higher priority implementation is already in place! New: {1} vs Current: {2}", rendererEngine.Name, implementationPriority, latestImplementationPriority ?? -1);
                return;
            }

            Logger.LogInfo("{0} promoted to new render engine", rendererEngine.Name);

            DI.Dependencies.RegisterMany(new[] { rendererEngine }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
            DI.Dependencies.RegisterMany(new[] { collectionManager }, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);

            isImplemented = true;
            latestImplementationPriority = implementationPriority;
        }

        public void AfterPluginsLoaded() {
            void PrepareOnFirstFrame(Surface? surface, int frameCount) {
                if (DI.Dependencies.IsRegistered<IRenderer>() == false) {
                    //TODO: Add dummy renderer or throw exception
                    Logger.LogWarn("UX Engine is missing a renderer implementation! Disabling all UX-Engine related systems");
                    callbacks.OnFrame -= PrepareOnFirstFrame;
                    return;
                }

                Logger.LogInfo($"UXEngine is ready to render with {DI.Dependencies.Resolve<IRenderer>().Name}");

                IsReady = true;

                SceneManager sceneManager = DI.Resolve<SceneManager>();
                sceneManager.Init();
                DI.Resolve<IInputManager>().Init();


                callbacks.OnTick += (uint tick, bool hasEvent, bool isDelayed) => {
                    sceneManager.DoTick();
                };
                callbacks.OnFrame -= PrepareOnFirstFrame;
            }

            callbacks.OnFrame += PrepareOnFirstFrame;
        }
    }
}
