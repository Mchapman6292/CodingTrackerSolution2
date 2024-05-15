namespace CodingTracker.Common.CurrentCodingSessions
{
    public class CurrentCodingsession
    {
        private int _currentSessionId;
        private int _currentUserId;
        private DateOnly _currentStartDate;
        private DateTime _currentStartTime;
        private DateOnly _currentEndDate;
        private DateTime _currentEndTime;
        private double _currentDurationSeconds;
        private string? _currentDurationHHMM;
        private string? _currentGoalHHMM;
        private int _currentGoalReached;

        public int SessionId
        {
            get { return _currentSessionId; }
            set { _currentSessionId = value; }
        }

        public int UserId
        {
            get { return _currentUserId; }
            set { _currentUserId = value; }
        }

        public DateOnly StartDate
        {
            get { return _currentStartDate; }
            set { _currentStartDate = value; }
        }

        public DateTime StartTime
        {
            get { return _currentStartTime; }
            set { _currentStartTime = value; }
        }

        public DateOnly EndDate
        {
            get { return _currentEndDate; }
            set { _currentEndDate = value; }
        }

        public DateTime EndTime
        {
            get { return _currentEndTime; }
            set { _currentEndTime = value; }
        }

        public double DurationSeconds
        {
            get { return _currentDurationSeconds; }
            set { _currentDurationSeconds = value; }
        }

        public string DurationHHMM
        {
            get { return _currentDurationHHMM; }
            set { _currentDurationHHMM = value; }
        }

        public string GoalHHMM
        {
            get { return _currentGoalHHMM; }
            set { _currentGoalHHMM = value; }
        }

        public int GoalReached
        {
            get { return _currentGoalReached; }
            set { _currentGoalReached = value; }
        }
    }
}