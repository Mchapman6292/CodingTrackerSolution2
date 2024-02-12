using CodingTracker.Common.IStartConfiguration;
using Microsoft.Extensions.Configuration;
using System;

namespace CodingTracker.Data.Configurations
{
    public class StartConfiguration : IStartConfiguration
    {
        public string ConnectionString { get; private set; }
        public string DatabasePath { get; private set; }

        public StartConfiguration(IConfiguration configuration)
        {
            try
            {
                ConnectionString = configuration.GetSection("DatabaseConfig:ConnectionString").Value;
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new InvalidOperationException("Connection string configuration is missing.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error loading configuration: " + ex.Message, ex);
            }
        }
    }
}