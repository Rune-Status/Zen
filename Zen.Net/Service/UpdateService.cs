using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Zen.Net.Update;

namespace Zen.Net.Service
{
    public class UpdateService
    {
        private readonly BlockingCollection<UpdateSession> _sessions = new BlockingCollection<UpdateSession>();

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public void Start()
        {
            while (true)
                _sessions.Take().ProcessFileQueue();
        }

        public void AddPendingSession(UpdateSession session)
        {
            _sessions.Add(session);
        }
    }
}