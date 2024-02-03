using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.IAppConfigs
{
    public interface IAppConfig
    {
        public string DatabasePath { get; }
        public string ConnectionString { get; }


        public void ValidateDatabasePath();
        public void ValidateConnectionString();
    }
}