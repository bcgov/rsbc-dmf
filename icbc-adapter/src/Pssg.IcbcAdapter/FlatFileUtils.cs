using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;

namespace Pssg.IcbcAdapter
{
    public enum ChangeNameType
    {
        ChangeName = 1,
        Transfer = 2,
        ThirdPartyOperator = 3
    }
    public class FlatFileUtils
    {
        

        /// <summary>
        /// Maximum number of new licenses that will be sent per interval.
        /// </summary>
        private int maxLicencesPerInterval;

        private IConfiguration _configuration { get; }

        public FlatFileUtils(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForWork(PerformContext hangfireContext)
        {            
            LogStatement(hangfireContext, "Starting check for work.");

            // Attempt to connect to a SCP server.

            string username = _configuration["SCP_USER"];
            string password = _configuration["SCP_PASS"];
            string host = _configuration["SCP_HOST"];



            var connectionInfo = new ConnectionInfo(host,
                                        username,
                                        new PasswordAuthenticationMethod(username, password));
            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();
                LogStatement(hangfireContext, "Connected.");

                var files = client.ListDirectory(".");

                foreach (var file in files)
                {
                    LogStatement(hangfireContext, file.Name);
                }

            }

            LogStatement(hangfireContext, "End of check for work.");            
        }

        private void LogStatement(PerformContext hangfireContext, string message)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine(message);
            }
        }

    }
}

