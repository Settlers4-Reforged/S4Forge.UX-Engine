using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UX_Engine_Tests {
    public static class Util {
        public static void AssemblySetup() {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                string assemblyName = new AssemblyName(args.Name).Name;
                return assemblyName switch {
                    "S4Forge" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4Forge.dll"),
                    "S4ForgeBootstrapper" => Assembly.LoadFile(Environment.CurrentDirectory + "\\S4ForgeBootstrapper.asi"),
                    _ => null
                };
            };
        }
    }
}
