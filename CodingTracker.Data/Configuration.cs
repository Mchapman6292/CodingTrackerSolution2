using CodingTracker.IAppConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iconfig = CodingTracker.Data.IAppConfigs.IAppConfig;

namespace CodingTracker.Data.Configurations
{

    public class Configuartion : Iconfig
    {
        public string DatabasePath { get; private set; }
        public string ConnectionString { get; private set; }

        public Configuartion()
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