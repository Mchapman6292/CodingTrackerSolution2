using CodingTracker.Common.IStartConfiguration;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;


// Exceptions to check if json file exists
//  Validation methods are called immediately after initilization, needs clear error handling

namespace CodingTracker.Data.Configurations
{
    public class StartConfiguration : IStartConfiguration
    {
        public string DatabasePath { get; private set; }
        public string ConnectionString { get; private set; }

        public StartConfiguration()
        {
            InitializeConfiguration();
            ValidateDatabasePath();
            ValidateConnectionString();
        }

        private void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string relativePath = configuration.GetSection("DatabaseConfig:DatabasePath").Value;  // Construct connection string here
            DatabasePath = Path.GetFullPath(relativePath, AppDomain.CurrentDomain.BaseDirectory);


            ConnectionString = $"Data Source={DatabasePath};Version=3;";
        }

        public void ValidateDatabasePath()
        {
            if (string.IsNullOrEmpty(DatabasePath))
                throw new InvalidOperationException("Database path cannot be null or empty.");
        }

        public void ValidateConnectionString()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException("Connection string cannot be null or empty.");
        }
    }
}
