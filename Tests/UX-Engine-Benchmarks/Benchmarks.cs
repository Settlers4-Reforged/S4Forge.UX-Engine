using BenchmarkDotNet.Attributes;

using Forge.Config;
using Forge.UX.UI;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UX_Engine_Tests;

namespace UX_Engine_Benchmarks {
    public partial class Benchmarks {
        private IoCMock mocks = null!;
        private SceneManager manager = null!;

        public void Setup() {
            Console.WriteLine("Setup");

            mocks = IoCMock.IoCSetup();

            manager = DI.Resolve<SceneManager>();
        }
    }
}
