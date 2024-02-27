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
using System;

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
                    
                    var httpClient = new HttpClient(httpClientHandler) 
                    { Timeout = TimeSpan.FromMinutes(30),
                      DefaultRequestVersion = HttpVersion.Version20
                    };

                    if (!string.IsNullOrEmpty(_configuration["ICBC_ADAPTER_JWT_SECRET"]))
                    {
                        var initialChannel = GrpcChannel.ForAddress(icbcAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                        var initialClient = new IcbcAdapterClient(initialChannel);
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

                    var channel = GrpcChannel.ForAddress(icbcAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                    _icbcAdapterClient = new IcbcAdapter.IcbcAdapter.IcbcAdapterClient(channel);

                }
            }

            if (_bcMailAdapterClient == null)
            {
                //Add BC MAil Adapter 
                string bcmailAdapterURI = _configuration["BCMAIL_ADAPTER_URI"];

                if (!string.IsNullOrEmpty(bcmailAdapterURI))
                {
                    var httpClientHandler = new HttpClientHandler();
                   
                    // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    

                    var httpClient = new HttpClient(httpClientHandler);
                    // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                    httpClient.DefaultRequestVersion = HttpVersion.Version20;

                    if (!string.IsNullOrEmpty(_configuration["BCMAIL_ADAPTER_JWT_SECRET"]))
                    {
                        var initialChannel = GrpcChannel.ForAddress(bcmailAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                        var initialClient = new BcMailAdapterClient(initialChannel);
                        // call the token service to get a token.
                        var tokenRequest = new BcMailAdapter.TokenRequest
                        {
                            Secret = _configuration["BCMAIL_ADAPTER_JWT_SECRET"]
                        };

                        var tokenReply = initialClient.GetToken(tokenRequest);

                        if (tokenReply != null && tokenReply.ResultStatus == BcMailAdapter.ResultStatus.Success)
                        {
                            // Add the bearer token to the client.
                            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                        }


                    }

                    var channel = GrpcChannel.ForAddress(bcmailAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                    _bcMailAdapterClient = new BcMailAdapterClient(channel);
                    

                }
            }

            if(_caseManagerClient == null)
            {
                string cmsAdapterURI = _configuration["CMS_ADAPTER_URI"];

                if (!string.IsNullOrEmpty(cmsAdapterURI))
                {
                    var httpClientHandler = new HttpClientHandler();
                   
                    // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    

                    var httpClient = new HttpClient(httpClientHandler);
                    // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                    httpClient.DefaultRequestVersion = HttpVersion.Version20;

                    if (!string.IsNullOrEmpty(_configuration["CMS_ADAPTER_JWT_SECRET"]))
                    {
                        var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                        var initialClient = new CaseManager.CaseManagerClient(initialChannel);
                        // call the token service to get a token.
                        var tokenRequest = new CaseManagement.Service.TokenRequest
                        {
                            Secret = _configuration["CMS_ADAPTER_JWT_SECRET"]
                        };

                        var tokenReply = initialClient.GetToken(tokenRequest);

                        if (tokenReply != null && tokenReply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                        {
                            // Add the bearer token to the client.
                            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                        }


                    }

                    var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                    _caseManagerClient = new CaseManager.CaseManagerClient(channel);

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
        /// Hangfire job Update Non Comply Document Status
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task UpdateNonComplyDocuments(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting to check the case status");

            // Call ICBC Adapter to do the check for candidates
            _caseManagerClient.UpdateNonComplyDocuments(new CaseManagement.Service.EmptyRequest());


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
            LogStatement(hangfireContext, "Starting BC Mail Send");

            // Call BCMAil Adapter to get the list of files

            _bcMailAdapterClient.SendDocumentsToBcMail(new BcMailAdapter.EmptyRequest());


            LogStatement(hangfireContext, "End of check for the file status");
        }

        /// <summary>
        /// Hangfire job fix Birth dates
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task TestBcMail(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting to check BC Mail");

            // Call BCMAil Adapter to test the connection

            _bcMailAdapterClient.TestBcMail(new BcMailAdapter.EmptyRequest());


            LogStatement(hangfireContext, "End of check BC Mail");
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
