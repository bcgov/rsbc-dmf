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
using Grpc.Net.Client;
using System.Net.Http;
using System.Net;

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


            if (_icbcAdapterClient == null)
            {

                // Add ICBC Adapter
                string icbcAdapterURI = _configuration["ICBC_ADAPTER_URI"];

                if (!string.IsNullOrEmpty(icbcAdapterURI))
                {
                    var httpClientHandler = new HttpClientHandler();
                    // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    

                    var httpClient = new HttpClient(httpClientHandler);
                    // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                    httpClient.DefaultRequestVersion = HttpVersion.Version20;

                    if (!string.IsNullOrEmpty(_configuration["ICBC_ADAPTER_JWT_SECRET"]))
                    {
                        var initialChannel = GrpcChannel.ForAddress(icbcAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

                        var initialClient = new IcbcAdapter.IcbcAdapter.IcbcAdapterClient(initialChannel);
                        // call the token service to get a token.
                        var tokenRequest = new IcbcAdapter.TokenRequest
                        {
                            Secret = _configuration["ICBC_ADAPTER_JWT_SECRET"]
                        };

                        var tokenReply = initialClient.GetToken(tokenRequest);

                        if (tokenReply != null && tokenReply.ResultStatus == IcbcAdapter.ResultStatus.Success)
                        {
                            // Add the bearer token to the client.
                            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                        }


                    }

                    var channel = GrpcChannel.ForAddress(icbcAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });
                    _icbcAdapterClient = new IcbcAdapter.IcbcAdapter.IcbcAdapterClient(channel);

                }
            }

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
        /// Hangfire job Update Clean pass Flag
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task UpdateCleanPassFlag(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting to check the case status");

            // Call ICBC Adapter to do the check for the document type is clean pass
            _caseManagerClient.UpdateCleanPassFlag(new CleanPassRequest());


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
