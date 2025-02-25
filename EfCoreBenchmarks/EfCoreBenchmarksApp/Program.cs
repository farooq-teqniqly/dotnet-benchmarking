using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
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
        // var connectionStringTemplate = "Server=localhost,1433;Database=benchmark-efcore;User Id=sa;Password={0};";

        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        var dbPassword = config["DatabaseSettings:Password"] ??
                         throw new InvalidOperationException("Database password not set.");

        // Benchmarks.ConnectionString = string.Format(connectionStringTemplate, dbPassword);

        //Console.WriteLine($"Connection string: {Benchmarks.ConnectionString}");

        await ConfigureAndStartContainer(dbPassword);

        var summary = BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
        // BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args: null, new DebugInProcessConfig());
    }
}
