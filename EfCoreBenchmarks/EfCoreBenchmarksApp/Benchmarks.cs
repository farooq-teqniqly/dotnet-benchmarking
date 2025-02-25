using BenchmarkDotNet.Attributes;

namespace EfCoreBenchmarksApp
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        public static string ConnectionString { get; set; } = default!;

        [GlobalSetup]
        public async Task Setup()
        {
            using (var context = new StoreContext(ConnectionString))
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
