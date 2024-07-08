using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
    }
}
