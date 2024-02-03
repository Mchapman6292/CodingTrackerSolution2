using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICRUDs
{
    public interface ICRUD
    {
        void ViewAllSession(bool partialView = false);


    }
}
