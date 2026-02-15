using Connectivity_Tracker.Models;
using Connectivity_Tracker.Services;

namespace Connectivity_Tracker.Controllers
{
    public class HistoryController
    {
        private readonly DatabaseRepository _databaseRepository;

        public HistoryController(DatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public List<NetworkMetrics> GetMetrics(DateTime startDateTime, DateTime endDateTime, int limit = 5000)
        {
            if (endDateTime < startDateTime)
            {
                return new List<NetworkMetrics>();
            }

            return _databaseRepository.GetMetrics(startDateTime, endDateTime, limit);
        }
    }
}
