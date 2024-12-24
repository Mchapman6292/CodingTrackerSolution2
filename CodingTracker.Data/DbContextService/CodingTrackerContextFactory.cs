using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CodingTracker.Data.DbContextService.CodingTrackerDbContexts;

namespace CodingTracker.Data.DbContextService.CodingTrackerContextFactory
{
    public class CodingTrackerContextFactory : IDesignTimeDbContextFactory<CodingTrackerDbContext> //
    {
        public CodingTrackerDbContext CreateDbContext(string[] args)
        {
            var appSettingsPath = @"C:\Users\mchap\source\repos\CodingTrackerSolutionClone2\CodingTracker.View";
            var appSettingsFile = Path.Combine(appSettingsPath, "AppSettings.json");

            if (!File.Exists(appSettingsFile))
            {
                throw new FileNotFoundException($"Configuration file not found: {appSettingsFile}");
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath)
                .AddJsonFile("AppSettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<CodingTrackerDbContext>();
            var connectionString = configuration.GetSection("DatabaseConfig:ConnectionString").Value;

            builder.UseNpgsql(connectionString);

            return new CodingTrackerDbContext(builder.Options);
        }
    }
}