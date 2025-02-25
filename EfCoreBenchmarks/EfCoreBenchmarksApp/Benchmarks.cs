using BenchmarkDotNet.Attributes;

namespace EfCoreBenchmarksApp
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        [GlobalSetup]
        public async Task Setup()
        {
            using (var context = new StoreContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                await context.SeedData(1, 10);
            }
        }

        [Benchmark]
        public void TestBenchmark()
        {
            var x = 1;
        }
    }
}