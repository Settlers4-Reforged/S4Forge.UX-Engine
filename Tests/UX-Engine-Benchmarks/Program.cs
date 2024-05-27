using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace UX_Engine_Benchmarks {
    internal class Program {
        static void Main(string[] args) {
            var summary = BenchmarkRunner.Run<Benchmarks>(config: new DebugInProcessConfig());
        }
    }
}
