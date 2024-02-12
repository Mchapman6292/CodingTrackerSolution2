using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IStartConfiguration
{
    public interface IStartConfiguration
    {
        public string ConnectionString { get; }
        public string DatabasePath { get; }
    }
}