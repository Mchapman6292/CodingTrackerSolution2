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
        List<CodingSessionDTO> ReadFromCodingSessionsTable(IQueryBuilder query);

        public List<UserCredentialDTO> ReadFromUserCredentialsTable();

        UserCredentialDTO ExtractUserCredentialFromReader(SQLiteDataReader reader);

    }
}
