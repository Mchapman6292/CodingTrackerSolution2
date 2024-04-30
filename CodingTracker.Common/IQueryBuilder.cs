using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IQueryBuilders
{
    public interface IQueryBuilder
    {
        string CreateCommandTextForUserCredentials
        (
            List<string> columnsToSelect,
            int userId = 0,
            string? username = null,
            string? passwordHash = null,
            DateTime? lastLoginDate = null,
            string? orderBy = null,
            bool ascending = true,
            string? groupBy = null,
            int? limit = null
        );
        void SetCommandParametersForUserCredentials
         (
           SQLiteCommand command,
           int userId = 0,
           string? username = null,
           string? passwordHash = null,
           DateTime? lastLoginDate = null
         );

        public string CreateCommandTextForCodingSessions
             (
                 List<string> columnsToSelect,
                 int sessionId = 0,
                 int userId = 0,
                 DateTime? startDate = null,
                 DateTime? startTime = null,
                 DateTime? endDate = null,
                 DateTime? endTime = null,
                 string? orderBy = null,
                 bool ascending = true,
                 string? groupBy = null,
                 string? sumColumn = null, // Parameter to specify which column to sum
                 int? limit = null
             );

        void SetCommandParametersForCodingSessions
        (
            SQLiteCommand command,
            int sessionId = 0,
            int userId = 0,
            DateTime? startDate = null,
            DateTime? startTime = null,
            DateTime? endDate = null,
            DateTime? endTime = null
        );




    }
}