using System.Reflection;
using BenchmarkDotNet.Running;
using EfCoreBenchmarksApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

var connectionStringTemplate = "Server=localhost,1433;Database=benchmark-efcore;User Id=sa;Password={0};";
var host = BuildHost();
await ConfigureAndStartContainer(host.Services.GetRequiredService<IConfigurationRoot>());
// await host.RunAsync();
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);

return;

IHost BuildHost() =>
    Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        services.AddSingleton(config);

        var dbPassword = config["DatabaseSettings:Password"] ??
                         throw new InvalidOperationException("Database password not set.");

        var connectionString = string.Format(connectionStringTemplate, dbPassword);

        services.AddDbContext<StoreContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
    }).Build();

string GetContainerName(string prefix = "benchmark-efcore") => $"{prefix}-{Ulid.NewUlid().ToString()[..10]}".ToLowerInvariant();

async Task ConfigureAndStartContainer(IConfigurationRoot config)
{
    var dbPassword = config["DatabaseSettings:Password"] ?? throw new InvalidOperationException("Database password not set.");
    var containerName = GetContainerName();

    var sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword(dbPassword)
        .WithPortBinding(1433, 1433)
        .WithName(containerName)
        .Build();

    await sqlContainer.StartAsync();
}
