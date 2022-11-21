using Hangfire.Server;
using Hangfire;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using Hangfire.Console;
using Serilog;
using System.IO;
using System.Text;
using Rsbc.Dmf.IcbcAdapter;
using FileHelpers;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Rsbc.Dmf.Scheduler
{
    public class ScheduledJobs
    {
        private IConfiguration _configuration { get; }
        private readonly ScheduledJobs _scheduledJobs;
        private readonly IcbcAdapterClient _icbcAdapterClient;

        public ScheduledJobs(IConfiguration configuration, ScheduledJobs schedulerJobClient, IcbcAdapterClient icbcAdapterClient)
        {
            _configuration = configuration;
            _scheduledJobs = schedulerJobClient;
            _icbcAdapterClient = icbcAdapterClient;
        }

    
        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForCandidates(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting check for Candidates.");

            // Call ICBC Adapter to do the check for candidates
            _icbcAdapterClient.ProcessMedicalStatusUpdates(new EmptyRequest());


            LogStatement(hangfireContext, "End of check for Candidates.");
        }


        private void LogStatement(PerformContext hangfireContext, string message)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine(message);
            }
            // emit to Serilog.
            Log.Logger.Information(message);
        }
    }
}
