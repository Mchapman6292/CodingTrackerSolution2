using CodingTracker.Common.IStartConfiguration;

namespace CodingTracker.Data.Configurations
{

    public class StartConfiguration : IStartConfiguration
    {
        public string DatabasePath { get; private set; }
        public string ConnectionString { get; private set; }

        public StartConfiguration()
        {
            ValidateDatabasePath();
            ValidateConnectionString();
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