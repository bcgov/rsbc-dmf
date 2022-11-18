using Hangfire.Server;
using Hangfire;
using System.Threading.Tasks;

namespace Rsbc.Dmf.Scheduler
{
    public class ScheduledJobs
    {
        // add the Hangfire tasks for each job here.

        /// <summary>
        /// Hangfire job example
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task ExampleJob(PerformContext hangfireContext)
        {
        }
    }
}
