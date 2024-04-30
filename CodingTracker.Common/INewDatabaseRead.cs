using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodingTracker.Common.INewDatabaseReads
{
    public interface INewDatabaseRead
    {

        public List<UserCredentialDTO> ReadFromUserCredentialsTable(List<string> columnsToSelect, int userId = 0, string? username = null, string? passwordHash = null, DateTime? lastLoginDate = null, string? orderBy = null, bool ascending = true, string? groupBy = null, int? limit = null);
        public List<CodingSessionDTO> ReadFromCodingSessionsTable(List<string> columnsToSelect, int sessionId = 0, int userId = 0, DateTime? startDate = null, DateTime? startTime = null, DateTime? endDate = null, DateTime? endTime = null, bool aggregateDurationsByDate = false, string? orderBy = null, bool ascending = true, string? groupBy = null, string? sumColumn = null, int? limit = null);
    }
}
