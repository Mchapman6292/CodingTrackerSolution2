﻿using System;
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
            string sqlCommand,
            int sessionId = 0,
            int userId = 0,
            DateTime? startDate = null,
            DateTime? startTime = null,
            DateTime? endDate = null,
            DateTime? endTime = null,
            double? durationSeconds = 0,
            string? durationHHMM = null,
            string? goalHHMM = null,
            int goalReached = 0,
            string? orderBy = null,
            bool ascending = true,
            string? groupBy = null,
            string? sumColumn = null,
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
               DateTime? endTime = null,
               double? durationSeconds = null,
               string? durationHHMM = null,
               string? goalHHMM = null,
               int goalReached = 0
           );




    }
}