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
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.Scheduler
{
    public class ScheduledJobs
    {
        private IConfiguration _configuration { get; }
        private readonly ScheduledJobs _scheduledJobs;
        private readonly IcbcAdapterClient _icbcAdapterClient;
        private readonly CaseManagerClient _caseManagerClient;



        public ScheduledJobs(IConfiguration configuration, ScheduledJobs schedulerJobClient, IcbcAdapterClient icbcAdapterClient, CaseManagerClient caseManagerClient)
        {
            _configuration = configuration;
            _scheduledJobs = schedulerJobClient;
            _icbcAdapterClient = icbcAdapterClient;
            _caseManagerClient = caseManagerClient;
        }

    
        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendMedicalUpdates(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting check for Candidates.");

            // Call ICBC Adapter to do the check for candidates
            _icbcAdapterClient.ProcessMedicalStatusUpdates(new IcbcAdapter.EmptyRequest());


            LogStatement(hangfireContext, "End of check for Candidates.");
        }

        /// <summary>
        /// Hangfire job resolve the case status
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task ResolveCaseStatus(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting to check the case status");

            // Call ICBC Adapter to do the check for candidates
            _caseManagerClient.ResolveCaseStatusUpdates(new CaseManagement.Service.EmptyRequest());


            LogStatement(hangfireContext, "End of checks case resolve status.");
        }


        /// <summary>
        /// Log Statement
        /// </summary>
        /// <param name="hangfireContext"></param>
        /// <param name="message"></param>
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
