using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace EfCoreBenchmarksApp
{
    public static class ConfigurationService
    {
        private static readonly IConfigurationRoot _config;

        static ConfigurationService()
        {
            _config = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
        }

        public static string GetDbPassword()
        {
            var dbPassword = _config["DatabaseSettings:Password"] ??
                             throw new InvalidOperationException("Database password not set.");

            return dbPassword;
        }
    }
}