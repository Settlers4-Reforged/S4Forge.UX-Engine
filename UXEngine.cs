using DryIoc;

using Forge.Config;
using Forge.Engine;
using Forge.Logging;
using Forge.Native;
using Forge.S4;
using Forge.S4.Callbacks;
using Forge.UPlay;
using Forge.UX.Input;
using Forge.UX.Rendering;
using Forge.UX.Rendering.Texture;
using Forge.UX.S4;
using Forge.UX.UI;

using Microsoft.DirectX.DirectDraw;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Forge.UX {
    public class UXEngine : IEngine {
        public string Name => "UXEngine";

        private S4Forge forge;

        public UXEngine(S4Forge forge) {
            this.forge = forge;
        }

        public bool Initialize() {
            Logger.LogInfo("Initialized UXEngine");

            RegisterDependencies();
            new UIEngine();

            IsInitialized = true;

            Logger.LogInfo("Requesting implementations from {0} assemblies that were already loaded...", onRequestingImplementation?.GetInvocationList().Length ?? 0);
            onRequestingImplementation?.Invoke();

            unsafe {
                Callbacks.OnFrame += (texture, width) => {
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

                    if (texture == null) return;

                    try {
                        texture.FillStyle = 6/*Cross*/;
                        texture.FillColor = Color.Blue;
                        texture.DrawBox(0, 0, 100, 100);
                    } catch (Exception e) {
                        Logger.LogWarn("Found error {0}", e);
                    }

                    var test = GameValues.GetAllUIElementsFromIndexUnsafe(7);
                };

                Callbacks.OnMouse += (button, x, y, id, element) => {
                    Logger.LogDebug("Mouse button {0} @ {1}, {2} on {3} ({4})", button, x, y, id, element);
                };

                onReady += () => {
                    DI.Dependencies.Resolve<SceneManager>().Init();
                };
            }

            return true;
        }

        public void RegisterDependencies() {

        }

        public static bool IsReady;
        public static bool IsInitialized;

        private static Action? onReady;
        static Action? onRequestingImplementation;

        private static bool isImplemented = false;
        private static int? latestImplementationPriority = null;

        public static void Implement(Type rendererEngine, Type collectionManager, int implementationPriority) {
            Logger.LogInfo("Requested to add a new render engine implementation for UXEngine: {0} @ {0}", rendererEngine.Name, implementationPriority);

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

                if (isImplemented && implementationPriority < (latestImplementationPriority ?? -1))
                    return;

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
