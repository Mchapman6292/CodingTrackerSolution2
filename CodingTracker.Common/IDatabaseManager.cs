﻿using System;
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
        void ExecuteDatabaseOperation(Action<SQLiteConnection> operation, string operationName);
        void CreateTableIfNotExists();
        List<string> GetTableColumns(string tableName);
        void OpenConnectionWithRetry();
        void CloseDatabaseConnection();
        void ExecuteCRUD(Action<SQLiteConnection> action);
        bool CheckSessionIdExist(int sessionId);
        void UpdateUserCredentialsTable();
        void UpdateCodingSessionsTable();
    }
}