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
using static Rsbc.Dmf.BcMailAdapter.BcMailAdapter;

namespace Rsbc.Dmf.Scheduler
{
    public class ScheduledJobs
    {
        private IConfiguration _configuration { get; }
        private readonly ScheduledJobs _scheduledJobs;
        private readonly IcbcAdapterClient _icbcAdapterClient;
        private readonly CaseManagerClient _caseManagerClient;
        private readonly BcMailAdapterClient _bcMailAdapterClient;



        public ScheduledJobs(IConfiguration configuration, ScheduledJobs schedulerJobClient, IcbcAdapterClient icbcAdapterClient, CaseManagerClient caseManagerClient, BcMailAdapterClient bcMailAdapterClient )
        {
            _configuration = configuration;
            _scheduledJobs = schedulerJobClient;
            _icbcAdapterClient = icbcAdapterClient;
            _caseManagerClient = caseManagerClient;
            _bcMailAdapterClient = bcMailAdapterClient;
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
        /// Hangfire job fix Birth dates
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task UpdateBirthdate(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting to check the Date Of Birth");

            // Call ICBC Adapter to do the check for Date of birth
            _icbcAdapterClient.UpdateBirthdate(new IcbcAdapter.EmptyRequest());


            LogStatement(hangfireContext, "End of checks case resolve status.");
        }

        /// <summary>
        /// Hangfire job fix Birth dates
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendToBcMail(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting to check the file status");

            // Call BCMAil Adapter to get the list of files

            _bcMailAdapterClient.SendDocumentsToBcMail(new BcMailAdapter.EmptyRequest());


            LogStatement(hangfireContext, "End of check for the file status");
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
