using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EfCoreBenchmarksApp
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private readonly IServiceProvider _serviceProvider;

        public Benchmarks(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [GlobalSetup]
        public async Task Setup()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                await context.SeedData(1, 10);
            }
        }

        [Benchmark]
        public void TestBenchmark()
        {
        }
    }
}
