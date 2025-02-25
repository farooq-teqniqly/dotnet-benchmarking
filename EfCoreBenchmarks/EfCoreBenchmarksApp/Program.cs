using System.Reflection;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

var config = new ConfigurationBuilder()
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

await ConfigureAndStartContainer(config);
Console.WriteLine("Hello, World!");
return;

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
