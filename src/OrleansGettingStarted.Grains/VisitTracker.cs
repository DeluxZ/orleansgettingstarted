using Orleans;
using Orleans.Providers;
using OrleansGettingStarted.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansGettingStarted.Grains
{
    [StorageProvider(ProviderName = Constants.OrleansMemoryProvider)]
    public class VisitTracker : Grain<VisitTrackerState>, IVisitTracker
    {
        public Task<int> GetNumberOfVisits()
        {
            return Task.FromResult(State.NumberOfVisits);
        }

        public async Task Visit()
        {
            var now = DateTimeOffset.UtcNow;

            if (!State.FirstVisit.HasValue)
            {
                State.FirstVisit = now;
            }

            State.NumberOfVisits++;
            State.LastVisit = now;

            await WriteStateAsync();
        }
    }

    public class VisitTrackerState
    {
        public DateTimeOffset? FirstVisit { get; set; }
        public DateTimeOffset? LastVisit { get; set; }
        public int NumberOfVisits { get; set; }
    }
}