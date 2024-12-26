using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Business.CodingSessionService.EditSessionPageContextManagers
{
    public class EditSessionPageContextManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly HashSet<int> _sessionIdsForDeletion;


        public EditSessionPageContextManager(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
            _sessionIdsForDeletion = new HashSet<int>();
        }

        public void AddSessionIdForDeletion(int sessionId)
        {
            _sessionIdsForDeletion.Add(sessionId);
        }

        public void RemoveSessionIdForDeletion(int sessionId)
        {
            _sessionIdsForDeletion.Remove(sessionId);
        }

        public void ClearSessionIdsForDeletion()
        {
            _sessionIdsForDeletion.Clear();
        }

        public bool CheckForSessionId(int sessionId)
        {
            return _sessionIdsForDeletion.Contains(sessionId);
        }

        public IReadOnlyCollection<int> GetSessionIdsForDeletion()
        {
            return _sessionIdsForDeletion.ToList().AsReadOnly();
        }




    }
}
