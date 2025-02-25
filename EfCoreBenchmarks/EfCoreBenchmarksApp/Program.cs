using System.Reflection;
using BenchmarkDotNet.Running;
using Testcontainers.MsSql;

namespace EfCoreBenchmarksApp;

public class Program
{
    private static async Task ConfigureAndStartContainer(string dbPassword)
    {
        var containerName = GetContainerName();

        var sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword(dbPassword)
            .WithPortBinding(1433, 1433)
            .WithName(containerName)
            .Build();

        await sqlContainer.StartAsync();
    }

    private static string GetContainerName(string prefix = "benchmark-efcore") => $"{prefix}-{Ulid.NewUlid().ToString()[..10]}".ToLowerInvariant();

    private static async Task Main(string[] args)
    {
        await ConfigureAndStartContainer(ConfigurationService.GetDbPassword());

        BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
    }
}