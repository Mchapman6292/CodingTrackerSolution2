using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IDatabaseManagers
{
    public interface IDatabaseManager
    {
        
        void EnsureDatabaseForUser();
        void CreateTableIfNotExists();
        void OpenConnection();
        void ExecuteCRUD(Action<SQLiteConnection> action);
        bool CheckSessionIdExist(int sessionId);
        Task ExecuteCRUDAsync(Func<SQLiteConnection, Task> asyncAction);
    }
}